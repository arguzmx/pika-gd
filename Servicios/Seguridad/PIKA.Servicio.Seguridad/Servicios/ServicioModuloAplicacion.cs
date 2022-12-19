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
    public class ServicioModuloAplicacion : ContextoServicioSeguridad, IServicioInyectable, IServicioModuloAplicacion
    {
        private const string DEFAULT_SORT_COL = "Nombre";
        private const string DEFAULT_SORT_DIRECTION = "asc";
        private IRepositorioAsync<ModuloAplicacion> repo;


        public ServicioModuloAplicacion(
              IAppCache cache,
         IRegistroAuditoria registroAuditoria,
          IProveedorOpcionesContexto<DbContextSeguridad> proveedorOpciones,
                  ILogger<ServicioLog> Logger
         ) : base(registroAuditoria, proveedorOpciones, Logger,
                 cache, ConstantesAppSeguridad.APP_ID, ConstantesAppSeguridad.MODULO_AUDITORIA)
        {
            this.repo = UDT.ObtenerRepositoryAsync<ModuloAplicacion>(new QueryComposer<ModuloAplicacion>());
        }

        public async Task<bool> Existe(Expression<Func<ModuloAplicacion, bool>> predicado)
        {
            List<ModuloAplicacion> l = await this.repo.ObtenerAsync(predicado);
            if (l.Count() == 0) return false;
            return true;
        }


        public async Task<ModuloAplicacion> CrearAsync(ModuloAplicacion entity, CancellationToken cancellationToken = default)
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

        public async Task ActualizarAsync(ModuloAplicacion entity)
        {

            ModuloAplicacion o = await this.repo.UnicoAsync(x => x.Id == entity.Id);

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
            o.ModuloPadreId = entity.ModuloPadreId;
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
        public async Task<IPaginado<ModuloAplicacion>> ObtenerPaginadoAsync(Consulta Query, Func<IQueryable<ModuloAplicacion>, IIncludableQueryable<ModuloAplicacion, object>> include = null, bool disableTracking = true, CancellationToken cancellationToken = default)
        {
            Query = GetDefaultQuery(Query);
            var respuesta = await this.repo.ObtenerPaginadoAsync(Query, null);

            return respuesta;
        }

       

        public async Task<ICollection<string>> Eliminar(string[] ids)
        {
            ModuloAplicacion o;
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

        public Task<List<ModuloAplicacion>> ObtenerAsync(Expression<Func<ModuloAplicacion, bool>> predicado)
        {
            return this.repo.ObtenerAsync(predicado);
        }

        public Task<List<ModuloAplicacion>> ObtenerAsync(string SqlCommand)
        {
            return this.repo.ObtenerAsync(SqlCommand);
        }

      


      
        public async Task<ModuloAplicacion> UnicoAsync(Expression<Func<ModuloAplicacion, bool>> predicado = null, Func<IQueryable<ModuloAplicacion>, IOrderedQueryable<ModuloAplicacion>> ordenarPor = null, Func<IQueryable<ModuloAplicacion>, IIncludableQueryable<ModuloAplicacion, object>> incluir = null, bool inhabilitarSegumiento = true)
        {

            ModuloAplicacion d = await this.repo.UnicoAsync(predicado);

            return d.Copia();
        }

        #region sin implementar
        public Task<IEnumerable<ModuloAplicacion>> CrearAsync(params ModuloAplicacion[] entities)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<ModuloAplicacion>> CrearAsync(IEnumerable<ModuloAplicacion> entities, CancellationToken cancellationToken = default)
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
        public Task<IPaginado<ModuloAplicacion>> ObtenerPaginadoAsync(Expression<Func<ModuloAplicacion, bool>> predicate = null, Func<IQueryable<ModuloAplicacion>, IOrderedQueryable<ModuloAplicacion>> orderBy = null, Func<IQueryable<ModuloAplicacion>, IIncludableQueryable<ModuloAplicacion, object>> include = null, int index = 0, int size = 20, bool disableTracking = true, CancellationToken cancellationToken = default)
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
