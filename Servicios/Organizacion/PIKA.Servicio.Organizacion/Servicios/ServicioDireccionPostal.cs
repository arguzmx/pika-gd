using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.Extensions.Logging;
using PIKA.Infraestructura.Comun;
using PIKA.Infraestructura.Comun.Excepciones;
using PIKA.Infraestructura.Comun.Interfaces;
using PIKA.Modelo.Organizacion;
using PIKA.Servicio.Organizacion.Interfaces;
using RepositorioEntidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PIKA.Servicio.Organizacion.Servicios
{
    public class ServicioDireccionPostal : IServicioInyectable, IServicioDireccionPostal
    {
        private const string DEFAULT_SORT_COL = "Nombre";
        private const string DEFAULT_SORT_DIRECTION = "asc";

        private IServicioCache cache;
        private IRepositorioAsync<DireccionPostal> repo;
        private ICompositorConsulta<DireccionPostal> compositor;
        private ILogger<ServicioDireccionPostal> logger;
        private DbContextOrganizacion contexto;
        private UnidadDeTrabajo<DbContextOrganizacion> UDT;
        public ServicioDireccionPostal(DbContextOrganizacion contexto,
            ICompositorConsulta<DireccionPostal> compositorConsulta,
            ILogger<ServicioDireccionPostal> Logger,
            IServicioCache servicioCache)
        {
            this.contexto = contexto;
            this.UDT = new UnidadDeTrabajo<DbContextOrganizacion>(contexto);
            this.cache = servicioCache;
            this.logger = Logger;
            this.compositor = compositorConsulta;
            this.repo = UDT.ObtenerRepositoryAsync<DireccionPostal>(compositor);
        }

        public async Task<bool> Existe(Expression<Func<DireccionPostal, bool>> predicado)
        {
            List<DireccionPostal> l = await this.repo.ObtenerAsync(predicado);
            if (l.Count() == 0) return false;
            return true;
        }

        public async Task<DireccionPostal> CrearAsync(DireccionPostal entity, CancellationToken cancellationToken = default)
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

        public async Task ActualizarAsync(DireccionPostal entity)
        {

            DireccionPostal o = await this.repo.UnicoAsync(x => x.Id == entity.Id);

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
            o.Calle = entity.Calle;
            o.NoInterno = entity.NoInterno;
            o.NoExterno = entity.NoExterno;
            o.Colonia = entity.Colonia;
            o.CP = entity.CP;
            o.Municipio = entity.Municipio;
            o.EstadoId = entity.EstadoId;
            o.PaisId = entity.PaisId;

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
                query = new Consulta() { indice = 0, tamano = 20, ord_columna= DEFAULT_SORT_COL, ord_direccion = DEFAULT_SORT_DIRECTION };
            }
            return query;
        }
        public async Task<IPaginado<DireccionPostal>> ObtenerPaginadoAsync(Consulta Query, Func<IQueryable<DireccionPostal>, IIncludableQueryable<DireccionPostal, object>> include = null, bool disableTracking = true, CancellationToken cancellationToken = default)
        {
            Query = GetDefaultQuery(Query);
            //Query.Filtros.Add(new FiltroConsulta() { Operador =  operado, Property = COL_OWNERID, Value = OwnerId });
            var respuesta = await this.repo.ObtenerPaginadoAsync(Query, null);

            return respuesta;
        }

        public Task<IEnumerable<DireccionPostal>> CrearAsync(params DireccionPostal[] entities)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<DireccionPostal>> CrearAsync(IEnumerable<DireccionPostal> entities, CancellationToken cancellationToken = default)
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

        public Task<List<DireccionPostal>> ObtenerAsync(Expression<Func<DireccionPostal, bool>> predicado)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<DireccionPostal>> ObtenerListaAsync(string SqlCommand)
        {
            throw new NotImplementedException();
        }

        public Task<IPaginado<DireccionPostal>> ObtenerPaginadoAsync(Expression<Func<DireccionPostal, bool>> predicate = null, Func<IQueryable<DireccionPostal>, IOrderedQueryable<DireccionPostal>> orderBy = null, Func<IQueryable<DireccionPostal>, IIncludableQueryable<DireccionPostal, object>> include = null, int index = 0, int size = 20, bool disableTracking = true, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }



        public Task Restaurar(string[] ids)
        {
            throw new NotImplementedException();
        }

        public Task<DireccionPostal> UnicoAsync(Expression<Func<DireccionPostal, bool>> predicado = null, Func<IQueryable<DireccionPostal>, IOrderedQueryable<DireccionPostal>> ordenarPor = null, Func<IQueryable<DireccionPostal>, IIncludableQueryable<DireccionPostal, object>> incluir = null, bool inhabilitarSegumiento = true)
        {
            throw new NotImplementedException();
        }
    }
}
