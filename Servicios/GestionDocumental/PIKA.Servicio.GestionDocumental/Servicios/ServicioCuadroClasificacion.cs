using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.Extensions.Logging;
using PIKA.Infraestructura.Comun;
using PIKA.Infraestructura.Comun.Excepciones;
using PIKA.Infraestructura.Comun.Interfaces;
using PIKA.Modelo.GestorDocumental;
using PIKA.Servicio.GestionDocumental.Data;
using PIKA.Servicio.GestionDocumental.Interfaces;
using RepositorioEntidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PIKA.Servicio.GestionDocumental.Servicios
{
    public class ServicioCuadroClasificacion : IServicioInyectable, IServicioCuadroClasificacion
    {
        private const string DEFAULT_SORT_COL = "Nombre";
        private const string DEFAULT_SORT_DIRECTION = "asc";

        private IServicioCache cache;
        private IRepositorioAsync<CuadroClasificacion> repo;
        private ICompositorConsulta<CuadroClasificacion> compositor;
        private ILogger<ServicioCuadroClasificacion> logger;
        private DBContextGestionDocumental contexto;
        private UnidadDeTrabajo<DBContextGestionDocumental> UDT;

        public ServicioCuadroClasificacion(DBContextGestionDocumental contexto,
           ICompositorConsulta<CuadroClasificacion> compositorConsulta,
           ILogger<ServicioCuadroClasificacion> Logger,
           IServicioCache servicioCache)
        {


            this.contexto = contexto;
            this.UDT = new UnidadDeTrabajo<DBContextGestionDocumental>(contexto);
            this.cache = servicioCache;
            this.logger = Logger;
            this.compositor = compositorConsulta;
            this.repo = UDT.ObtenerRepositoryAsync<CuadroClasificacion>(compositor);
        }

        public async Task<bool> Existe(Expression<Func<CuadroClasificacion, bool>> predicado)
        {
            List<CuadroClasificacion> l = await this.repo.ObtenerAsync(predicado);
            if (l.Count() == 0) return false;
            return true;
        }


        public async Task<CuadroClasificacion> CrearAsync(CuadroClasificacion entity, CancellationToken cancellationToken = default)
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

        public async Task ActualizarAsync(CuadroClasificacion entity)
        {

            CuadroClasificacion o = await this.repo.UnicoAsync(x => x.Id == entity.Id);

            if (o == null)
            {
                throw new EXNoEncontrado(entity.Id);
            }

            if (await Existe(x =>
            x.Id != entity.Id & x.TipoOrigenId == entity.TipoOrigenId && x.OrigenId == entity.OrigenId
            && x.Nombre.Equals(entity.Nombre, StringComparison.InvariantCultureIgnoreCase)))
            {
                throw new ExElementoExistente(entity.Nombre);
            }

            o.Nombre = entity.Nombre;
            o.Eliminada = entity.Eliminada;
            o.EstadoCuadroClasificacionId = entity.EstadoCuadroClasificacionId;

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
        public async Task<IPaginado<CuadroClasificacion>> ObtenerPaginadoAsync(Consulta Query, Func<IQueryable<CuadroClasificacion>, IIncludableQueryable<CuadroClasificacion, object>> include = null, bool disableTracking = true, CancellationToken cancellationToken = default)
        {
            Query = GetDefaultQuery(Query);
            var respuesta = await this.repo.ObtenerPaginadoAsync(Query, null);

            return respuesta;
        }

        public Task<IEnumerable<CuadroClasificacion>> CrearAsync(params CuadroClasificacion[] entities)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<CuadroClasificacion>> CrearAsync(IEnumerable<CuadroClasificacion> entities, CancellationToken cancellationToken = default)
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

        public Task<List<CuadroClasificacion>> ObtenerAsync(Expression<Func<CuadroClasificacion, bool>> predicado)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<CuadroClasificacion>> ObtenerListaAsync(string SqlCommand)
        {
            throw new NotImplementedException();
        }

        public Task<IPaginado<CuadroClasificacion>> ObtenerPaginadoAsync(Expression<Func<CuadroClasificacion, bool>> predicate = null, Func<IQueryable<CuadroClasificacion>, IOrderedQueryable<CuadroClasificacion>> orderBy = null, Func<IQueryable<CuadroClasificacion>, IIncludableQueryable<CuadroClasificacion, object>> include = null, int index = 0, int size = 20, bool disableTracking = true, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }



        public Task Restaurar(string[] ids)
        {
            throw new NotImplementedException();
        }

        public Task<CuadroClasificacion> UnicoAsync(Expression<Func<CuadroClasificacion, bool>> predicado = null, Func<IQueryable<CuadroClasificacion>, IOrderedQueryable<CuadroClasificacion>> ordenarPor = null, Func<IQueryable<CuadroClasificacion>, IIncludableQueryable<CuadroClasificacion, object>> incluir = null, bool inhabilitarSegumiento = true)
        {
            throw new NotImplementedException();
        }
    }
}
