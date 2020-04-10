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
   public class ServicioValidadorNumero: IServicioInyectable, IServicioValidadorNumero
    {
        private const string DEFAULT_SORT_COL = "Nombre";
        private const string DEFAULT_SORT_DIRECTION = "asc";

        private IServicioCache cache;
        private IRepositorioAsync<ValidadorNumero> repo;
        private ICompositorConsulta<ValidadorNumero> compositor;
        private ILogger<ServicioValidadorNumero> logger;
        private DbContextMetadatos contexto;
        private UnidadDeTrabajo<DbContextMetadatos> UDT;
        public ServicioValidadorNumero(DbContextMetadatos contexto,
            ICompositorConsulta<ValidadorNumero> compositorConsulta,
            ILogger<ServicioValidadorNumero> Logger,
            IServicioCache servicioCache)
        {


            this.contexto = contexto;
            this.UDT = new UnidadDeTrabajo<DbContextMetadatos>(contexto);
            this.cache = servicioCache;
            this.logger = Logger;
            this.compositor = compositorConsulta;
            this.repo = UDT.ObtenerRepositoryAsync<ValidadorNumero>(compositor);
        }






        public async Task<bool> Existe(Expression<Func<ValidadorNumero, bool>> predicado)
        {
            List<ValidadorNumero> l = await this.repo.ObtenerAsync(predicado);
            if (l.Count() == 0) return false;
            return true;
        }


        public async Task<ValidadorNumero> CrearAsync(ValidadorNumero entity, CancellationToken cancellationToken = default)
        {

            if (await Existe(x => x.Id.Equals(entity.Id, StringComparison.InvariantCultureIgnoreCase)))
            {
                throw new ExElementoExistente(entity.Id);
            }

            entity.Id = System.Guid.NewGuid().ToString();
            await this.repo.CrearAsync(entity);
            UDT.SaveChanges();
            return entity;
        }

        public async Task ActualizarAsync(ValidadorNumero entity)
        {

            ValidadorNumero o = await this.repo.UnicoAsync(x => x.Id == entity.Id);

            if (o == null)
            {
                throw new EXNoEncontrado(entity.Id);
            }

            if (await Existe(x =>
            x.Id != entity.Id
            && x.Id.Equals(entity.Id, StringComparison.InvariantCultureIgnoreCase)))
            {
                throw new ExElementoExistente(entity.Id);
            }

            o.PropiedadId = entity.PropiedadId;
            o.max = entity.max;
            o.min = entity.min;
            o.valordefault = entity.valordefault;
            o.Propiedad = entity.Propiedad;


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
        public async Task<IPaginado<ValidadorNumero>> ObtenerPaginadoAsync(Consulta Query, Func<IQueryable<ValidadorNumero>, IIncludableQueryable<ValidadorNumero, object>> include = null, bool disableTracking = true, CancellationToken cancellationToken = default)
        {
            Query = GetDefaultQuery(Query);
            var respuesta = await this.repo.ObtenerPaginadoAsync(Query, null);

            return respuesta;
        }

        public Task<IEnumerable<ValidadorNumero>> CrearAsync(params ValidadorNumero[] entities)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<ValidadorNumero>> CrearAsync(IEnumerable<ValidadorNumero> entities, CancellationToken cancellationToken = default)
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

        public Task<List<ValidadorNumero>> ObtenerAsync(Expression<Func<ValidadorNumero, bool>> predicado)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<ValidadorNumero>> ObtenerListaAsync(string SqlCommand)
        {
            throw new NotImplementedException();
        }

        public Task<IPaginado<ValidadorNumero>> ObtenerPaginadoAsync(Expression<Func<ValidadorNumero, bool>> predicate = null, Func<IQueryable<ValidadorNumero>, IOrderedQueryable<ValidadorNumero>> orderBy = null, Func<IQueryable<ValidadorNumero>, IIncludableQueryable<ValidadorNumero, object>> include = null, int index = 0, int size = 20, bool disableTracking = true, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }



        public Task Restaurar(string[] ids)
        {
            throw new NotImplementedException();
        }

        public Task<ValidadorNumero> UnicoAsync(Expression<Func<ValidadorNumero, bool>> predicado = null, Func<IQueryable<ValidadorNumero>, IOrderedQueryable<ValidadorNumero>> ordenarPor = null, Func<IQueryable<ValidadorNumero>, IIncludableQueryable<ValidadorNumero, object>> incluir = null, bool inhabilitarSegumiento = true)
        {
            throw new NotImplementedException();
        }

        Task<ValidadorNumero> IServicioRepositorioAsync<ValidadorNumero, string>.UnicoAsync(Expression<Func<ValidadorNumero, bool>> predicado, Func<IQueryable<ValidadorNumero>, IOrderedQueryable<ValidadorNumero>> ordenarPor, Func<IQueryable<ValidadorNumero>, IIncludableQueryable<ValidadorNumero, object>> incluir, bool inhabilitarSegumiento)
        {
            throw new NotImplementedException();
        }

        Task<List<ValidadorNumero>> IServicioRepositorioAsync<ValidadorNumero, string>.ObtenerAsync(Expression<Func<ValidadorNumero, bool>> predicado)
        {
            throw new NotImplementedException();
        }

        Task<IEnumerable<ValidadorNumero>> IServicioRepositorioAsync<ValidadorNumero, string>.ObtenerListaAsync(string SqlCommand)
        {
            throw new NotImplementedException();
        }

        Task<IPaginado<ValidadorNumero>> IServicioRepositorioAsync<ValidadorNumero, string>.ObtenerPaginadoAsync(Expression<Func<ValidadorNumero, bool>> predicate, Func<IQueryable<ValidadorNumero>, IOrderedQueryable<ValidadorNumero>> orderBy, Func<IQueryable<ValidadorNumero>, IIncludableQueryable<ValidadorNumero, object>> include, int index, int size, bool disableTracking, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        Task<IPaginado<ValidadorNumero>> IServicioRepositorioAsync<ValidadorNumero, string>.ObtenerPaginadoAsync(Consulta Query, Func<IQueryable<ValidadorNumero>, IIncludableQueryable<ValidadorNumero, object>> include, bool disableTracking, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        Task<ValidadorNumero> IServicioRepositorioAsync<ValidadorNumero, string>.CrearAsync(ValidadorNumero entity, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        Task<IEnumerable<ValidadorNumero>> IServicioRepositorioAsync<ValidadorNumero, string>.CrearAsync(params ValidadorNumero[] entities)
        {
            throw new NotImplementedException();
        }

        Task<IEnumerable<ValidadorNumero>> IServicioRepositorioAsync<ValidadorNumero, string>.CrearAsync(IEnumerable<ValidadorNumero> entities, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        Task IServicioRepositorioAsync<ValidadorNumero, string>.ActualizarAsync(ValidadorNumero entity)
        {
            throw new NotImplementedException();
        }

        Task IServicioRepositorioAsync<ValidadorNumero, string>.Eliminar(string[] ids)
        {
            throw new NotImplementedException();
        }

        Task IServicioRepositorioAsync<ValidadorNumero, string>.Restaurar(string[] ids)
        {
            throw new NotImplementedException();
        }

        Task IServicioRepositorioAsync<ValidadorNumero, string>.EjecutarSql(string sqlCommand)
        {
            throw new NotImplementedException();
        }

        Task IServicioRepositorioAsync<ValidadorNumero, string>.EjecutarSqlBatch(List<string> sqlCommand)
        {
            throw new NotImplementedException();
        }

        Task<bool> IServicioRepositorioAsync<ValidadorNumero, string>.Existe(Expression<Func<ValidadorNumero, bool>> predicado)
        {
            throw new NotImplementedException();
        }
    }
}
