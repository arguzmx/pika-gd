
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
    public class ServicioComentarioTransferencia : ContextoServicioGestionDocumental,
        IServicioInyectable, IServicioComentarioTransferencia
    {
        private const string DEFAULT_SORT_COL = "Comentario";
        private const string DEFAULT_SORT_DIRECTION = "asc";

        private IRepositorioAsync<ComentarioTransferencia> repo;
        private UnidadDeTrabajo<DBContextGestionDocumental> UDT;

        public ServicioComentarioTransferencia(IProveedorOpcionesContexto<DBContextGestionDocumental> proveedorOpciones,
           ILogger<ServicioComentarioTransferencia> Logger) : base(proveedorOpciones, Logger)
        {
            this.UDT = new UnidadDeTrabajo<DBContextGestionDocumental>(contexto);
            this.repo = UDT.ObtenerRepositoryAsync<ComentarioTransferencia>(new QueryComposer<ComentarioTransferencia>());
        }

        public async Task<bool> Existe(Expression<Func<ComentarioTransferencia, bool>> predicado)
        {
            List<ComentarioTransferencia> l = await this.repo.ObtenerAsync(predicado);
            if (l.Count() == 0) return false;
            return true;
        }


        public async Task<ComentarioTransferencia> CrearAsync(ComentarioTransferencia entity, CancellationToken cancellationToken = default)
        {

            if (await Existe(x => x.Comentario.Equals(entity.Comentario, StringComparison.InvariantCultureIgnoreCase)))
            {
                throw new ExElementoExistente(entity.Comentario);
            }

            entity.Id = System.Guid.NewGuid().ToString();
            await this.repo.CrearAsync(entity);
            UDT.SaveChanges();
            return entity;
        }

        public async Task ActualizarAsync(ComentarioTransferencia entity)
        {

            ComentarioTransferencia o = await this.repo.UnicoAsync(x => x.Id == entity.Id);

            if (o == null)
            {
                throw new EXNoEncontrado(entity.Id);
            }

            if (await Existe(x =>
            x.Id != entity.Id
            && x.Comentario.Equals(entity.Comentario, StringComparison.InvariantCultureIgnoreCase)))
            {
                throw new ExElementoExistente(entity.Comentario);
            }

            o.Comentario = entity.Comentario;
            o.Fecha = entity.Fecha;
            o.Publico = entity.Publico;

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
        public async Task<IPaginado<ComentarioTransferencia>> ObtenerPaginadoAsync(Consulta Query, Func<IQueryable<ComentarioTransferencia>, IIncludableQueryable<ComentarioTransferencia, object>> include = null, bool disableTracking = true, CancellationToken cancellationToken = default)
        {
            Query = GetDefaultQuery(Query);
            var respuesta = await this.repo.ObtenerPaginadoAsync(Query, null);

            return respuesta;
        }

        public Task<IEnumerable<ComentarioTransferencia>> CrearAsync(params ComentarioTransferencia[] entities)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<ComentarioTransferencia>> CrearAsync(IEnumerable<ComentarioTransferencia> entities, CancellationToken cancellationToken = default)
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
            ComentarioTransferencia a;
            ICollection<string> listaEliminados = new HashSet<string>();
            foreach (var Id in ids)
            {
                a = await this.repo.UnicoAsync(x => x.Id == Id);
                if (a != null)
                {
                    UDT.Context.Entry(a).State = EntityState.Deleted;
                    listaEliminados.Add(a.Id);
                }
            }
            UDT.SaveChanges();
            return listaEliminados;
        }

        public Task<List<ComentarioTransferencia>> ObtenerAsync(Expression<Func<ComentarioTransferencia, bool>> predicado)
        {
            throw new NotImplementedException();
        }

        public Task<List<ComentarioTransferencia>> ObtenerAsync(string SqlCommand)
        {
            throw new NotImplementedException();
        }

        public Task<IPaginado<ComentarioTransferencia>> ObtenerPaginadoAsync(Expression<Func<ComentarioTransferencia, bool>> predicate = null, Func<IQueryable<ComentarioTransferencia>, IOrderedQueryable<ComentarioTransferencia>> orderBy = null, Func<IQueryable<ComentarioTransferencia>, IIncludableQueryable<ComentarioTransferencia, object>> include = null, int index = 0, int size = 20, bool disableTracking = true, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }



        public Task<IEnumerable<string>> Restaurar(string[] ids)
        {
            throw new NotImplementedException();
        }

        public async Task<ComentarioTransferencia> UnicoAsync(Expression<Func<ComentarioTransferencia, bool>> predicado = null, Func<IQueryable<ComentarioTransferencia>, IOrderedQueryable<ComentarioTransferencia>> ordenarPor = null, Func<IQueryable<ComentarioTransferencia>, IIncludableQueryable<ComentarioTransferencia, object>> incluir = null, bool inhabilitarSegumiento = true)
        {
            ComentarioTransferencia t = await this.repo.UnicoAsync(predicado);
            return t.CopiaComentarioTransferencia();
        }
    }
}
