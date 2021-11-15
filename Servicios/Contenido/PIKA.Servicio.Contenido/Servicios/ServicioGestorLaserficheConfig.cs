using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
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
using PIKA.Servicio.Contenido.Helpers;
using PIKA.Servicio.Contenido.Interfaces;
using RepositorioEntidades;
using GestorLaserficheConfig = PIKA.Modelo.Contenido.GestorLaserficheConfig;
namespace PIKA.Servicio.Contenido.Servicios
{
    public class ServicioGestorLaserficheConfig : ContextoServicioContenido,
        IServicioInyectable, IServicioGestorLaserficheConfig
    {
        private const string DEFAULT_SORT_COL = "VolumenId";
        private const string DEFAULT_SORT_DIRECTION = "asc";

        private IRepositorioAsync<Volumen> repoVol;
        private IRepositorioAsync<GestorLaserficheConfig> repo;
        private UnidadDeTrabajo<DbContextContenido> UDT;
        private IOptions<ConfiguracionServidor> configServer;
        public ServicioGestorLaserficheConfig(
            IProveedorOpcionesContexto<DbContextContenido> proveedorOpciones,
        ILogger<ServicioLog> Logger,
        IOptions<ConfiguracionServidor> opciones) : base(proveedorOpciones, Logger)
        {
            this.configServer = opciones;
            this.UDT = new UnidadDeTrabajo<DbContextContenido>(contexto);
            this.repo = UDT.ObtenerRepositoryAsync<GestorLaserficheConfig>( new QueryComposer<GestorLaserficheConfig>());
            this.repoVol = UDT.ObtenerRepositoryAsync<Volumen>(new QueryComposer<Volumen>());
        }

        public async Task<bool> Existe(Expression<Func<GestorLaserficheConfig, bool>> predicado)
        {
            List<GestorLaserficheConfig> l = await this.repo.ObtenerAsync(predicado);
            if (l.Count() == 0) return false;
            return true;
        }
        private async Task ValidaEntidad(GestorLaserficheConfig entity)
        {
            Volumen v = await repoVol.UnicoAsync(x => x.Id == entity.VolumenId);
            if (v == null)
            {
                throw new ExDatosNoValidos($"VolumenId: {entity.VolumenId}");
            }

            if (string.IsNullOrEmpty(entity.Ruta))
            {
                throw new ExDatosNoValidos($"Ruta: {entity.Ruta}");
            }

            GestorLaserfiche g = new GestorLaserfiche(this.logger, entity, configServer);
            if (!g.ConexionValida())
            {
                throw new ExDatosNoValidos($"Sin acceso a ruta: {entity.Ruta}");
            }

        }

        private async Task ActualizaEstadoVolumen(string Id, bool configuracionValida)
        {
            var v = await this.repoVol.UnicoAsync(x => x.Id == Id);
            if (v != null)
            {
                v.ConfiguracionValida = configuracionValida;
                UDT.Context.Entry(v).State = EntityState.Modified;
                UDT.SaveChanges();
            }
        }

        public async Task<GestorLaserficheConfig> CrearAsync(GestorLaserficheConfig entity, CancellationToken cancellationToken = default)
        {
            try
            {
                GestorLaserficheConfig o = await this.repo.UnicoAsync(x => x.VolumenId == entity.VolumenId);
                if (o == null)
                {
                    await ValidaEntidad(entity);
                    await this.repo.CrearAsync(entity);
                    UDT.SaveChanges();
                    await ActualizaEstadoVolumen(entity.VolumenId, true);

                } else
                {
                    await ActualizarAsync(entity);
                }
                

                return  entity.Copia();
            }
            catch (DbUpdateException)
            {
                throw new ExErrorRelacional("Alguno de los identificadores no es válido");
            }
            catch (Exception ex)
            {
                
                throw ex;
            }
        }
        public async Task ActualizarAsync(GestorLaserficheConfig entity)
        {
            GestorLaserficheConfig o =  await this.repo.UnicoAsync(x => x.VolumenId == entity.VolumenId);

                if (o == null)
                {
                    throw new EXNoEncontrado(entity.VolumenId);
                }
            await ValidaEntidad(entity);

            o.Ruta = entity.Ruta;

            UDT.Context.Entry(o).State = EntityState.Modified;
            UDT.SaveChanges();
            await ActualizaEstadoVolumen(entity.VolumenId, true);


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
        public async Task<IPaginado<GestorLaserficheConfig>> ObtenerPaginadoAsync(Consulta Query, Func<IQueryable<GestorLaserficheConfig>, IIncludableQueryable<GestorLaserficheConfig, object>> include = null, bool disableTracking = true, CancellationToken cancellationToken = default)
        {
            Query = GetDefaultQuery(Query);
            var respuesta = await this.repo.ObtenerPaginadoAsync(Query, null);

            return respuesta;
        }

        public async Task<ICollection<string>> Eliminar(string[] ids)
        {
            GestorLaserficheConfig d;
            ICollection<string> lista = new HashSet<string>();
            foreach (var Id in ids.LimpiaIds())
            {
                d = await this.repo.UnicoAsync(x => x.VolumenId == Id);
                if (d != null)
                {
                    lista.Add(Id);
                    await repo.Eliminar(d);
                 }
            }

            UDT.SaveChanges();
            return lista;
        }

       public async Task<GestorLaserficheConfig> UnicoAsync(Expression<Func<GestorLaserficheConfig, bool>> predicado = null, Func<IQueryable<GestorLaserficheConfig>, IOrderedQueryable<GestorLaserficheConfig>> ordenarPor = null, Func<IQueryable<GestorLaserficheConfig>, IIncludableQueryable<GestorLaserficheConfig, object>> incluir = null, bool inhabilitarSegumiento = true)
        {

            var o = await this.repo.UnicoAsync(predicado);

            return o.Copia();
        }

        #region No Implementado


        public Task<List<GestorLaserficheConfig>> ObtenerAsync(Expression<Func<GestorLaserficheConfig, bool>> predicado)
        {
            throw new NotImplementedException();
        }


        public Task<List<GestorLaserficheConfig>> ObtenerAsync(string SqlCommand)
        {
            throw new NotImplementedException();
        }



        public Task<IEnumerable<GestorLaserficheConfig>> CrearAsync(params GestorLaserficheConfig[] entities)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<GestorLaserficheConfig>> CrearAsync(IEnumerable<GestorLaserficheConfig> entities, CancellationToken cancellationToken = default)
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

        public Task<IPaginado<GestorLaserficheConfig>> ObtenerPaginadoAsync(Expression<Func<GestorLaserficheConfig, bool>> predicate = null, Func<IQueryable<GestorLaserficheConfig>, IOrderedQueryable<GestorLaserficheConfig>> orderBy = null, Func<IQueryable<GestorLaserficheConfig>, IIncludableQueryable<GestorLaserficheConfig, object>> include = null, int index = 0, int size = 20, bool disableTracking = true, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<string>> Restaurar(string[] ids)
        {
            throw new NotImplementedException();
        }


        #endregion





    }



}