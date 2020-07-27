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
using PIKA.Infraestructura.Comun;
using PIKA.Infraestructura.Comun.Excepciones;
using PIKA.Infraestructura.Comun.Interfaces;
using PIKA.Modelo.Contenido;
using PIKA.Servicio.Contenido.Helpers;
using PIKA.Servicio.Contenido.Interfaces;
using RepositorioEntidades;
using GestorSMBConfig = PIKA.Modelo.Contenido.GestorSMBConfig;
namespace PIKA.Servicio.Contenido.Servicios
{
    public class ServicioGestorSMBConfig : ContextoServicioContenido,
        IServicioInyectable, IServicioGestorSMBConfig
    {
        private const string DEFAULT_SORT_COL = "VolumenId";
        private const string DEFAULT_SORT_DIRECTION = "asc";

        private IRepositorioAsync<GestorSMBConfig> repo;
        private UnidadDeTrabajo<DbContextContenido> UDT;
        public ServicioGestorSMBConfig(
            IProveedorOpcionesContexto<DbContextContenido> proveedorOpciones,
        ILogger<ServicioGestorSMBConfig> Logger) : base(proveedorOpciones, Logger)
        {
            this.UDT = new UnidadDeTrabajo<DbContextContenido>(contexto);
            this.repo = UDT.ObtenerRepositoryAsync<GestorSMBConfig>( new QueryComposer<GestorSMBConfig>());
        }

        public async Task<bool> Existe(Expression<Func<GestorSMBConfig, bool>> predicado)
        {
            List<GestorSMBConfig> l = await this.repo.ObtenerAsync(predicado);
            if (l.Count() == 0) return false;
            return true;
        }


        public async Task<GestorSMBConfig> CrearAsync(GestorSMBConfig entity, CancellationToken cancellationToken = default)
        {


            try
            {
                GestorSMBConfig o = await this.repo.UnicoAsync(x => x.VolumenId == entity.VolumenId);
                if (o == null)
                {

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

   

        public async Task ActualizarAsync(GestorSMBConfig entity)
        {
            GestorSMBConfig o =  await this.repo.UnicoAsync(x => x.VolumenId == entity.VolumenId);

                if (o == null)
                {
                    throw new EXNoEncontrado(entity.VolumenId);
                }


            o.Ruta = entity.Ruta;
            o.Usuario = entity.Usuario;
            o.VolumenId = entity.VolumenId;
            o.Contrasena = entity.Contrasena;
            o.Dominio = entity.Dominio;

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
        public async Task<IPaginado<GestorSMBConfig>> ObtenerPaginadoAsync(Consulta Query, Func<IQueryable<GestorSMBConfig>, IIncludableQueryable<GestorSMBConfig, object>> include = null, bool disableTracking = true, CancellationToken cancellationToken = default)
        {
            Query = GetDefaultQuery(Query);
            var respuesta = await this.repo.ObtenerPaginadoAsync(Query, null);

            return respuesta;
        }

        public async Task<ICollection<string>> Eliminar(string[] ids)
        {
            GestorSMBConfig d;
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

       public async Task<GestorSMBConfig> UnicoAsync(Expression<Func<GestorSMBConfig, bool>> predicado = null, Func<IQueryable<GestorSMBConfig>, IOrderedQueryable<GestorSMBConfig>> ordenarPor = null, Func<IQueryable<GestorSMBConfig>, IIncludableQueryable<GestorSMBConfig, object>> incluir = null, bool inhabilitarSegumiento = true)
        {

            var o = await this.repo.UnicoAsync(predicado);

            return o.Copia();
        }

        #region No Implementado


        public Task<List<GestorSMBConfig>> ObtenerAsync(Expression<Func<GestorSMBConfig, bool>> predicado)
        {
            throw new NotImplementedException();
        }


        public Task<List<GestorSMBConfig>> ObtenerAsync(string SqlCommand)
        {
            throw new NotImplementedException();
        }



        public Task<IEnumerable<GestorSMBConfig>> CrearAsync(params GestorSMBConfig[] entities)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<GestorSMBConfig>> CrearAsync(IEnumerable<GestorSMBConfig> entities, CancellationToken cancellationToken = default)
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

        public Task<IPaginado<GestorSMBConfig>> ObtenerPaginadoAsync(Expression<Func<GestorSMBConfig, bool>> predicate = null, Func<IQueryable<GestorSMBConfig>, IOrderedQueryable<GestorSMBConfig>> orderBy = null, Func<IQueryable<GestorSMBConfig>, IIncludableQueryable<GestorSMBConfig, object>> include = null, int index = 0, int size = 20, bool disableTracking = true, CancellationToken cancellationToken = default)
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