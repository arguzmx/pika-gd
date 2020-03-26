using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.Extensions.Logging;
using PIKA.Infraestructura.Comun;
using PIKA.Infraestructura.Comun.Excepciones;
using PIKA.Infraestructura.Comun.Interfaces;
using PIKA.Servicio.Seguridad.Interfaces;
using RepositorioEntidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PIKA.Servicio.Seguridad.Servicios
{
    public class ServicioModuloAplicacion : IServicioInyectable, IServicioModuloAplicacion
    {
        private const string DEFAULT_SORT_COL = "Nombre";
        private const string DEFAULT_SORT_DIRECTION = "asc";

        private IServicioCache cache;
        private IRepositorioAsync<ModuloAplicacion> repo;
        private ICompositorConsulta<ModuloAplicacion> compositor;
        private ILogger<ServicioModuloAplicacion> logger;
        private DbContextSeguridad contexto;
        private UnidadDeTrabajo<DbContextSeguridad> UDT;
        public ServicioModuloAplicacion(DbContextSeguridad contexto,
            ICompositorConsulta<ModuloAplicacion> compositorConsulta,
            ILogger<ServicioModuloAplicacion> Logger,
            IServicioCache servicioCache)
        {


            this.contexto = contexto;
            this.UDT = new UnidadDeTrabajo<DbContextSeguridad>(contexto);
            this.cache = servicioCache;
            this.logger = Logger;
            this.compositor = compositorConsulta;
            this.repo = UDT.ObtenerRepositoryAsync<ModuloAplicacion>(compositor);
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
            return entity;
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

        public Task<IEnumerable<ModuloAplicacion>> CrearAsync(params ModuloAplicacion[] entities)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<ModuloAplicacion>> CrearAsync(IEnumerable<ModuloAplicacion> entities, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task EjecutarSql(string sqlCommand)
        {
            throw new NotImplementedException();
        }

        public Task EjecutarSqlBatch(List<string> sqlCommand)
        {
            throw new NotImplementedException();
        }

        public Task Eliminar(string[] ids)
        {
            throw new NotImplementedException();
        }

        public Task<List<ModuloAplicacion>> ObtenerAsync(Expression<Func<ModuloAplicacion, bool>> predicado)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<ModuloAplicacion>> ObtenerListaAsync(string SqlCommand)
        {
            throw new NotImplementedException();
        }

        public Task<IPaginado<ModuloAplicacion>> ObtenerPaginadoAsync(Expression<Func<ModuloAplicacion, bool>> predicate = null, Func<IQueryable<ModuloAplicacion>, IOrderedQueryable<ModuloAplicacion>> orderBy = null, Func<IQueryable<ModuloAplicacion>, IIncludableQueryable<ModuloAplicacion, object>> include = null, int index = 0, int size = 20, bool disableTracking = true, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }



        public Task Restaurar(string[] ids)
        {
            throw new NotImplementedException();
        }

        public Task<ModuloAplicacion> UnicoAsync(Expression<Func<ModuloAplicacion, bool>> predicado = null, Func<IQueryable<ModuloAplicacion>, IOrderedQueryable<ModuloAplicacion>> ordenarPor = null, Func<IQueryable<ModuloAplicacion>, IIncludableQueryable<ModuloAplicacion, object>> incluir = null, bool inhabilitarSegumiento = true)
        {
            throw new NotImplementedException();
        }
    }
}
