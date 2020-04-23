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
using PIKA.Modelo.Metadatos;
using PIKA.Servicio.Metadatos.Data;
using PIKA.Servicio.Metadatos.Interfaces;
using RepositorioEntidades;

namespace PIKA.Servicio.Metadatos.Servicios
{
 public   class ServicioTipoAlmacenMetadatos : ContextoServicioMetadatos, IServicioInyectable, IServicioTipoAlmacenMetadatos
    {
        private const string DEFAULT_SORT_COL = "Nombre";
        private const string DEFAULT_SORT_DIRECTION = "asc";

        private IRepositorioAsync<TipoAlmacenMetadatos> repo;
        private ICompositorConsulta<TipoAlmacenMetadatos> compositor;
        private UnidadDeTrabajo<DbContextMetadatos> UDT;
        public ServicioTipoAlmacenMetadatos(
          IProveedorOpcionesContexto<DbContextMetadatos> proveedorOpciones,
          ICompositorConsulta<TipoAlmacenMetadatos> compositorConsulta,
          ILogger<ServicioTipoAlmacenMetadatos> Logger,
          IServicioCache servicioCache) : base(proveedorOpciones, Logger, servicioCache)
        {
            this.UDT = new UnidadDeTrabajo<DbContextMetadatos>(contexto);
            this.compositor = compositorConsulta;
            this.repo = UDT.ObtenerRepositoryAsync<TipoAlmacenMetadatos>(compositor);
        }
        public async Task<bool> Existe(Expression<Func<TipoAlmacenMetadatos, bool>> predicado)
        {
            List<TipoAlmacenMetadatos> l = await this.repo.ObtenerAsync(predicado);
            if (l.Count() == 0) return false;
            return true;
        }


        public async Task<TipoAlmacenMetadatos> CrearAsync(TipoAlmacenMetadatos entity, CancellationToken cancellationToken = default)
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

        public async Task ActualizarAsync(TipoAlmacenMetadatos entity)
        {

            TipoAlmacenMetadatos o = await this.repo.UnicoAsync(x => x.Id == entity.Id);

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
        public async Task<IPaginado<TipoAlmacenMetadatos>> ObtenerPaginadoAsync(Consulta Query, Func<IQueryable<TipoAlmacenMetadatos>, IIncludableQueryable<TipoAlmacenMetadatos, object>> include = null, bool disableTracking = true, CancellationToken cancellationToken = default)
        {
            Query = GetDefaultQuery(Query);
            var respuesta = await this.repo.ObtenerPaginadoAsync(Query, null);

            return respuesta;
        }

        public Task<IEnumerable<TipoAlmacenMetadatos>> CrearAsync(params TipoAlmacenMetadatos[] entities)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<TipoAlmacenMetadatos>> CrearAsync(IEnumerable<TipoAlmacenMetadatos> entities, CancellationToken cancellationToken = default)
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

        public Task<List<TipoAlmacenMetadatos>> ObtenerAsync(Expression<Func<TipoAlmacenMetadatos, bool>> predicado)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<TipoAlmacenMetadatos>> ObtenerListaAsync(string SqlCommand)
        {
            throw new NotImplementedException();
        }

        public Task<IPaginado<TipoAlmacenMetadatos>> ObtenerPaginadoAsync(Expression<Func<TipoAlmacenMetadatos, bool>> predicate = null, Func<IQueryable<TipoAlmacenMetadatos>, IOrderedQueryable<TipoAlmacenMetadatos>> orderBy = null, Func<IQueryable<TipoAlmacenMetadatos>, IIncludableQueryable<TipoAlmacenMetadatos, object>> include = null, int index = 0, int size = 20, bool disableTracking = true, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }



        public Task Restaurar(string[] ids)
        {
            throw new NotImplementedException();
        }

        public async Task<TipoAlmacenMetadatos> UnicoAsync(Expression<Func<TipoAlmacenMetadatos, bool>> predicado = null, Func<IQueryable<TipoAlmacenMetadatos>, IOrderedQueryable<TipoAlmacenMetadatos>> ordenarPor = null, Func<IQueryable<TipoAlmacenMetadatos>, IIncludableQueryable<TipoAlmacenMetadatos, object>> incluir = null, bool inhabilitarSegumiento = true)
        {
<<<<<<< Updated upstream

            TipoAlmacenMetadatos d = await this.repo.UnicoAsync(predicado);

            return d.CopiaTipoAlmacenMetadatos();
        }
=======
>>>>>>> Stashed changes

            TipoAlmacenMetadatos d = await this.repo.UnicoAsync(predicado);

            return d.CopiaTipoAlmacenMetadatos();
        }

        Task<ICollection<string>> IServicioRepositorioAsync<TipoAlmacenMetadatos, string>.Eliminar(string[] ids)
        {
            throw new NotImplementedException();
        }
    }
}
