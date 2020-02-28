using Microsoft.EntityFrameworkCore.Query;
using Microsoft.Extensions.Logging;
using PIKA.Infraestructura.Comun;
using PIKA.Infraestructura.Comun.Interfaces;
using PIKA.Modelo.Organizacion;
using RepositorioEntidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace PIKA.Servicio.Organizacion
{
    public class ServicioUnidadOrganizacional: IServicioInyectable, IServicioUnidadOrganizacional
    {

        private IServicioCache cache;
        private IRepositorioAsync<UnidadOrganizacional> repo;
        private ICompositorConsulta<UnidadOrganizacional> compositor;
        private ILogger<ServicioUnidadOrganizacional> logger;
        private DbContextOrganizacion contexto;
        private UnidadDeTrabajo<DbContextOrganizacion> UDT;
        public ServicioUnidadOrganizacional(DbContextOrganizacion contexto,
            ICompositorConsulta<UnidadOrganizacional> compositorConsulta,
            ILogger<ServicioUnidadOrganizacional> Logger, 
            IServicioCache servicioCache) {


            this.contexto = contexto;
            this.UDT = new UnidadDeTrabajo<DbContextOrganizacion>(contexto);
            this.cache = servicioCache;
            this.logger = Logger;
            this.compositor = compositorConsulta;
            this.repo = UDT.ObtenerRepositoryAsync<UnidadOrganizacional>( compositor);
        }

        public Task ActualizarAsync(UnidadOrganizacional entity)
        {
            throw new NotImplementedException();
        }

        public Task<UnidadOrganizacional> CrearAsync(UnidadOrganizacional entity, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<UnidadOrganizacional>> CrearAsync(params UnidadOrganizacional[] entities)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<UnidadOrganizacional>> CrearAsync(IEnumerable<UnidadOrganizacional> entities, CancellationToken cancellationToken = default)
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

        public Task<List<UnidadOrganizacional>> ObtenerAsync(Expression<Func<UnidadOrganizacional, bool>> predicado)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<UnidadOrganizacional>> ObtenerListaAsync(string SqlCommand)
        {
            throw new NotImplementedException();
        }

        public Task<IPaginado<UnidadOrganizacional>> ObtenerPaginadoAsync(Expression<Func<UnidadOrganizacional, bool>> predicate = null, Func<IQueryable<UnidadOrganizacional>, IOrderedQueryable<UnidadOrganizacional>> orderBy = null, Func<IQueryable<UnidadOrganizacional>, IIncludableQueryable<UnidadOrganizacional, object>> include = null, int index = 0, int size = 20, bool disableTracking = true, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<IPaginado<UnidadOrganizacional>> ObtenerPaginadoAsync(Consulta Query, Func<IQueryable<UnidadOrganizacional>, IIncludableQueryable<UnidadOrganizacional, object>> include = null, bool disableTracking = true, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<UnidadOrganizacional> UnicoAsync(Expression<Func<UnidadOrganizacional, bool>> predicado = null, Func<IQueryable<UnidadOrganizacional>, IOrderedQueryable<UnidadOrganizacional>> ordenarPor = null, Func<IQueryable<UnidadOrganizacional>, IIncludableQueryable<UnidadOrganizacional, object>> incluir = null, bool inhabilitarSegumiento = true)
        {
            throw new NotImplementedException();
        }

        public Task Restaurar(string[] ids)
        {
            throw new NotImplementedException();
        }
    }
}
