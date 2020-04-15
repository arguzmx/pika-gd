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
   public class ServicioAtributoMetadato : ContextoServicioMetadatos, IServicioInyectable, IServicioAtributoMetadato
    {
        private const string DEFAULT_SORT_COL = "Nombre";
        private const string DEFAULT_SORT_DIRECTION = "asc";

        private IServicioCache cache;
        private IRepositorioAsync<AtributoMetadato> repo;
        private ICompositorConsulta<AtributoMetadato> compositor;
        private ILogger<ServicioAtributoMetadato> logger;
        private DbContextMetadatos contexto;
        private UnidadDeTrabajo<DbContextMetadatos> UDT;
        public ServicioAtributoMetadato(
          IProveedorOpcionesContexto<DbContextMetadatos> proveedorOpciones,
          ICompositorConsulta<AtributoMetadato> compositorConsulta,
          ILogger<ServicioAtributoMetadato> Logger,
          IServicioCache servicioCache) : base(proveedorOpciones, Logger, servicioCache)
        {
            this.UDT = new UnidadDeTrabajo<DbContextMetadatos>(contexto);
            this.compositor = compositorConsulta;
            this.repo = UDT.ObtenerRepositoryAsync<AtributoMetadato>(compositor);
        }


        public async Task<bool> Existe(Expression<Func<AtributoMetadato, bool>> predicado)
        {
            List<AtributoMetadato> l = await this.repo.ObtenerAsync(predicado);
            if (l.Count() == 0) return false;
            return true;
        }


        public async Task<AtributoMetadato> CrearAsync(AtributoMetadato entity, CancellationToken cancellationToken = default)
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

        public async Task ActualizarAsync(AtributoMetadato entity)
        {

            AtributoMetadato o = await this.repo.UnicoAsync(x => x.Id == entity.Id);

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

            o.Id = entity.Id;
          

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
        public async Task<IPaginado<AtributoMetadato>> ObtenerPaginadoAsync(Consulta Query, Func<IQueryable<AtributoMetadato>, IIncludableQueryable<AtributoMetadato, object>> include = null, bool disableTracking = true, CancellationToken cancellationToken = default)
        {
            Query = GetDefaultQuery(Query);
            var respuesta = await this.repo.ObtenerPaginadoAsync(Query, null);

            return respuesta;
        }

        public Task<IEnumerable<AtributoMetadato>> CrearAsync(params AtributoMetadato[] entities)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<AtributoMetadato>> CrearAsync(IEnumerable<AtributoMetadato> entities, CancellationToken cancellationToken = default)
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
            AtributoMetadato am;
            ICollection<string> listaEliminados = new HashSet<string>();
            foreach (var Id in ids)
            {
                am = await this.repo.UnicoAsync(x => x.Id == Id);
                if (am != null)
                {
                    UDT.Context.Entry(am).State = EntityState.Deleted;
                    listaEliminados.Add(am.Id);
                }
            }
            UDT.SaveChanges();
            return listaEliminados;
        }

        public Task<List<AtributoMetadato>> ObtenerAsync(Expression<Func<AtributoMetadato, bool>> predicado)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<AtributoMetadato>> ObtenerListaAsync(string SqlCommand)
        {
            throw new NotImplementedException();
        }

        public Task<IPaginado<AtributoMetadato>> ObtenerPaginadoAsync(Expression<Func<AtributoMetadato, bool>> predicate = null, Func<IQueryable<AtributoMetadato>, IOrderedQueryable<AtributoMetadato>> orderBy = null, Func<IQueryable<AtributoMetadato>, IIncludableQueryable<AtributoMetadato, object>> include = null, int index = 0, int size = 20, bool disableTracking = true, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }



        public Task Restaurar(string[] ids)
        {
            throw new NotImplementedException();
        }

        public Task<AtributoMetadato> UnicoAsync(Expression<Func<AtributoMetadato, bool>> predicado = null, Func<IQueryable<AtributoMetadato>, IOrderedQueryable<AtributoMetadato>> ordenarPor = null, Func<IQueryable<AtributoMetadato>, IIncludableQueryable<AtributoMetadato, object>> incluir = null, bool inhabilitarSegumiento = true)
        {
            throw new NotImplementedException();
        }
    }
}
