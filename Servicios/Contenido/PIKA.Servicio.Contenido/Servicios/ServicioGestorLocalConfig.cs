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
using PIKA.Modelo.Contenido;
using PIKA.Servicio.Contenido.Gestores;
using PIKA.Servicio.Contenido.Helpers;
using PIKA.Servicio.Contenido.Interfaces;
using RepositorioEntidades;
using GestorLocalConfig = PIKA.Modelo.Contenido.GestorLocalConfig;
namespace PIKA.Servicio.Contenido.Servicios
{
    public class ServicioGestorLocalConfig : ContextoServicioContenido,
        IServicioInyectable, IServicioGestorLocalConfig
    {
        private const string DEFAULT_SORT_COL = "VolumenId";
        private const string DEFAULT_SORT_DIRECTION = "asc";

        private IRepositorioAsync<GestorLocalConfig> repo;
        private IRepositorioAsync<Volumen> repoVol;
        private UnidadDeTrabajo<DbContextContenido> UDT;
        private IOptions<ConfiguracionServidor> configServer;
        public ServicioGestorLocalConfig(
            IProveedorOpcionesContexto<DbContextContenido> proveedorOpciones,
        ILogger<ServicioGestorLocalConfig> Logger, 
        IOptions<ConfiguracionServidor> opciones) : base(proveedorOpciones, Logger)
        {
            this.configServer = opciones;
            this.UDT = new UnidadDeTrabajo<DbContextContenido>(contexto);
            this.repo = UDT.ObtenerRepositoryAsync<GestorLocalConfig>( new QueryComposer<GestorLocalConfig>());
            this.repoVol = UDT.ObtenerRepositoryAsync<Volumen>(new QueryComposer<Volumen>());
        }

        public async Task<bool> Existe(Expression<Func<GestorLocalConfig, bool>> predicado)
        {
            List<GestorLocalConfig> l = await this.repo.ObtenerAsync(predicado);
            if (l.Count() == 0) return false;
            return true;
        }


        private async Task ValidaEntidad(GestorLocalConfig entity)
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

            GestorLocal g = new GestorLocal(entity, v, configServer);
            if (!g.ConexionValida() )
            {
                throw new ExDatosNoValidos($"Sin acceso a ruta: {entity.Ruta}");
            }

        }

        public async Task<GestorLocalConfig> CrearAsync(GestorLocalConfig entity, CancellationToken cancellationToken = default)
        {
            try
            {
                GestorLocalConfig o = await this.repo.UnicoAsync(x => x.VolumenId == entity.VolumenId);
                if (o == null)
                {
                    await ValidaEntidad(entity);
                    await this.repo.CrearAsync(entity);
                    UDT.SaveChanges();

                } else
                {
                    await ActualizarAsync(entity);
                }
                

                return entity.Copia();
            }
            catch (DbUpdateException)
            {
                throw new ExErrorRelacional("Alguno de los identificadores no es válido");
            }
            catch (Exception ex)
            {
                logger.LogError("Error al crear Unidad Organizacional {0}", ex.Message);
                throw ex;
            }
        }

   

        public async Task ActualizarAsync(GestorLocalConfig entity)
        {
            GestorLocalConfig o =  await this.repo.UnicoAsync(x => x.VolumenId == entity.VolumenId);

                if (o == null)
                {
                    throw new EXNoEncontrado(entity.VolumenId);
                }

            await ValidaEntidad(entity);

            o.Ruta = entity.Ruta;

            UDT.Context.Entry(o).State = EntityState.Modified;
            UDT.SaveChanges();


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
        public async Task<IPaginado<GestorLocalConfig>> ObtenerPaginadoAsync(Consulta Query, Func<IQueryable<GestorLocalConfig>, IIncludableQueryable<GestorLocalConfig, object>> include = null, bool disableTracking = true, CancellationToken cancellationToken = default)
        {
            Query = GetDefaultQuery(Query);
            var respuesta = await this.repo.ObtenerPaginadoAsync(Query, null);

            return respuesta;
        }

        public async Task<ICollection<string>> Eliminar(string[] ids)
        {
            GestorLocalConfig d;
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

       public async Task<GestorLocalConfig> UnicoAsync(Expression<Func<GestorLocalConfig, bool>> predicado = null, Func<IQueryable<GestorLocalConfig>, IOrderedQueryable<GestorLocalConfig>> ordenarPor = null, Func<IQueryable<GestorLocalConfig>, IIncludableQueryable<GestorLocalConfig, object>> incluir = null, bool inhabilitarSegumiento = true)
        {

            var o = await this.repo.UnicoAsync(predicado);

            return o.Copia();
        }

        #region No Implementado


        public Task<List<GestorLocalConfig>> ObtenerAsync(Expression<Func<GestorLocalConfig, bool>> predicado)
        {
            throw new NotImplementedException();
        }


        public Task<List<GestorLocalConfig>> ObtenerAsync(string SqlCommand)
        {
            throw new NotImplementedException();
        }



        public Task<IEnumerable<GestorLocalConfig>> CrearAsync(params GestorLocalConfig[] entities)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<GestorLocalConfig>> CrearAsync(IEnumerable<GestorLocalConfig> entities, CancellationToken cancellationToken = default)
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

        public Task<IPaginado<GestorLocalConfig>> ObtenerPaginadoAsync(Expression<Func<GestorLocalConfig, bool>> predicate = null, Func<IQueryable<GestorLocalConfig>, IOrderedQueryable<GestorLocalConfig>> orderBy = null, Func<IQueryable<GestorLocalConfig>, IIncludableQueryable<GestorLocalConfig, object>> include = null, int index = 0, int size = 20, bool disableTracking = true, CancellationToken cancellationToken = default)
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