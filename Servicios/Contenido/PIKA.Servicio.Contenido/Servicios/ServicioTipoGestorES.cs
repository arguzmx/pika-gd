using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.Extensions.Logging;
using PIKA.Infraestructura.Comun;
using PIKA.Infraestructura.Comun.Excepciones;
using PIKA.Infraestructura.Comun.Interfaces;
using PIKA.Modelo.Contenido;
using PIKA.Servicio.Contenido.Interfaces;
using RepositorioEntidades;

namespace PIKA.Servicio.Contenido.Servicios
{
   public class ServicioTipoGestorES : ContextoServicioContenido,
        IServicioInyectable, IServicioTipoGestorES
    {
        private const string DEFAULT_SORT_COL = "Volumenid";
        private const string DEFAULT_SORT_DIRECTION = "asc";

        private IRepositorioAsync<TipoGestorES> repo;
        private ICompositorConsulta<TipoGestorES> compositor;
        private UnidadDeTrabajo<DbContextContenido> UDT;

        public ServicioTipoGestorES(
            IProveedorOpcionesContexto<DbContextContenido> proveedorOpciones,
        ICompositorConsulta<TipoGestorES> compositorConsulta,
        ILogger<ServicioTipoGestorES> Logger,
        IServicioCache servicioCache) : base(proveedorOpciones, Logger, servicioCache)
        {
            this.UDT = new UnidadDeTrabajo<DbContextContenido>(contexto);
            this.compositor = compositorConsulta;
            this.repo = UDT.ObtenerRepositoryAsync<TipoGestorES>(compositor);
        }






        public async Task<bool> Existe(Expression<Func<TipoGestorES, bool>> predicado)
        {
            List<TipoGestorES> l = await this.repo.ObtenerAsync(predicado);
            if (l.Count() == 0) return false;
            return true;
        }


        public async Task<TipoGestorES> CrearAsync(TipoGestorES entity, CancellationToken cancellationToken = default)
        {

            if (await Existe(x => x.Nombre.Equals(entity.Nombre, StringComparison.InvariantCultureIgnoreCase)))
            {
                throw new ExElementoExistente(entity.Nombre);
            }

            entity.Id = System.Guid.NewGuid().ToString();
            entity.Volumenes.FirstOrDefault().Id = System.Guid.NewGuid().ToString();
            entity.Volumenid = entity.Volumenes.FirstOrDefault().Id;
            entity.Volumenes.FirstOrDefault().TipoGestorESId = entity.Id;
            await this.repo.CrearAsync(entity);
            UDT.SaveChanges();
            try
            {
                return ClonaTipoGEstorES(entity);
            }
            catch (Exception ex)
            {
                await this.repo.Eliminar(entity);
                UDT.SaveChanges();
                throw (ex);
            }

        }
        private TipoGestorES ClonaTipoGEstorES(TipoGestorES entidad)
        {
            TipoGestorES resuldtado = new TipoGestorES()
            {
                Id = entidad.Id,
                Nombre=entidad.Nombre,
                Volumenid=entidad.Volumenid,
                Eliminada=entidad.Eliminada
            };

            return resuldtado;
        }
        public async Task ActualizarAsync(TipoGestorES entity)
        {

            TipoGestorES o = await this.repo.UnicoAsync(x => x.Id == entity.Id);

            if (o == null)
            {
                throw new EXNoEncontrado(entity.Id);
            }

            if (await Existe(x =>
            x.Id != entity.Id && x.Nombre.Equals(entity.Nombre, StringComparison.InvariantCultureIgnoreCase)))
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
        public async Task<IPaginado<TipoGestorES>> ObtenerPaginadoAsync(Consulta Query, Func<IQueryable<TipoGestorES>, IIncludableQueryable<TipoGestorES, object>> include = null, bool disableTracking = true, CancellationToken cancellationToken = default)
        {
            Query = GetDefaultQuery(Query);
            var respuesta = await this.repo.ObtenerPaginadoAsync(Query, null);

            return respuesta;
        }

        public Task<IEnumerable<TipoGestorES>> CrearAsync(params TipoGestorES[] entities)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<TipoGestorES>> CrearAsync(IEnumerable<TipoGestorES> entities, CancellationToken cancellationToken = default)
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
            TipoGestorES d;
            ICollection<string> listaEliminados = new HashSet<string>();
            foreach (var Id in ids)
            {
                d = await this.repo.UnicoAsync(x => x.Id == Id);
                if (d != null)
                {
                    d.Eliminada = true;
                    UDT.Context.Entry(d).State = EntityState.Modified;
                    listaEliminados.Add(d.Id);
                }
            }
            UDT.SaveChanges();
            return listaEliminados;
        }

        public Task<List<TipoGestorES>> ObtenerAsync(Expression<Func<TipoGestorES, bool>> predicado)
        {
            return this.repo.ObtenerAsync(predicado);
        }

        public Task<List<TipoGestorES>> ObtenerAsync(string SqlCommand)
        {
            return this.repo.ObtenerAsync(SqlCommand);
        }

        public Task<IPaginado<TipoGestorES>> ObtenerPaginadoAsync(Expression<Func<TipoGestorES, bool>> predicate = null, Func<IQueryable<TipoGestorES>, IOrderedQueryable<TipoGestorES>> orderBy = null, Func<IQueryable<TipoGestorES>, IIncludableQueryable<TipoGestorES, object>> include = null, int index = 0, int size = 20, bool disableTracking = true, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }



        public Task<IEnumerable<string>> Restaurar(string[] ids)
        {
            throw new NotImplementedException();
        }

        public async Task<TipoGestorES> UnicoAsync(Expression<Func<TipoGestorES, bool>> predicado = null, Func<IQueryable<TipoGestorES>, IOrderedQueryable<TipoGestorES>> ordenarPor = null, Func<IQueryable<TipoGestorES>, IIncludableQueryable<TipoGestorES, object>> incluir = null, bool inhabilitarSegumiento = true)
        {

            TipoGestorES d = await this.repo.UnicoAsync(predicado);

         
            return d.CopiaTipoGestorES();
        }

    }



}