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
    public class ServicioTraduccionAplicacionModulo : IServicioInyectable, IServicioTraduccionAplicacionModulo
    {
        private const string DEFAULT_SORT_COL = "Nombre";
        private const string DEFAULT_SORT_DIRECTION = "asc";

        private IServicioCache cache;
        private IRepositorioAsync<TraduccionAplicacionModulo> repo;
        private ICompositorConsulta<TraduccionAplicacionModulo> compositor;
        private ILogger<ServicioTraduccionAplicacionModulo> logger;
        private DbContextSeguridad contexto;
        private UnidadDeTrabajo<DbContextSeguridad> UDT;
        public ServicioTraduccionAplicacionModulo(DbContextSeguridad contexto,
            ICompositorConsulta<TraduccionAplicacionModulo> compositorConsulta,
            ILogger<ServicioTraduccionAplicacionModulo> Logger,
            IServicioCache servicioCache)
        {


            this.contexto = contexto;
            this.UDT = new UnidadDeTrabajo<DbContextSeguridad>(contexto);
            this.cache = servicioCache;
            this.logger = Logger;
            this.compositor = compositorConsulta;
            this.repo = UDT.ObtenerRepositoryAsync<TraduccionAplicacionModulo>(compositor);
        }






        public async Task<bool> Existe(Expression<Func<TraduccionAplicacionModulo, bool>> predicado)
        {
            List<TraduccionAplicacionModulo> l = await this.repo.ObtenerAsync(predicado);
            if (l.Count() == 0) return false;
            return true;
        }


        public async Task<TraduccionAplicacionModulo> CrearAsync(TraduccionAplicacionModulo entity, CancellationToken cancellationToken = default)
        {

            if (await Existe(x => x.Nombre.Equals(entity.Nombre, StringComparison.InvariantCultureIgnoreCase)))
            {
                throw new ExElementoExistente(entity.Nombre);
            }

            entity.ModuloId = System.Guid.NewGuid().ToString();
            await this.repo.CrearAsync(entity);
            UDT.SaveChanges();
            return entity;
        }

        public async Task ActualizarAsync(TraduccionAplicacionModulo entity)
        {

            TraduccionAplicacionModulo o = await this.repo.UnicoAsync(x => x.ModuloId == entity.ModuloId);

            if (o == null)
            {
                throw new EXNoEncontrado(entity.ModuloId);
            }

            if (await Existe(x =>
            x.ModuloId != entity.ModuloId
            && x.Nombre.Equals(entity.Nombre, StringComparison.InvariantCultureIgnoreCase)))
            {
                throw new ExElementoExistente(entity.Nombre);
            }

            o.Nombre = entity.Nombre;
            o.Descripcion = entity.Descripcion;
            o.UICulture = entity.UICulture;
          
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
        public async Task<IPaginado<TraduccionAplicacionModulo>> ObtenerPaginadoAsync(Consulta Query, Func<IQueryable<TraduccionAplicacionModulo>, IIncludableQueryable<TraduccionAplicacionModulo, object>> include = null, bool disableTracking = true, CancellationToken cancellationToken = default)
        {
            Query = GetDefaultQuery(Query);
            var respuesta = await this.repo.ObtenerPaginadoAsync(Query, null);

            return respuesta;
        }

        public Task<IEnumerable<TraduccionAplicacionModulo>> CrearAsync(params TraduccionAplicacionModulo[] entities)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<TraduccionAplicacionModulo>> CrearAsync(IEnumerable<TraduccionAplicacionModulo> entities, CancellationToken cancellationToken = default)
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

        public Task<List<TraduccionAplicacionModulo>> ObtenerAsync(Expression<Func<TraduccionAplicacionModulo, bool>> predicado)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<TraduccionAplicacionModulo>> ObtenerListaAsync(string SqlCommand)
        {
            throw new NotImplementedException();
        }

        public Task<IPaginado<TraduccionAplicacionModulo>> ObtenerPaginadoAsync(Expression<Func<TraduccionAplicacionModulo, bool>> predicate = null, Func<IQueryable<TraduccionAplicacionModulo>, IOrderedQueryable<TraduccionAplicacionModulo>> orderBy = null, Func<IQueryable<TraduccionAplicacionModulo>, IIncludableQueryable<TraduccionAplicacionModulo, object>> include = null, int index = 0, int size = 20, bool disableTracking = true, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }



        public Task Restaurar(string[] ids)
        {
            throw new NotImplementedException();
        }

        public Task<TraduccionAplicacionModulo> UnicoAsync(Expression<Func<TraduccionAplicacionModulo, bool>> predicado = null, Func<IQueryable<TraduccionAplicacionModulo>, IOrderedQueryable<TraduccionAplicacionModulo>> ordenarPor = null, Func<IQueryable<TraduccionAplicacionModulo>, IIncludableQueryable<TraduccionAplicacionModulo, object>> incluir = null, bool inhabilitarSegumiento = true)
        {
            throw new NotImplementedException();
        }
    }
}
