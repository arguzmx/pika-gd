using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.Extensions.Logging;
using PIKA.Infraestructura.Comun;
using PIKA.Infraestructura.Comun.Excepciones;
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
        private const string DEFAULT_SORT_COL = "Nombre";
        private const string DEFAULT_SORT_DIRECTION = "asc";

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


     



        public async Task<bool> Existe(Expression<Func<Dominio, bool>> predicado)
        {
            List<Dominio> l = await this.repo.ObtenerAsync(predicado);
            if (l.Count() == 0) return false;
            return true;
        }


        public async Task<Dominio> CrearAsync(Dominio entity, CancellationToken cancellationToken = default)
        {

            if( await  Existe(x=>x.Nombre.Equals(entity.Nombre, StringComparison.InvariantCultureIgnoreCase)))
            {
                throw new ExElementoExistente(entity.Nombre);
            }

            entity.Id = System.Guid.NewGuid().ToString();
            await this.repo.CrearAsync(entity);
            UDT.SaveChanges();
            return entity;
        }

        public async Task ActualizarAsync(Dominio entity)
        {
            
            Dominio o = await this.repo.UnicoAsync(x => x.Id == entity.Id);

            if (o == null)
            {
                throw new EXNoEncontrado(entity.Id);
            }
            
            if (await Existe(x => 
            x.Id!=entity.Id & x.TipoOrigenId ==entity.TipoOrigenId && x.OrigenId==entity.OrigenId
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
                query.columna_ordenamiento = string.IsNullOrEmpty(query.columna_ordenamiento) ? DEFAULT_SORT_COL : query.columna_ordenamiento;
                query.direccion_ordenamiento = string.IsNullOrEmpty(query.direccion_ordenamiento) ? DEFAULT_SORT_DIRECTION : query.direccion_ordenamiento;
            }
            else
            {
                query = new Consulta() { indice = 0, tamano = 20, columna_ordenamiento = DEFAULT_SORT_COL, direccion_ordenamiento = DEFAULT_SORT_DIRECTION };
            }
            return query;
        }
        public async Task<IPaginado<Dominio>> ObtenerPaginadoAsync(Consulta Query, Func<IQueryable<Dominio>, IIncludableQueryable<Dominio, object>> include = null, bool disableTracking = true, CancellationToken cancellationToken = default)
        {
            Query = GetDefaultQuery(Query);
            //Query.Filtros.Add(new FiltroConsulta() { Operador =  operado, Property = COL_OWNERID, Value = OwnerId });
            var respuesta = await this.repo.ObtenerPaginadoAsync(Query, null);

            return respuesta;
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
