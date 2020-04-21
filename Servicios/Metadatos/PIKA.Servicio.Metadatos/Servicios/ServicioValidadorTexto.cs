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
    public class ServicioValidadorTexto : IServicioInyectable, IServicioValidadorTexto
    {
        private const string DEFAULT_SORT_COL = "Nombre";
        private const string DEFAULT_SORT_DIRECTION = "asc";

        private IServicioCache cache;
        private IRepositorioAsync<ValidadorTexto> repo;
        private ICompositorConsulta<ValidadorTexto> compositor;
        private ILogger<ServicioValidadorTexto> logger;
        private DbContextMetadatos contexto;
        private UnidadDeTrabajo<DbContextMetadatos> UDT;
        public ServicioValidadorTexto(DbContextMetadatos contexto,
            ICompositorConsulta<ValidadorTexto> compositorConsulta,
            ILogger<ServicioValidadorTexto> Logger,
            IServicioCache servicioCache)
        {


            this.contexto = contexto;
            this.UDT = new UnidadDeTrabajo<DbContextMetadatos>(contexto);
            this.cache = servicioCache;
            this.logger = Logger;
            this.compositor = compositorConsulta;
            this.repo = UDT.ObtenerRepositoryAsync<ValidadorTexto>(compositor);
        }






        public async Task<bool> Existe(Expression<Func<ValidadorTexto, bool>> predicado)
        {
            List<ValidadorTexto> l = await this.repo.ObtenerAsync(predicado);
            if (l.Count() == 0) return false;
            return true;
        }


        public async Task<ValidadorTexto> CrearAsync(ValidadorTexto entity, CancellationToken cancellationToken = default)
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

        public async Task ActualizarAsync(ValidadorTexto entity)
        {

            ValidadorTexto o = await this.repo.UnicoAsync(x => x.Id == entity.Id);

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
            o.longmax = entity.longmax;
            o.longmin = entity.longmin;
            o.Propiedad = entity.Propiedad;
            o.regexp = entity.regexp;
            o.valordefaulr = entity.valordefaulr;

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
        public async Task<IPaginado<ValidadorTexto>> ObtenerPaginadoAsync(Consulta Query, Func<IQueryable<ValidadorTexto>, IIncludableQueryable<ValidadorTexto, object>> include = null, bool disableTracking = true, CancellationToken cancellationToken = default)
        {
            Query = GetDefaultQuery(Query);
            var respuesta = await this.repo.ObtenerPaginadoAsync(Query, null);

            return respuesta;
        }

        public Task<IEnumerable<ValidadorTexto>> CrearAsync(params ValidadorTexto[] entities)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<ValidadorTexto>> CrearAsync(IEnumerable<ValidadorTexto> entities, CancellationToken cancellationToken = default)
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

        public async Task<ICollection<string>> Eliminar(string[] ids)
        {
            ValidadorTexto vt;
            ICollection<string> listaEliminados = new HashSet<string>();
            foreach (var Id in ids)
            {
                vt = await this.repo.UnicoAsync(x => x.Id == Id);
                if (vt != null)
                {
                    UDT.Context.Entry(vt).State = EntityState.Deleted;
                    listaEliminados.Add(vt.Id);
                }
            }
            UDT.SaveChanges();
            return listaEliminados;
        }

        public Task<List<ValidadorTexto>> ObtenerAsync(Expression<Func<ValidadorTexto, bool>> predicado)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<ValidadorTexto>> ObtenerListaAsync(string SqlCommand)
        {
            throw new NotImplementedException();
        }

        public Task<IPaginado<ValidadorTexto>> ObtenerPaginadoAsync(Expression<Func<ValidadorTexto, bool>> predicate = null, Func<IQueryable<ValidadorTexto>, IOrderedQueryable<ValidadorTexto>> orderBy = null, Func<IQueryable<ValidadorTexto>, IIncludableQueryable<ValidadorTexto, object>> include = null, int index = 0, int size = 20, bool disableTracking = true, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }



        public Task Restaurar(string[] ids)
        {
            throw new NotImplementedException();
        }

        public Task<ValidadorTexto> UnicoAsync(Expression<Func<ValidadorTexto, bool>> predicado = null, Func<IQueryable<ValidadorTexto>, IOrderedQueryable<ValidadorTexto>> ordenarPor = null, Func<IQueryable<ValidadorTexto>, IIncludableQueryable<ValidadorTexto, object>> incluir = null, bool inhabilitarSegumiento = true)
        {
            throw new NotImplementedException();
        }
    }
}
