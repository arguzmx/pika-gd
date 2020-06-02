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
    public class ServicioComentarioPrestamo : ContextoServicioGestionDocumental,
        IServicioInyectable, IServicioComentarioPrestamo
    {
        private const string DEFAULT_SORT_COL = "Fecha";
        private const string DEFAULT_SORT_DIRECTION = "asc";

        private IRepositorioAsync<ComentarioPrestamo> repo;
        private ICompositorConsulta<ComentarioPrestamo> compositor;
        private UnidadDeTrabajo<DBContextGestionDocumental> UDT;

        public ServicioComentarioPrestamo(IProveedorOpcionesContexto<DBContextGestionDocumental> proveedorOpciones,
           ICompositorConsulta<ComentarioPrestamo> compositorConsulta,
           ILogger<ServicioComentarioPrestamo> Logger,
           IServicioCache servicioCache) : base(proveedorOpciones, Logger, servicioCache)
        {
            this.UDT = new UnidadDeTrabajo<DBContextGestionDocumental>(contexto);
            this.compositor = compositorConsulta;
            this.repo = UDT.ObtenerRepositoryAsync<ComentarioPrestamo>(compositor);
        }

        public async Task<bool> Existe(Expression<Func<ComentarioPrestamo, bool>> predicado)
        {
            List<ComentarioPrestamo> l = await this.repo.ObtenerAsync(predicado);
            if (l.Count() == 0) return false;
            return true;
        }


        public async Task<ComentarioPrestamo> CrearAsync(ComentarioPrestamo entity, CancellationToken cancellationToken = default)
        {

            if (await Existe(x => x.PrestamoId == entity.PrestamoId &&x.Comentario.Equals(entity.Comentario, StringComparison.InvariantCultureIgnoreCase)))
            {
                throw new ExElementoExistente(entity.Id);
            }

            entity.Id = Guid.NewGuid().ToString();
            entity.Fecha = DateTime.UtcNow;
            await this.repo.CrearAsync(entity);
            UDT.SaveChanges();
            return entity;
        }

        public async Task ActualizarAsync(ComentarioPrestamo entity)
        {

            ComentarioPrestamo o = await this.repo.UnicoAsync(x => x.Id == entity.Id);

            if (o == null)
            {
                throw new EXNoEncontrado(entity.Id);
            }

            if (await Existe(x =>
            x.Id != entity.Id & x.Comentario == entity.Comentario))
            {
                throw new ExElementoExistente(entity.Id);
            }

            o.Comentario = entity.Comentario;

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
        public async Task<IPaginado<ComentarioPrestamo>> ObtenerPaginadoAsync(Consulta Query, Func<IQueryable<ComentarioPrestamo>, IIncludableQueryable<ComentarioPrestamo, object>> include = null, bool disableTracking = true, CancellationToken cancellationToken = default)
        {
            Query = GetDefaultQuery(Query);
            var respuesta = await this.repo.ObtenerPaginadoAsync(Query, null);

            return respuesta;
        }

        public Task<IEnumerable<ComentarioPrestamo>> CrearAsync(params ComentarioPrestamo[] entities)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<ComentarioPrestamo>> CrearAsync(IEnumerable<ComentarioPrestamo> entities, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public async Task EjecutarSql(string sqlCommand)
        {
            throw new NotImplementedException();
        }

        public Task EjecutarSqlBatch(List<string> sqlCommand)
        {
            throw new NotImplementedException();
        }

        public async Task<ICollection<string>> Eliminar(string[] ids)
        {
            ComentarioPrestamo cp;
            ICollection<string> listaEliminados = new HashSet<string>();
            foreach (var Id in ids)
            {
                cp = await this.repo.UnicoAsync(x => x.Id == Id);
                if (cp != null)
                {
                    UDT.Context.Entry(cp).State = EntityState.Deleted;
                    listaEliminados.Add(cp.Id);
                }
            }
            UDT.SaveChanges();
            return listaEliminados;
        }

        public Task<List<ComentarioPrestamo>> ObtenerAsync(Expression<Func<ComentarioPrestamo, bool>> predicado)
        {
            return this.repo.ObtenerAsync(predicado);
        }

        public Task<List<ComentarioPrestamo>> ObtenerAsync(string SqlCommand)
        {
            return this.repo.ObtenerAsync(SqlCommand);
        }

        public Task<IPaginado<ComentarioPrestamo>> ObtenerPaginadoAsync(Expression<Func<ComentarioPrestamo, bool>> predicate = null, Func<IQueryable<ComentarioPrestamo>, IOrderedQueryable<ComentarioPrestamo>> orderBy = null, Func<IQueryable<ComentarioPrestamo>, IIncludableQueryable<ComentarioPrestamo, object>> include = null, int index = 0, int size = 20, bool disableTracking = true, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }


        public Task<IEnumerable<string>> Restaurar(string[] ids)
        {
            throw new NotImplementedException();
        }

        public async Task<ComentarioPrestamo> UnicoAsync(Expression<Func<ComentarioPrestamo, bool>> predicado = null, Func<IQueryable<ComentarioPrestamo>, IOrderedQueryable<ComentarioPrestamo>> ordenarPor = null, Func<IQueryable<ComentarioPrestamo>, IIncludableQueryable<ComentarioPrestamo, object>> incluir = null, bool inhabilitarSegumiento = true)
        {
            ComentarioPrestamo a = await this.repo.UnicoAsync(predicado);
            return a.CopiaComentarioPrestamo();
        }
    }
}
