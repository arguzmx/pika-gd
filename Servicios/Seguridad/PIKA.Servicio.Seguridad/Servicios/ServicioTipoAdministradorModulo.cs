using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.Extensions.Logging;
using PIKA.Infraestructura.Comun;
using PIKA.Infraestructura.Comun.Excepciones;
using PIKA.Infraestructura.Comun.Interfaces;
using PIKA.Servicio.Seguridad.Interfaces;
using RepositorioEntidades;

namespace PIKA.Servicio.Seguridad.Servicios
{
    public class ServicioTipoAdministradorModulo : ContextoServicioSeguridad, IServicioInyectable, IServicioTipoAdministradorModulo
    {
        private const string DEFAULT_SORT_COL = "Nombre";
        private const string DEFAULT_SORT_DIRECTION = "asc";

        private IRepositorioAsync<TipoAdministradorModulo> repo;
        private ICompositorConsulta<TipoAdministradorModulo> compositor;
        private UnidadDeTrabajo<DbContextSeguridad> UDT;
        public ServicioTipoAdministradorModulo(
         IProveedorOpcionesContexto<DbContextSeguridad> proveedorOpciones,
         ICompositorConsulta<TipoAdministradorModulo> compositorConsulta,
         ILogger<ServicioAplicacion> Logger,
         IServicioCache servicioCache) : base(proveedorOpciones, Logger, servicioCache)
        {
            this.UDT = new UnidadDeTrabajo<DbContextSeguridad>(contexto);
            this.compositor = compositorConsulta;
            this.repo = UDT.ObtenerRepositoryAsync<TipoAdministradorModulo>(compositor);
        }

        public async Task<bool> Existe(Expression<Func<TipoAdministradorModulo, bool>> predicado)
        {
            List<TipoAdministradorModulo> l = await this.repo.ObtenerAsync(predicado);
            if (l.Count() == 0) return false;
            return true;
        }


        public async Task<TipoAdministradorModulo> CrearAsync(TipoAdministradorModulo entity, CancellationToken cancellationToken = default)
        {

            if (await Existe(x => x.AplicacionId.Equals(entity.AplicacionId, StringComparison.InvariantCultureIgnoreCase)))
            {
                throw new ExElementoExistente(entity.AplicacionId);
            }

            entity.ModuloId = System.Guid.NewGuid().ToString();
            await this.repo.CrearAsync(entity);
            UDT.SaveChanges();
            return entity;
        }

        public async Task ActualizarAsync(TipoAdministradorModulo entity)
        {

            TipoAdministradorModulo o = await this.repo.UnicoAsync(x => x.ModuloId == entity.ModuloId);

            if (o == null)
            {
                throw new EXNoEncontrado(entity.ModuloId);
            }

            if (await Existe(x =>
            x.ModuloId != entity.ModuloId
            && x.AplicacionId.Equals(entity.AplicacionId, StringComparison.InvariantCultureIgnoreCase)))
            {
                throw new ExElementoExistente(entity.AplicacionId);
            }

            o.AplicacionId = entity.AplicacionId;
            o.ModuloId = entity.ModuloId;
            o.TiposAdministrados = entity.TiposAdministrados;
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
        public async Task<IPaginado<TipoAdministradorModulo>> ObtenerPaginadoAsync(Consulta Query, Func<IQueryable<TipoAdministradorModulo>, IIncludableQueryable<TipoAdministradorModulo, object>> include = null, bool disableTracking = true, CancellationToken cancellationToken = default)
        {
            Query = GetDefaultQuery(Query);
            var respuesta = await this.repo.ObtenerPaginadoAsync(Query, null);

            return respuesta;
        }

        public Task<IEnumerable<TipoAdministradorModulo>> CrearAsync(params TipoAdministradorModulo[] entities)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<TipoAdministradorModulo>> CrearAsync(IEnumerable<TipoAdministradorModulo> entities, CancellationToken cancellationToken = default)
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

        public Task<ICollection<string>> Eliminar(string[] ids)
        {
            throw new NotImplementedException();
        }

        public Task<List<TipoAdministradorModulo>> ObtenerAsync(Expression<Func<TipoAdministradorModulo, bool>> predicado)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<TipoAdministradorModulo>> ObtenerListaAsync(string SqlCommand)
        {
            throw new NotImplementedException();
        }

        public Task<IPaginado<TipoAdministradorModulo>> ObtenerPaginadoAsync(Expression<Func<TipoAdministradorModulo, bool>> predicate = null, Func<IQueryable<TipoAdministradorModulo>, IOrderedQueryable<TipoAdministradorModulo>> orderBy = null, Func<IQueryable<TipoAdministradorModulo>, IIncludableQueryable<TipoAdministradorModulo, object>> include = null, int index = 0, int size = 20, bool disableTracking = true, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }



        public Task Restaurar(string[] ids)
        {
            throw new NotImplementedException();
        }

        public Task<TipoAdministradorModulo> UnicoAsync(Expression<Func<TipoAdministradorModulo, bool>> predicado = null, Func<IQueryable<TipoAdministradorModulo>, IOrderedQueryable<TipoAdministradorModulo>> ordenarPor = null, Func<IQueryable<TipoAdministradorModulo>, IIncludableQueryable<TipoAdministradorModulo, object>> incluir = null, bool inhabilitarSegumiento = true)
        {
            throw new NotImplementedException();
        }

       
    }

}
