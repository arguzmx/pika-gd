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
using Microsoft.Extensions.Logging;
using PIKA.Constantes.Aplicaciones.Seguridad;
using PIKA.Infraestructura.Comun;
using PIKA.Infraestructura.Comun.Excepciones;
using PIKA.Infraestructura.Comun.Interfaces;
using PIKA.Infraestructura.Comun.Seguridad;
using PIKA.Infraestructura.Comun.Servicios;
using PIKA.Servicio.Seguridad.Interfaces;
using RepositorioEntidades;

namespace PIKA.Servicio.Seguridad.Servicios
{

    public class ServicioAplicacion : ContextoServicioSeguridad
        , IServicioInyectable, IServicioAplicacion

    {
        private const string DEFAULT_SORT_COL = "Nombre";
        private const string DEFAULT_SORT_DIRECTION = "asc";
        private IRepositorioAsync<Aplicacion> repo;

        public ServicioAplicacion(
         IAppCache cache,
         IRegistroAuditoria registroAuditoria,
         IProveedorOpcionesContexto<DbContextSeguridad> proveedorOpciones,
         ILogger<ServicioLog> Logger
         ) : base(registroAuditoria, proveedorOpciones, Logger,
                 cache, ConstantesAppSeguridad.APP_ID, ConstantesAppSeguridad.MODULO_AUDITORIA)
        {
            this.repo = UDT.ObtenerRepositoryAsync<Aplicacion>(new QueryComposer<Aplicacion>());
        }

        public async Task<bool> Existe(Expression<Func<Aplicacion, bool>> predicado)
        {
            List<Aplicacion> l = await this.repo.ObtenerAsync(predicado);
            if (l.Count() == 0) return false;
            return true;
        }


        public async Task<Aplicacion> CrearAsync(Aplicacion entity, CancellationToken cancellationToken = default)
        {

            if (await Existe(x => x.Nombre.Equals(entity.Nombre, StringComparison.InvariantCultureIgnoreCase)))
            {
                throw new ExElementoExistente(entity.Nombre);
            }

            entity.Id = System.Guid.NewGuid().ToString();
            await this.repo.CrearAsync(entity);
            UDT.SaveChanges();
            return entity.Copia();
        }

        public async Task ActualizarAsync(Aplicacion entity)
        {

            Aplicacion o = await this.repo.UnicoAsync(x => x.Id == entity.Id);

            if (o == null)
            {
                throw new EXNoEncontrado(entity.Id);
            }

            if (await Existe(x =>
            x.Id != entity.Id
            && x.Nombre.Equals(entity.Nombre, StringComparison.InvariantCultureIgnoreCase)))
            {
                throw new ExElementoExistente(entity.Nombre);
            }

            o.Nombre = entity.Nombre;
            o.Descripcion = entity.Descripcion;
            o.UICulture = entity.UICulture;
            o.Version = entity.Version;
            o.ReleaseIndex = entity.ReleaseIndex;

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
        public async Task<IPaginado<Aplicacion>> ObtenerPaginadoAsync(Consulta Query, Func<IQueryable<Aplicacion>, IIncludableQueryable<Aplicacion, object>> include = null, bool disableTracking = true, CancellationToken cancellationToken = default)
        {
            Query = GetDefaultQuery(Query);
            var respuesta = await this.repo.ObtenerPaginadoAsync(Query, null);

            return respuesta;
        }


        public async Task<Aplicacion> UnicoAsync(Expression<Func<Aplicacion, bool>> predicado = null, Func<IQueryable<Aplicacion>, IOrderedQueryable<Aplicacion>> ordenarPor = null, Func<IQueryable<Aplicacion>, IIncludableQueryable<Aplicacion, object>> incluir = null, bool inhabilitarSegumiento = true)
        {

            Aplicacion d = await this.repo.UnicoAsync(predicado);

            return d.Copia();
        }
        public Task<List<Aplicacion>> ObtenerAsync(Expression<Func<Aplicacion, bool>> predicado)
        {
            return this.repo.ObtenerAsync(predicado);
        }
        public async Task<ICollection<string>> Eliminar(string[] ids)
        {
            Aplicacion o;
            ICollection<string> listaEliminados = new HashSet<string>();
            foreach (var Id in ids)
            {
                o = await this.repo.UnicoAsync(x => x.Id == Id.Trim());
                if (o != null)
                {
                    try
                    {
                        o = await this.repo.UnicoAsync(x => x.Id == Id);
                        if (o != null)
                        {
                            await this.repo.Eliminar(o);
                        }
                        this.UDT.SaveChanges();
                        listaEliminados.Add(o.Id);
                    }
                    catch (DbUpdateException)
                    {
                        throw new ExErrorRelacional(Id);
                    }
                    catch (Exception)
                    {
                        throw;
                    }
                }
            }
            UDT.SaveChanges();

            return listaEliminados;

        }

        public async Task<IEnumerable<string>> Restaurar(string[] ids)
        {
            Aplicacion c;
            ICollection<string> listaEliminados = new HashSet<string>();
            foreach (var Id in ids)
            {
                c = await this.repo.UnicoAsync(x => x.Id == Id);
                if (c != null)
                {
                    listaEliminados.Add(c.Id);
                }
            }
            UDT.SaveChanges();
            return listaEliminados;
        }

        #region sin implementar

        public Task<IEnumerable<Aplicacion>> CrearAsync(params Aplicacion[] entities)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Aplicacion>> CrearAsync(IEnumerable<Aplicacion> entities, CancellationToken cancellationToken = default)
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


        public Task<List<Aplicacion>> ObtenerAsync(string SqlCommand)
        {
            throw new NotImplementedException();
        }

        public Task<IPaginado<Aplicacion>> ObtenerPaginadoAsync(Expression<Func<Aplicacion, bool>> predicate = null, Func<IQueryable<Aplicacion>, IOrderedQueryable<Aplicacion>> orderBy = null, Func<IQueryable<Aplicacion>, IIncludableQueryable<Aplicacion, object>> include = null, int index = 0, int size = 20, bool disableTracking = true, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }


        #endregion


    }
}
