using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.Extensions.Logging;
using PIKA.Infraestructura.Comun;
using PIKA.Infraestructura.Comun.Excepciones;
using PIKA.Infraestructura.Comun.Interfaces;
using PIKA.Infraestructura.Comun.Servicios;
using PIKA.Modelo.Contenido;
using PIKA.Servicio.Contenido.Interfaces;
using RepositorioEntidades;

namespace PIKA.Servicio.Contenido.Servicios
{
   public class ServicioPuntoMontaje : ContextoServicioContenido,
        IServicioInyectable, IServicioPuntoMontaje
    {
        private const string DEFAULT_SORT_COL = "Nombre";
        private const string DEFAULT_SORT_DIRECTION = "asc";

        private IRepositorioAsync<PuntoMontaje> repo;
        private IRepositorioAsync<VolumenPuntoMontaje> repoVPM;
        private UnidadDeTrabajo<DbContextContenido> UDT;

        public ServicioPuntoMontaje(
            IProveedorOpcionesContexto<DbContextContenido> proveedorOpciones,
            ILogger<ServicioLog> Logger
        ) : base(proveedorOpciones, Logger)
        {
            this.UDT = new UnidadDeTrabajo<DbContextContenido>(contexto);
            this.repo = UDT.ObtenerRepositoryAsync<PuntoMontaje>(new QueryComposer<PuntoMontaje>());
            this.repoVPM = UDT.ObtenerRepositoryAsync<VolumenPuntoMontaje>(new QueryComposer<VolumenPuntoMontaje>());
        }




        public async Task<bool> Existe(Expression<Func<PuntoMontaje, bool>> predicado)
        {
            List<PuntoMontaje> l = await this.repo.ObtenerAsync(predicado);
            if (l.Count() == 0) return false;
            return true;
        }


        public async Task<PuntoMontaje> CrearAsync(PuntoMontaje entity, CancellationToken cancellationToken = default)
        {

          
            if(await Existe(x=>x.Nombre == entity.Nombre 
            && x.OrigenId  == entity.OrigenId 
            && x.TipoOrigenId == entity.TipoOrigenId))
            {
                throw new ExElementoExistente(entity.Nombre);
            }


            try
            {

                entity.Id = System.Guid.NewGuid().ToString();
                entity.FechaCreacion = DateTime.UtcNow;
                entity.Eliminada = false;


                if (!string.IsNullOrEmpty(entity.VolumenDefaultId))
                {
                    VolumenPuntoMontaje vpm = new VolumenPuntoMontaje() { 
                        PuntoMontajeId = entity.Id, 
                        VolumenId = entity.VolumenDefaultId };
                    await repoVPM.CrearAsync(vpm);
                }

                await this.repo.CrearAsync(entity);

                UDT.SaveChanges();

                return entity.Copia();
            }
            catch (DbUpdateException)
            {
                throw new ExErrorRelacional("El identificador de tipo de volumen por default no es válido");
            }
            catch (Exception ex)
            {
                
                throw ex;
            }
           

        }


        public async Task ActualizarAsync(PuntoMontaje entity)
        {

            PuntoMontaje o = await this.repo.UnicoAsync(x => x.Id == entity.Id);

            if (o == null)
            {
                throw new EXNoEncontrado(entity.Id);
            }

            if (await Existe(x => x.Nombre == entity.Nombre && x.OrigenId == o.OrigenId
           && x.TipoOrigenId == o.TipoOrigenId && x.Id != entity.Id))
            {
                throw new ExElementoExistente(entity.Nombre);
            }
 

            try
            {

              
               o.Nombre = entity.Nombre;
               o.Eliminada = entity.Eliminada;
                o.VolumenDefaultId = entity.VolumenDefaultId;

                if (!string.IsNullOrEmpty(entity.VolumenDefaultId))
                {
                    VolumenPuntoMontaje vpm = await repoVPM.UnicoAsync(x => x.VolumenId == entity.VolumenDefaultId
                    && x.PuntoMontajeId == o.Id);
                    if (vpm == null)
                    {
                        vpm = new VolumenPuntoMontaje()
                        {
                            PuntoMontajeId = entity.Id,
                            VolumenId = entity.VolumenDefaultId
                        };
                        await repoVPM.CrearAsync(vpm);
                    }
                }

                UDT.Context.Entry(o).State = EntityState.Modified;
               UDT.SaveChanges();
            }
            catch (DbUpdateException)
            {
                throw new ExErrorRelacional("El identificador de tipo de volumen por default no es válido");
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
        public async Task<IPaginado<PuntoMontaje>> ObtenerPaginadoAsync(Consulta Query, Func<IQueryable<PuntoMontaje>, IIncludableQueryable<PuntoMontaje, object>> include = null, bool disableTracking = true, CancellationToken cancellationToken = default)
        {
            Query = GetDefaultQuery(Query);
            var respuesta = await this.repo.ObtenerPaginadoAsync(Query, include);
            return respuesta;
        }

      

        public async Task<ICollection<string>> Eliminar(string[] ids)
        {
            PuntoMontaje d;
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
            PuntoMontaje d;
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



        public async Task<PuntoMontaje> UnicoAsync(Expression<Func<PuntoMontaje, bool>> predicado = null, Func<IQueryable<PuntoMontaje>, IOrderedQueryable<PuntoMontaje>> ordenarPor = null, Func<IQueryable<PuntoMontaje>, IIncludableQueryable<PuntoMontaje, object>> incluir = null, bool inhabilitarSegumiento = true)
        {

            PuntoMontaje d = await this.repo.UnicoAsync(predicado, ordenarPor, incluir);

            return d.Copia();
        }


        public async Task<List<string>> Purgar()
        {
            List<PuntoMontaje> ListaCarpeta = await this.repo.ObtenerAsync(x=>x.Eliminada==true);

            throw new NotImplementedException();
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



        #region No Implemenatdaos

        public Task<IEnumerable<PuntoMontaje>> CrearAsync(params PuntoMontaje[] entities)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<PuntoMontaje>> CrearAsync(IEnumerable<PuntoMontaje> entities, CancellationToken cancellationToken = default)
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

        public Task<List<PuntoMontaje>> ObtenerAsync(Expression<Func<PuntoMontaje, bool>> predicado)
        {
            return this.repo.ObtenerAsync(predicado);
        }

        public Task<List<PuntoMontaje>> ObtenerAsync(string SqlCommand)
        {
            return this.repo.ObtenerAsync(SqlCommand);
        }

        public Task<IPaginado<PuntoMontaje>> ObtenerPaginadoAsync(Expression<Func<PuntoMontaje, bool>> predicate = null, Func<IQueryable<PuntoMontaje>, IOrderedQueryable<PuntoMontaje>> orderBy = null, Func<IQueryable<PuntoMontaje>, IIncludableQueryable<PuntoMontaje, object>> include = null, int index = 0, int size = 20, bool disableTracking = true, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

       




        #endregion
    }



}