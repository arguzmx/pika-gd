using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using LazyCache;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PIKA.Infraestructura.Comun;
using PIKA.Infraestructura.Comun.Excepciones;
using PIKA.Infraestructura.Comun.Interfaces;
using PIKA.Infraestructura.Comun.Servicios;
using PIKA.Modelo.Contenido;
using PIKA.Servicio.Contenido.Gestores;
using PIKA.Servicio.Contenido.Interfaces;
using RepositorioEntidades;

namespace PIKA.Servicio.Contenido.Servicios
{
   public class ServicioVolumen : ContextoServicioContenido,
        IServicioInyectable, IServicioVolumen
    {
        private const string DEFAULT_SORT_COL = "Nombre";
        private const string DEFAULT_SORT_DIRECTION = "asc";

        private IRepositorioAsync<Volumen> repo;
        private UnidadDeTrabajo<DbContextContenido> UDT;
        private static TimeSpan volCacheExpiry = new TimeSpan(0, 10, 0); 
        private readonly IAppCache lazycache;
        private IOptions<ConfiguracionServidor> opciones;
        public ServicioVolumen(
            IProveedorOpcionesContexto<DbContextContenido> proveedorOpciones,
            ILogger<ServicioLog> Logger,
            IAppCache lazycache,
            IOptions<ConfiguracionServidor> opciones
        ) : base(proveedorOpciones, Logger)
        {
            this.opciones = opciones;
            this.lazycache = lazycache;
            this.UDT = new UnidadDeTrabajo<DbContextContenido>(contexto);
            this.repo = UDT.ObtenerRepositoryAsync<Volumen>(new QueryComposer<Volumen>());
        }

        private string GetCacheVolKey(string VolumenId)
        {
            return $"cache-vol-{VolumenId}";
        }

        private string GetCacheGestorESKey(string VolumenId)
        {
            return $"cache-gestor-{VolumenId}";
        }

        public async Task<IGestorES> ObtienInstanciaGestor(string VolumenId)
        {
            Volumen vol = null;
            IGestorES gestor = null;
            if (!string.IsNullOrEmpty(VolumenId))
            {
                string key = GetCacheVolKey(VolumenId);
                vol = lazycache.Get<Volumen>(key);
                if (vol == null)
                {
                    vol = await repo.UnicoAsync(x => x.Id.Equals(VolumenId.Trim(), StringComparison.InvariantCultureIgnoreCase));
                    if (vol != null)
                    {
                        this.lazycache.Add<Volumen>(key, vol, volCacheExpiry);
                    }
                }
            }

            if( vol!=null)
            {
                string key = GetCacheGestorESKey(vol.Id);
                switch (vol.TipoGestorESId)
                {
                    case TipoGestorES.LOCAL_FOLDER:
                        GestorLocalConfig config = lazycache.Get<GestorLocalConfig>(key);
                        if (config == null)
                        {
                            IRepositorioAsync<GestorLocalConfig> repoconfig = UDT.ObtenerRepositoryAsync<GestorLocalConfig>(new QueryComposer<GestorLocalConfig>()); ;
                            config = await repoconfig.UnicoAsync(x => x.VolumenId.Equals(VolumenId.Trim(), StringComparison.InvariantCultureIgnoreCase));
                            if (config != null)
                            {
                                this.lazycache.Add<GestorLocalConfig>(key, config, volCacheExpiry);
                            }
                        }

                        if (config != null)
                        {
                            gestor = new GestorLocal(logger, config, opciones);
                        }
                        break;

                    case TipoGestorES.SMB:
                        break;

                    case TipoGestorES.AzureBlob:
                        break;

                }
            }
            

            return gestor;
        }


        public async Task<List<ValorListaOrdenada>> ObtenerParesAsync(Consulta Query)
        {
            for (int i = 0; i < Query.Filtros.Count; i++)
            {
                if (Query.Filtros[i].Propiedad.ToLower() == "texto")
                {
                    Query.Filtros[i].Propiedad = "Nombre";
                }
            }
            if (Query.Filtros.Where(x => x.Propiedad.ToLower() == "eliminada").Count() == 0)
            {
                Query.Filtros.Add(new FiltroConsulta()
                {
                    Propiedad = "Eliminada",
                    Negacion = true,
                    Operador = "eq",
                    Valor = "true"
                });
            }
            Query = GetDefaultQuery(Query);
            var resultados = await this.repo.ObtenerPaginadoAsync(Query);
            List<ValorListaOrdenada> l = resultados.Elementos.Select(x => new ValorListaOrdenada()
            {
                Id = x.Id,
                Indice = 0,
                Texto = x.Nombre
            }).ToList();

            return l.OrderBy(x => x.Texto).ToList();
        }


        public async Task<List<ValorListaOrdenada>> ObtenerParesPorId(List<string> Lista)
        {
            var resultados = await this.repo.ObtenerAsync(x => Lista.Contains(x.Id));
            List<ValorListaOrdenada> l = resultados.Select(x => new ValorListaOrdenada()
            {
                Id = x.Id,
                Indice = 0,
                Texto = x.Nombre
            }).ToList();

            return l.OrderBy(x => x.Texto).ToList();
        }



        public async Task<bool> Existe(Expression<Func<Volumen, bool>> predicado)
        {
            List<Volumen> l = await this.repo.ObtenerAsync(predicado);
            if (l.Count() == 0) return false;
            return true;
        }


        public async Task<Volumen> CrearAsync(Volumen entity, CancellationToken cancellationToken = default)
        {

          
            if(await Existe(x=>x.Nombre == entity.Nombre 
            && x.OrigenId  != entity.OrigenId 
            && x.TipoOrigenId == entity.TipoOrigenId))
            {
                throw new ExElementoExistente(entity.Nombre);
            }


            try
            {

                entity.Id = System.Guid.NewGuid().ToString();
                // Se actualizará a activo cuando se configure la conexión del tipo de gestor

                entity.Activo = true; 
                entity.EscrituraHabilitada = true;
                entity.Eliminada = false;
                entity.ConsecutivoVolumen = 0;
                entity.CanidadPartes = 0;
                entity.CanidadElementos = 0;
                await this.repo.CrearAsync(entity);

                UDT.SaveChanges();

                return entity.Copia();
            }
            catch (DbUpdateException)
            {
                throw new ExErrorRelacional("El identificador de tipo de gestor no es válido");
            }
            catch (Exception ex)
            {
                throw ex;
            }
           

        }


        public async Task ActualizarAsync(Volumen entity)
        {

            Volumen o = await this.repo.UnicoAsync(x => x.Id == entity.Id);

            if (o == null)
            {
                throw new EXNoEncontrado(entity.Id);
            }

            if (await Existe(x => x.Nombre == entity.Nombre && x.OrigenId == o.OrigenId
           && x.TipoOrigenId == o.TipoOrigenId && x.Id != entity.Id))
            {
                throw new ExElementoExistente(entity.Nombre);
            }

            if (o.Tamano > entity.TamanoMaximo)
            {
                throw new ExDatosNoValidos("El tamaño máximo es menor al actual");
            }

            try
            {

                /// Solo se permiten cambios en el estao una vez que l aconfiguración es válida
                if( o.ConfiguracionValida)
                {
                    o.Activo = entity.Activo;
                }


                if(o.TipoGestorESId != entity.TipoGestorESId)
                {
                    o.TipoGestorESId = entity.TipoGestorESId;
                    o.ConfiguracionValida = false;
                }

                o.Nombre = entity.Nombre;
               o.EscrituraHabilitada = entity.EscrituraHabilitada;
               o.TamanoMaximo = entity.TamanoMaximo;

               UDT.Context.Entry(o).State = EntityState.Modified;
               UDT.SaveChanges();
            }
            catch (DbUpdateException)
            {
                throw new ExErrorRelacional("El identificador de tipo de gestor no es válido");
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        private Consulta GetDefaultQuery(Consulta query)
        {
            if (query != null)
            {
                query.indice = query.indice < 0 ? 0 : query.indice;
                query.tamano = query.tamano <= 0 ? 20 : query.tamano;
                query.ord_columna = string.IsNullOrEmpty(query.ord_columna) ? DEFAULT_SORT_COL : query.ord_columna;
                query.ord_direccion = string.IsNullOrEmpty(query.ord_direccion) ? DEFAULT_SORT_DIRECTION : query.ord_direccion;
            }
            else
            {
                query = new Consulta() { indice = 0, tamano = 20, ord_columna = DEFAULT_SORT_COL, ord_direccion = DEFAULT_SORT_DIRECTION };
            }
            return query;
        }
        public async Task<IPaginado<Volumen>> ObtenerPaginadoAsync(Consulta Query, Func<IQueryable<Volumen>, IIncludableQueryable<Volumen, object>> include = null, bool disableTracking = true, CancellationToken cancellationToken = default)
        {
            Query = GetDefaultQuery(Query);
            var respuesta = await this.repo.ObtenerPaginadoAsync(Query, include);
            return respuesta;
        }

      

        public async Task<ICollection<string>> Eliminar(string[] ids)
        {
            Volumen d;
            ICollection<string> listaEliminados = new HashSet<string>();
            foreach (var Id in ids.LimpiaIds())
            {
                d = await this.repo.UnicoAsync(x => x.Id == Id);
                if (d != null)
                {
                    d.Eliminada = true;
                    this.UDT.Context.Entry(d).State = EntityState.Modified;
                    listaEliminados.Add(d.Id);
                }
            }
            UDT.SaveChanges();
            return listaEliminados;
        }

        public async Task<IEnumerable<string>> Restaurar(string[] ids)
        {
            Volumen d;
            ICollection<string> listaEliminados = new HashSet<string>();
            foreach (var Id in ids.LimpiaIds())
            {
                d = await this.repo.UnicoAsync(x => x.Id == Id);
                if (d != null)
                {
                    d.Eliminada = false;
                    this.UDT.Context.Entry(d).State = EntityState.Modified;
                    listaEliminados.Add(d.Id);
                }
            }
            UDT.SaveChanges();
            return listaEliminados;
        }



        public async Task<Volumen> UnicoAsync(Expression<Func<Volumen, bool>> predicado = null, Func<IQueryable<Volumen>, IOrderedQueryable<Volumen>> ordenarPor = null, Func<IQueryable<Volumen>, IIncludableQueryable<Volumen, object>> incluir = null, bool inhabilitarSegumiento = true)
        {

            Volumen d = await this.repo.UnicoAsync(predicado, ordenarPor, incluir);


            return d.Copia();
        }
        public async Task<List<string>> Purgar()
        {
            List<Volumen> ListaVolumenEli = await this.repo.ObtenerAsync(x=>x.Eliminada==true).ConfigureAwait(false);
            string[] IdsEliminar = ListaVolumenEli.Select(x=>x.Id).ToArray();
            // ServicioElemento se = new ServicioElemento(this.proveedorOpciones,this.logger);
            ServicioVolumenPuntoMontaje svpm = new ServicioVolumenPuntoMontaje(this.proveedorOpciones, this.logger);
            ServicioTipoGestorES sTes = new ServicioTipoGestorES(this.proveedorOpciones, this.logger);
            ServicioParte sp = new ServicioParte(this.proveedorOpciones, this.logger);
            ServicioVersion sv = new ServicioVersion(this.proveedorOpciones, this.logger);
            ServicioGestorAzureConfig sgvc = new ServicioGestorAzureConfig(this.proveedorOpciones, this.logger);
            ServicioGestorLocalConfig sglc = new ServicioGestorLocalConfig(this.proveedorOpciones, this.logger,opciones);
            ServicioGestorSMBConfig sgsmb = new ServicioGestorSMBConfig(this.proveedorOpciones, this.logger);
            await sgvc.Eliminar(IdsEliminar);
            await sglc.Eliminar(IdsEliminar);
            await sgsmb.Eliminar(IdsEliminar);
            //await se.Purgar();

            throw new NotImplementedException();
        }


        #region No Implemenatdaos

        public Task<IEnumerable<Volumen>> CrearAsync(params Volumen[] entities)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Volumen>> CrearAsync(IEnumerable<Volumen> entities, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public async Task EjecutarSql(string sqlCommand)
        {
            throw new NotImplementedException();
        }

        public Task EjecutarSqlBatch(List<string> sqlCommand)
        {
            throw new NotImplementedException();
        }

        public Task<List<Volumen>> ObtenerAsync(Expression<Func<Volumen, bool>> predicado)
        {
            return this.repo.ObtenerAsync(predicado);
        }

        public Task<List<Volumen>> ObtenerAsync(string SqlCommand)
        {
            return this.repo.ObtenerAsync(SqlCommand);
        }

        public Task<IPaginado<Volumen>> ObtenerPaginadoAsync(Expression<Func<Volumen, bool>> predicate = null, Func<IQueryable<Volumen>, IOrderedQueryable<Volumen>> orderBy = null, Func<IQueryable<Volumen>, IIncludableQueryable<Volumen, object>> include = null, int index = 0, int size = 20, bool disableTracking = true, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

      


        #endregion
    }



}