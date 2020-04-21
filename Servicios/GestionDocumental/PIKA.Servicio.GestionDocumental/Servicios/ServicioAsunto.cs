using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.Extensions.Logging;
using PIKA.Infraestructura.Comun;
using PIKA.Infraestructura.Comun.Excepciones;
using PIKA.Infraestructura.Comun.Interfaces;
using PIKA.Modelo.GestorDocumental;
using PIKA.Servicio.GestionDocumental.Data;
using PIKA.Servicio.GestionDocumental.Interfaces;
using RepositorioEntidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PIKA.Servicio.GestionDocumental.Servicios
{
    public class ServicioAsunto : ContextoServicioGestionDocumental,
        IServicioInyectable, IServicioAsunto
    {
        private const string DEFAULT_SORT_COL = "Nombre";
        private const string DEFAULT_SORT_DIRECTION = "asc";

        private IServicioCache cache;
        private IRepositorioAsync<Asunto> repo;
        private ICompositorConsulta<Asunto> compositor;
        private ILogger<ServicioAsunto> logger;
        private DBContextGestionDocumental contexto;
        private UnidadDeTrabajo<DBContextGestionDocumental> UDT;

        public ServicioAsunto(IProveedorOpcionesContexto<DBContextGestionDocumental> proveedorOpciones,
           ICompositorConsulta<Asunto> compositorConsulta,
           ILogger<ServicioAsunto> Logger,
           IServicioCache servicioCache) : base(proveedorOpciones, Logger, servicioCache)
        {
            this.UDT = new UnidadDeTrabajo<DBContextGestionDocumental>(contexto);
            this.compositor = compositorConsulta;
            this.repo = UDT.ObtenerRepositoryAsync<Asunto>(compositor);
        }

        public async Task<bool> Existe(Expression<Func<Asunto, bool>> predicado)
        {
            List<Asunto> l = await this.repo.ObtenerAsync(predicado);
            if (l.Count() == 0) return false;
            return true;
        }


        public async Task<Asunto> CrearAsync(Asunto entity, CancellationToken cancellationToken = default)
        {

            if (await Existe(x => x.ActivoId.Equals(entity.ActivoId, StringComparison.InvariantCultureIgnoreCase)))
            {
                throw new ExElementoExistente(entity.ActivoId);
            }

            entity.Id = System.Guid.NewGuid().ToString();
            await this.repo.CrearAsync(entity);
            UDT.SaveChanges();
            return entity;
        }

        public async Task ActualizarAsync(Asunto entity)
        {

            Asunto o = await this.repo.UnicoAsync(x => x.Id == entity.Id);

            if (o == null)
            {
                throw new EXNoEncontrado(entity.Id);
            }

            if (await Existe(x =>
            x.Id != entity.Id & x.ActivoId == entity.ActivoId
            && x.Contenido.Equals(entity.Contenido, StringComparison.InvariantCultureIgnoreCase)))
            {
                throw new ExElementoExistente(entity.Contenido);
            }

            o.Contenido = entity.Contenido;

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
        public async Task<IPaginado<Asunto>> ObtenerPaginadoAsync(Consulta Query, Func<IQueryable<Asunto>, IIncludableQueryable<Asunto, object>> include = null, bool disableTracking = true, CancellationToken cancellationToken = default)
        {
            Query = GetDefaultQuery(Query);
            var respuesta = await this.repo.ObtenerPaginadoAsync(Query, null);

            return respuesta;
        }

        public Task<IEnumerable<Asunto>> CrearAsync(params Asunto[] entities)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Asunto>> CrearAsync(IEnumerable<Asunto> entities, CancellationToken cancellationToken = default)
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
            Asunto asunto;
            ICollection<string> listaEliminados = new HashSet<string>();
            foreach (var Id in ids)
            {
                asunto = await this.repo.UnicoAsync(x => x.Id == Id);
                if (asunto != null)
                {
                    UDT.Context.Entry(asunto).State = EntityState.Deleted;
                    listaEliminados.Add(asunto.Id);
                }
            }
            UDT.SaveChanges();
            return listaEliminados;
        }

        public Task<List<Asunto>> ObtenerAsync(Expression<Func<Asunto, bool>> predicado)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Asunto>> ObtenerListaAsync(string SqlCommand)
        {
            throw new NotImplementedException();
        }

        public Task<IPaginado<Asunto>> ObtenerPaginadoAsync(Expression<Func<Asunto, bool>> predicate = null, Func<IQueryable<Asunto>, IOrderedQueryable<Asunto>> orderBy = null, Func<IQueryable<Asunto>, IIncludableQueryable<Asunto, object>> include = null, int index = 0, int size = 20, bool disableTracking = true, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }


        public Task Restaurar(string[] ids)
        {
            throw new NotImplementedException();
        }

        public Task<Asunto> UnicoAsync(Expression<Func<Asunto, bool>> predicado = null, Func<IQueryable<Asunto>, IOrderedQueryable<Asunto>> ordenarPor = null, Func<IQueryable<Asunto>, IIncludableQueryable<Asunto, object>> incluir = null, bool inhabilitarSegumiento = true)
        {
            throw new NotImplementedException();
        }
    }
}

