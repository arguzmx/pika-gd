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
using PIKA.Infraestructura.Comun.Excepciones;
using PIKA.Infraestructura.Comun.Interfaces;
using PIKA.Modelo.GestorDocumental;
using PIKA.Servicio.GestionDocumental.Data;
using PIKA.Servicio.GestionDocumental.Interfaces;
using RepositorioEntidades;

namespace PIKA.Servicio.GestionDocumental.Servicios
{
  public  class ServicioEntradaClasificacion : ContextoServicioGestionDocumental,
        IServicioInyectable, IServicioEntradaClasificacion
    {
        private const string DEFAULT_SORT_COL = "Nombre";
        private const string DEFAULT_SORT_DIRECTION = "asc";

        private IRepositorioAsync<EntradaClasificacion> repo;
        private IRepositorioAsync<ValoracionEntradaClasificacion> respoEC;

        private ICompositorConsulta<EntradaClasificacion> compositor;
        private UnidadDeTrabajo<DBContextGestionDocumental> UDT;
         
        public ServicioEntradaClasificacion(
            IProveedorOpcionesContexto<DBContextGestionDocumental> proveedorOpciones,
           ILogger<ServicioEntradaClasificacion> Logger
           ) : base(proveedorOpciones, Logger)
        {
            this.UDT = new UnidadDeTrabajo<DBContextGestionDocumental>(contexto);
            this.repo = UDT.ObtenerRepositoryAsync<EntradaClasificacion>(new QueryComposer<EntradaClasificacion>());
            this.respoEC = UDT.ObtenerRepositoryAsync<ValoracionEntradaClasificacion>(new QueryComposer<ValoracionEntradaClasificacion>());
        }

        public async Task<bool> Existe(Expression<Func<EntradaClasificacion, bool>> predicado)
        {
            List<EntradaClasificacion> l = await this.repo.ObtenerAsync(predicado);
            if (l.Count() == 0) return false;
            return true;
        }


        public async Task<EntradaClasificacion> CrearAsync(EntradaClasificacion entity, CancellationToken cancellationToken = default)
        {

            if (await Existe(x => x.Clave.Equals
            (entity.Clave, StringComparison.InvariantCultureIgnoreCase)))
            {
                throw new ExElementoExistente(entity.Clave);
            }

            entity.Id = System.Guid.NewGuid().ToString();
            await this.repo.CrearAsync(entity);
            UDT.SaveChanges();

            return entity.Copia();
        }
       
        public async Task ActualizarAsync(EntradaClasificacion entity)
        {

            EntradaClasificacion o = await this.repo.UnicoAsync(x => x.Id == entity.Id);

            if (o == null)
            {
                throw new EXNoEncontrado(entity.Id);
            }

            if (await Existe(x =>
          x.Id != entity.Id
          && x.Clave.Equals(entity.Clave, StringComparison.InvariantCultureIgnoreCase)))
            {
                throw new ExElementoExistente(entity.Clave);
            }

            o.Nombre = entity.Nombre;
            o.Eliminada = entity.Eliminada;
            o.Clave = entity.Clave;
            o.DisposicionEntrada = entity.DisposicionEntrada;
            o.MesesVigenciConcentracion = entity.MesesVigenciConcentracion;
            o.MesesVigenciHistorico = entity.MesesVigenciHistorico;
            o.MesesVigenciTramite = entity.MesesVigenciTramite;
            o.Posicion = entity.Posicion;
            o.TipoDisposicionDocumentalId = entity.TipoDisposicionDocumentalId;
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

        public async Task<IPaginado<EntradaClasificacion>> ObtenerPaginadoAsync(Consulta Query, Func<IQueryable<EntradaClasificacion>, IIncludableQueryable<EntradaClasificacion, object>> include = null, bool disableTracking = true, CancellationToken cancellationToken = default)
        {
            Query = GetDefaultQuery(Query);
            var respuesta = await this.repo.ObtenerPaginadoAsync(Query, include);
            return respuesta;
        }
        public async Task<ICollection<string>> Eliminar(string[] ids)
        {
            EntradaClasificacion c;
            ICollection<string> listaEliminados = new HashSet<string>();
            foreach (var Id in ids)
            {
                c = await this.repo.UnicoAsync(x => x.Id == Id);
                if (c != null)
                {
                    try
                    {
                        c.Eliminada = true;
                        UDT.Context.Entry(c).State = EntityState.Modified;
                        listaEliminados.Add(c.Id);
                    }
                    catch (Exception)
                    { }
                }
            }
            UDT.SaveChanges();

            return listaEliminados;

        }


        public async Task<IEnumerable<string>> Restaurar(string[] ids)
        {
            EntradaClasificacion c;
            ICollection<string> listaEliminados = new HashSet<string>();
            foreach (var Id in ids)
            {
                c = await this.repo.UnicoAsync(x => x.Id == Id);
                if (c != null)
                {
                    c.Eliminada = false;
                    UDT.Context.Entry(c).State = EntityState.Modified;
                    listaEliminados.Add(c.Id);
                }
            }
            UDT.SaveChanges();
            return listaEliminados;
        }



        public async Task<EntradaClasificacion> UnicoAsync(Expression<Func<EntradaClasificacion, bool>> predicado = null, Func<IQueryable<EntradaClasificacion>, IOrderedQueryable<EntradaClasificacion>> ordenarPor = null, Func<IQueryable<EntradaClasificacion>, IIncludableQueryable<EntradaClasificacion, object>> incluir = null, bool inhabilitarSegumiento = true)
        {
            EntradaClasificacion c = await this.repo.UnicoAsync(predicado);
            return c.Copia();
        }

      
        
        public Task<List<EntradaClasificacion>> ObtenerAsync(Expression<Func<EntradaClasificacion, bool>> predicado)
        {
            return this.repo.ObtenerAsync(predicado);
        }

        #region No implmentados



        public Task<IEnumerable<EntradaClasificacion>> CrearAsync(params EntradaClasificacion[] entities)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<EntradaClasificacion>> CrearAsync(IEnumerable<EntradaClasificacion> entities, CancellationToken cancellationToken = default)
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



        

        public Task<List<EntradaClasificacion>> ObtenerAsync(string SqlCommand)
        {
            throw new NotImplementedException();
        }

        public Task<IPaginado<EntradaClasificacion>> ObtenerPaginadoAsync(Expression<Func<EntradaClasificacion, bool>> predicate = null, Func<IQueryable<EntradaClasificacion>, IOrderedQueryable<EntradaClasificacion>> orderBy = null, Func<IQueryable<EntradaClasificacion>, IIncludableQueryable<EntradaClasificacion, object>> include = null, int index = 0, int size = 20, bool disableTracking = true, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

      

        




        #endregion
    }
}