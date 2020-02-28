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


namespace PIKA.Servicio.Organizacion.Servicios
{
    public class ServicioDominio : IServicioInyectable, IServicioDominio
    {

        private IServicioCache cache;
        private IRepositorioAsync<Dominio> repo;
        private ICompositorConsulta<Dominio> compositor;
        private ILogger<ServicioDominio> logger;
        private DbContextOrganizacion contexto;
        private UnidadDeTrabajo<DbContextOrganizacion> UDT;
        public ServicioDominio(DbContextOrganizacion contexto,
            ICompositorConsulta<Dominio> compositorConsulta,
            ILogger<ServicioDominio> Logger,
            IServicioCache servicioCache)
        {


            this.contexto = contexto;
            this.UDT = new UnidadDeTrabajo<DbContextOrganizacion>(contexto);
            this.cache = servicioCache;
            this.logger = Logger;
            this.compositor = compositorConsulta;
            this.repo = UDT.ObtenerRepositoryAsync<Dominio>(compositor);
        }


        public Task ActualizarAsync(Dominio entity)
        {

            throw new NotImplementedException();
        }

        public Task<Dominio> CrearAsync(Dominio entity, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Dominio>> CrearAsync(params Dominio[] entities)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Dominio>> CrearAsync(IEnumerable<Dominio> entities, CancellationToken cancellationToken = default)
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

        public Task<List<Dominio>> ObtenerAsync(Expression<Func<Dominio, bool>> predicado)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Dominio>> ObtenerListaAsync(string SqlCommand)
        {
            throw new NotImplementedException();
        }

        public Task<IPaginado<Dominio>> ObtenerPaginadoAsync(Expression<Func<Dominio, bool>> predicate = null, Func<IQueryable<Dominio>, IOrderedQueryable<Dominio>> orderBy = null, Func<IQueryable<Dominio>, IIncludableQueryable<Dominio, object>> include = null, int index = 0, int size = 20, bool disableTracking = true, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<IPaginado<Dominio>> ObtenerPaginadoAsync(Consulta Query, Func<IQueryable<Dominio>, IIncludableQueryable<Dominio, object>> include = null, bool disableTracking = true, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task Restaurar(string[] ids)
        {
            throw new NotImplementedException();
        }

        public Task<Dominio> UnicoAsync(Expression<Func<Dominio, bool>> predicado = null, Func<IQueryable<Dominio>, IOrderedQueryable<Dominio>> ordenarPor = null, Func<IQueryable<Dominio>, IIncludableQueryable<Dominio, object>> incluir = null, bool inhabilitarSegumiento = true)
        {
            throw new NotImplementedException();
        }
    }
}
