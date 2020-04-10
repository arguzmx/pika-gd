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
    public class ServicioPlantilla : IServicioInyectable, IServicioPlantilla
    {
        private const string DEFAULT_SORT_COL = "Nombre";
        private const string DEFAULT_SORT_DIRECTION = "asc";

        private IServicioCache cache;
        private IRepositorioAsync<Plantilla> repo;
        private ICompositorConsulta<Plantilla> compositor;
        private ILogger<ServicioPlantilla> logger;
        private DbContextMetadatos contexto;
        private UnidadDeTrabajo<DbContextMetadatos> UDT;
        public ServicioPlantilla(DbContextMetadatos contexto,
            ICompositorConsulta<Plantilla> compositorConsulta,
            ILogger<ServicioPlantilla> Logger,
            IServicioCache servicioCache)
        {


            this.contexto = contexto;
            this.UDT = new UnidadDeTrabajo<DbContextMetadatos>(contexto);
            this.cache = servicioCache;
            this.logger = Logger;
            this.compositor = compositorConsulta;
            this.repo = UDT.ObtenerRepositoryAsync<Plantilla>(compositor);
        }






        public async Task<bool> Existe(Expression<Func<Plantilla, bool>> predicado)
        {
            List<Plantilla> l = await this.repo.ObtenerAsync(predicado);
            if (l.Count() == 0) return false;
            return true;
        }


        public async Task<Plantilla> CrearAsync(Plantilla entity, CancellationToken cancellationToken = default)
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

        public async Task ActualizarAsync(Plantilla entity)
        {

            Plantilla o = await this.repo.UnicoAsync(x => x.Id == entity.Id);

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
            o.Eliminada = entity.Eliminada;
            o.OrigenId = entity.OrigenId;
            o.Propiedades = entity.Propiedades;
            o.TipoOrigenId= entity.TipoOrigenId;
           

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
        public async Task<IPaginado<Plantilla>> ObtenerPaginadoAsync(Consulta Query, Func<IQueryable<Plantilla>, IIncludableQueryable<Plantilla, object>> include = null, bool disableTracking = true, CancellationToken cancellationToken = default)
        {
            Query = GetDefaultQuery(Query);
            var respuesta = await this.repo.ObtenerPaginadoAsync(Query, null);

            return respuesta;
        }

        public Task<IEnumerable<Plantilla>> CrearAsync(params Plantilla[] entities)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Plantilla>> CrearAsync(IEnumerable<Plantilla> entities, CancellationToken cancellationToken = default)
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

        public Task<List<Plantilla>> ObtenerAsync(Expression<Func<Plantilla, bool>> predicado)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Plantilla>> ObtenerListaAsync(string SqlCommand)
        {
            throw new NotImplementedException();
        }

        public Task<IPaginado<Plantilla>> ObtenerPaginadoAsync(Expression<Func<Plantilla, bool>> predicate = null, Func<IQueryable<Plantilla>, IOrderedQueryable<Plantilla>> orderBy = null, Func<IQueryable<Plantilla>, IIncludableQueryable<Plantilla, object>> include = null, int index = 0, int size = 20, bool disableTracking = true, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }



        public Task Restaurar(string[] ids)
        {
            throw new NotImplementedException();
        }

        public Task<Plantilla> UnicoAsync(Expression<Func<Plantilla, bool>> predicado = null, Func<IQueryable<Plantilla>, IOrderedQueryable<Plantilla>> ordenarPor = null, Func<IQueryable<Plantilla>, IIncludableQueryable<Plantilla, object>> incluir = null, bool inhabilitarSegumiento = true)
        {
            throw new NotImplementedException();
        }
    }
}
