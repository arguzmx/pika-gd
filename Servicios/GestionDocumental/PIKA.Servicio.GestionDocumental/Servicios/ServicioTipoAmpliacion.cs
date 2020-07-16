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
    public class ServicioTipoAmpliacion : ContextoServicioGestionDocumental,
        IServicioInyectable, IServicioTipoAmpliacion
    {
        private const string DEFAULT_SORT_COL = "Nombre";
        private const string DEFAULT_SORT_DIRECTION = "asc";

        private IRepositorioAsync<TipoAmpliacion> repo;
        private UnidadDeTrabajo<DBContextGestionDocumental> UDT;

        public ServicioTipoAmpliacion(IProveedorOpcionesContexto<DBContextGestionDocumental> proveedorOpciones,
           ILogger<ServicioTipoAmpliacion> Logger) : base(proveedorOpciones, Logger)
        {
            this.UDT = new UnidadDeTrabajo<DBContextGestionDocumental>(contexto);
            this.repo = UDT.ObtenerRepositoryAsync<TipoAmpliacion>(new QueryComposer<TipoAmpliacion>());
        }

        public async Task<bool> Existe(Expression<Func<TipoAmpliacion, bool>> predicado)
        {
            List<TipoAmpliacion> l = await this.repo.ObtenerAsync(predicado);
            if (l.Count() == 0) return false;
            return true;
        }


        public async Task<TipoAmpliacion> CrearAsync(TipoAmpliacion entity, CancellationToken cancellationToken = default)
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

        public async Task ActualizarAsync(TipoAmpliacion entity)
        {

            TipoAmpliacion o = await this.repo.UnicoAsync(x => x.Id == entity.Id);

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
        public async Task<IPaginado<TipoAmpliacion>> ObtenerPaginadoAsync(Consulta Query, Func<IQueryable<TipoAmpliacion>, IIncludableQueryable<TipoAmpliacion, object>> include = null, bool disableTracking = true, CancellationToken cancellationToken = default)
        {
            Query = GetDefaultQuery(Query);
            var respuesta = await this.repo.ObtenerPaginadoAsync(Query, null);

            return respuesta;
        }

        public Task<IEnumerable<TipoAmpliacion>> CrearAsync(params TipoAmpliacion[] entities)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<TipoAmpliacion>> CrearAsync(IEnumerable<TipoAmpliacion> entities, CancellationToken cancellationToken = default)
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
            TipoAmpliacion ta;
            ICollection<string> listaEliminados = new HashSet<string>();
            foreach (var Id in ids)
            {
                ta = await this.repo.UnicoAsync(x => x.Id == Id);
                if (ta != null)
                {
                    UDT.Context.Entry(ta).State = EntityState.Deleted;
                    listaEliminados.Add(ta.Id);
                }
            }
            UDT.SaveChanges();
            return listaEliminados;
        }

        public Task<List<TipoAmpliacion>> ObtenerAsync(Expression<Func<TipoAmpliacion, bool>> predicado)
        {
            throw new NotImplementedException();
        }

        public Task<List<TipoAmpliacion>> ObtenerAsync(string SqlCommand)
        {
            throw new NotImplementedException();
        }

        public Task<IPaginado<TipoAmpliacion>> ObtenerPaginadoAsync(Expression<Func<TipoAmpliacion, bool>> predicate = null, Func<IQueryable<TipoAmpliacion>, IOrderedQueryable<TipoAmpliacion>> orderBy = null, Func<IQueryable<TipoAmpliacion>, IIncludableQueryable<TipoAmpliacion, object>> include = null, int index = 0, int size = 20, bool disableTracking = true, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }



        public Task<IEnumerable<string>> Restaurar(string[] ids)
        {
            throw new NotImplementedException();
        }

        public async Task<TipoAmpliacion> UnicoAsync(Expression<Func<TipoAmpliacion, bool>> predicado = null, Func<IQueryable<TipoAmpliacion>, IOrderedQueryable<TipoAmpliacion>> ordenarPor = null, Func<IQueryable<TipoAmpliacion>, IIncludableQueryable<TipoAmpliacion, object>> incluir = null, bool inhabilitarSegumiento = true)
        {
            TipoAmpliacion t = await this.repo.UnicoAsync(predicado);
            return t.CopiaTipoAmpliacion();
        }
    }
}
