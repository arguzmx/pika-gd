using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.Extensions.Logging;
using PIKA.Infraestructura.Comun;
using PIKA.Infraestructura.Comun.Excepciones;
using PIKA.Infraestructura.Comun.Interfaces;
using PIKA.Modelo.Metadatos;
using PIKA.Servicio.Metadatos.Data;
using PIKA.Servicio.Metadatos.Interfaces;
using RepositorioEntidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PIKA.Servicio.Metadatos.Servicios
{
   public class ServicioTipoDato : ContextoServicioMetadatos, IServicioInyectable, IServicioTipoDato
    {
        private const string DEFAULT_SORT_COL = "Nombre";
        private const string DEFAULT_SORT_DIRECTION = "asc";

        private IServicioCache cache;
        private IRepositorioAsync<TipoDato> repo;
        private ICompositorConsulta<TipoDato> compositor;
        private ILogger<ServicioTipoDato> logger;
        private DbContextMetadatos contexto;
        private UnidadDeTrabajo<DbContextMetadatos> UDT;
        public ServicioTipoDato(
          IProveedorOpcionesContexto<DbContextMetadatos> proveedorOpciones,
          ICompositorConsulta<TipoDato> compositorConsulta,
          ILogger<ServicioTipoDato> Logger,
          IServicioCache servicioCache) : base(proveedorOpciones, Logger, servicioCache)
        {
            this.UDT = new UnidadDeTrabajo<DbContextMetadatos>(contexto);
            this.compositor = compositorConsulta;
            this.repo = UDT.ObtenerRepositoryAsync<TipoDato>(compositor);
        }

        public async Task<bool> Existe(Expression<Func<TipoDato, bool>> predicado)
        {
            List<TipoDato> l = await this.repo.ObtenerAsync(predicado);
            if (l.Count() == 0) return false;
            return true;
        }


        public async Task<TipoDato> CrearAsync(TipoDato entity, CancellationToken cancellationToken = default)
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

        public async Task ActualizarAsync(TipoDato entity)
        {

            TipoDato o = await this.repo.UnicoAsync(x => x.Id == entity.Id);

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
        public async Task<IPaginado<TipoDato>> ObtenerPaginadoAsync(Consulta Query, Func<IQueryable<TipoDato>, IIncludableQueryable<TipoDato, object>> include = null, bool disableTracking = true, CancellationToken cancellationToken = default)
        {
            Query = GetDefaultQuery(Query);
            var respuesta = await this.repo.ObtenerPaginadoAsync(Query, null);

            return respuesta;
        }

        public Task<IEnumerable<TipoDato>> CrearAsync(params TipoDato[] entities)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<TipoDato>> CrearAsync(IEnumerable<TipoDato> entities, CancellationToken cancellationToken = default)
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

        public Task<List<TipoDato>> ObtenerAsync(Expression<Func<TipoDato, bool>> predicado)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<TipoDato>> ObtenerListaAsync(string SqlCommand)
        {
            throw new NotImplementedException();
        }

        public Task<IPaginado<TipoDato>> ObtenerPaginadoAsync(Expression<Func<TipoDato, bool>> predicate = null, Func<IQueryable<TipoDato>, IOrderedQueryable<TipoDato>> orderBy = null, Func<IQueryable<TipoDato>, IIncludableQueryable<TipoDato, object>> include = null, int index = 0, int size = 20, bool disableTracking = true, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }



        public Task Restaurar(string[] ids)
        {
            throw new NotImplementedException();
        }

        public Task<TipoDato> UnicoAsync(Expression<Func<TipoDato, bool>> predicado = null, Func<IQueryable<TipoDato>, IOrderedQueryable<TipoDato>> ordenarPor = null, Func<IQueryable<TipoDato>, IIncludableQueryable<TipoDato, object>> incluir = null, bool inhabilitarSegumiento = true)
        {
            throw new NotImplementedException();
        }
    }
}
