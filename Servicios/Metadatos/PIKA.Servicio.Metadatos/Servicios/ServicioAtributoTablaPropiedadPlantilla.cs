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
   public class ServicioAtributoTablaPropiedadPlantilla : IServicioInyectable, IServicioAtributoTablaPropiedadPlantilla
    {
        private const string DEFAULT_SORT_COL = "Nombre";
        private const string DEFAULT_SORT_DIRECTION = "asc";

        private IServicioCache cache;
        private IRepositorioAsync<AtributoTablaPropiedadPlantilla> repo;
        private ICompositorConsulta<AtributoTablaPropiedadPlantilla> compositor;
        private ILogger<ServicioAtributoTablaPropiedadPlantilla> logger;
        private DbContextMetadatos contexto;
        private UnidadDeTrabajo<DbContextMetadatos> UDT;
        public ServicioAtributoTablaPropiedadPlantilla(DbContextMetadatos contexto,
            ICompositorConsulta<AtributoTablaPropiedadPlantilla> compositorConsulta,
            ILogger<ServicioAtributoTablaPropiedadPlantilla> Logger,
            IServicioCache servicioCache)
        {


            this.contexto = contexto;
            this.UDT = new UnidadDeTrabajo<DbContextMetadatos>(contexto);
            this.cache = servicioCache;
            this.logger = Logger;
            this.compositor = compositorConsulta;
            this.repo = UDT.ObtenerRepositoryAsync<AtributoTablaPropiedadPlantilla>(compositor);
        }
                      
        public async Task<bool> Existe(Expression<Func<AtributoTablaPropiedadPlantilla, bool>> predicado)
        {
            List<AtributoTablaPropiedadPlantilla> l = await this.repo.ObtenerAsync(predicado);
            if (l.Count() == 0) return false;
            return true;
        }

        public async Task<AtributoTablaPropiedadPlantilla> CrearAsync(AtributoTablaPropiedadPlantilla entity, CancellationToken cancellationToken = default)
        {

            if (await Existe(x => x.PropiedadPlantillaid.Equals(entity.PropiedadPlantillaid, StringComparison.InvariantCultureIgnoreCase)))
            {
                throw new ExElementoExistente(entity.PropiedadPlantillaid);
            }

            entity.Atributotablaid = System.Guid.NewGuid().ToString();
            await this.repo.CrearAsync(entity);
            UDT.SaveChanges();
            return entity;
        }

        public async Task ActualizarAsync(AtributoTablaPropiedadPlantilla entity)
        {

            AtributoTablaPropiedadPlantilla o = await this.repo.UnicoAsync(x => x.Atributotablaid == entity.Atributotablaid);

            if (o == null)
            {
                throw new EXNoEncontrado(entity.Atributotablaid);
            }

            if (await Existe(x =>
            x.Atributotablaid != entity.Atributotablaid & x.PropiedadPlantillaid == entity.PropiedadPlantillaid && x.Atributotablaid == entity.Atributotablaid))
            {
                throw new ExElementoExistente(entity.PropiedadPlantillaid);
            }

            o.Atributotablaid = entity.Atributotablaid;
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
        public async Task<IPaginado<AtributoTablaPropiedadPlantilla>> ObtenerPaginadoAsync(Consulta Query, Func<IQueryable<AtributoTablaPropiedadPlantilla>, IIncludableQueryable<AtributoTablaPropiedadPlantilla, object>> include = null, bool disableTracking = true, CancellationToken cancellationToken = default)
        {
            Query = GetDefaultQuery(Query);
            var respuesta = await this.repo.ObtenerPaginadoAsync(Query, null);

            return respuesta;
        }

        public Task<IEnumerable<AtributoTablaPropiedadPlantilla>> CrearAsync(params AtributoTablaPropiedadPlantilla[] entities)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<AtributoTablaPropiedadPlantilla>> CrearAsync(IEnumerable<AtributoTablaPropiedadPlantilla> entities, CancellationToken cancellationToken = default)
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

        public Task<List<AtributoTablaPropiedadPlantilla>> ObtenerAsync(Expression<Func<AtributoTablaPropiedadPlantilla, bool>> predicado)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<AtributoTablaPropiedadPlantilla>> ObtenerListaAsync(string SqlCommand)
        {
            throw new NotImplementedException();
        }

        public Task<IPaginado<AtributoTablaPropiedadPlantilla>> ObtenerPaginadoAsync(Expression<Func<AtributoTablaPropiedadPlantilla, bool>> predicate = null, Func<IQueryable<AtributoTablaPropiedadPlantilla>, IOrderedQueryable<AtributoTablaPropiedadPlantilla>> orderBy = null, Func<IQueryable<AtributoTablaPropiedadPlantilla>, IIncludableQueryable<AtributoTablaPropiedadPlantilla, object>> include = null, int index = 0, int size = 20, bool disableTracking = true, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }



        public Task Restaurar(string[] ids)
        {
            throw new NotImplementedException();
        }

        public Task<AtributoTablaPropiedadPlantilla> UnicoAsync(Expression<Func<AtributoTablaPropiedadPlantilla, bool>> predicado = null, Func<IQueryable<AtributoTablaPropiedadPlantilla>, IOrderedQueryable<AtributoTablaPropiedadPlantilla>> ordenarPor = null, Func<IQueryable<AtributoTablaPropiedadPlantilla>, IIncludableQueryable<AtributoTablaPropiedadPlantilla, object>> incluir = null, bool inhabilitarSegumiento = true)
        {
            throw new NotImplementedException();
        }

   
    }
}
