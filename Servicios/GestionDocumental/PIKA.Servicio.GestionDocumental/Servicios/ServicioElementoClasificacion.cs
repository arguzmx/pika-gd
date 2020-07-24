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
    public class ServicioElementoClasificacion : ContextoServicioGestionDocumental,
        IServicioInyectable, IServicioElementoClasificacion
    {
        private const string DEFAULT_SORT_COL = "Nombre";
        private const string DEFAULT_SORT_DIRECTION = "asc";

        private IRepositorioAsync<ElementoClasificacion> repo;
        private UnidadDeTrabajo<DBContextGestionDocumental> UDT;

        public ServicioElementoClasificacion(IProveedorOpcionesContexto<DBContextGestionDocumental> proveedorOpciones,
           ILogger<ServicioElementoClasificacion> Logger) : base(proveedorOpciones, Logger)
        {
            this.UDT = new UnidadDeTrabajo<DBContextGestionDocumental>(contexto);
            this.repo = UDT.ObtenerRepositoryAsync<ElementoClasificacion>(new QueryComposer<ElementoClasificacion>());
        }

        public async Task<bool> Existe(Expression<Func<ElementoClasificacion, bool>> predicado)
        {
            List<ElementoClasificacion> l = await this.repo.ObtenerAsync(predicado);
            if (l.Count() == 0) return false;
            return true;
        }


        public async Task<ElementoClasificacion> CrearAsync(ElementoClasificacion entity, CancellationToken cancellationToken = default)
        {

            if (await Existe(x => x.Clave.Equals(entity.Clave, StringComparison.InvariantCultureIgnoreCase)))
            {
                throw new ExElementoExistente(entity.Clave );
            }


            entity.Id = System.Guid.NewGuid().ToString();
            await this.repo.CrearAsync(entity);
            UDT.SaveChanges();
            return entity.Copia();
        }

        public async Task ActualizarAsync(ElementoClasificacion entity)
        {

            ElementoClasificacion o = await this.repo.UnicoAsync(x => x.Id == entity.Id);

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
            o.Posicion = entity.Posicion;
            o.Clave = entity.Clave;
            o.ElementoClasificacionId = entity.ElementoClasificacionId;
            UDT.Context.Entry(o).State = EntityState.Modified;
            UDT.SaveChanges();

        }
        private async Task<bool> ValidarIdPadre(ElementoClasificacion e)
        {
            bool padre = false;
            
            return padre;
        }
        private async Task<bool> ValidarPadreElimando(ElementoClasificacion e) {
            bool padre=false;
            ElementoClasificacion o = await this.repo.UnicoAsync(x => x.Id == e.ElementoClasificacionId);
            if (o != null)
            {
                if (o.Eliminada)
                    padre = true;
                else
                {
                    List<ElementoClasificacion> ListPadre = new List<ElementoClasificacion>();
                    ListPadre.Where(x => x.Id == e.ElementoClasificacionId);
                    foreach (var el in ListPadre)
                    {
                        if (el.Eliminada)
                            padre = true;
                    }
                }
            }
            else
                padre = true;

            return padre;    
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
        public async Task<IPaginado<ElementoClasificacion>> ObtenerPaginadoAsync(Consulta Query, Func<IQueryable<ElementoClasificacion>, IIncludableQueryable<ElementoClasificacion, object>> include = null, bool disableTracking = true, CancellationToken cancellationToken = default)
        {
            Query = GetDefaultQuery(Query);
            var respuesta = await this.repo.ObtenerPaginadoAsync(Query, null);

            return respuesta;
        }


        public async Task<ICollection<string>> Eliminar(string[] ids)
        {
            ElementoClasificacion e;
            ICollection<string> listaEliminados = new HashSet<string>();
            foreach (var Id in ids)
            {
                e = await this.repo.UnicoAsync(x => x.Id == Id);
                if (e != null)
                {
                    e.Eliminada = true;
                    UDT.Context.Entry(e).State = EntityState.Modified;
                    listaEliminados.Add(e.Id);
                }
            }
            UDT.SaveChanges();
            return listaEliminados;
        }

        public async Task<IEnumerable<string>> Restaurar(string[] ids)
        {
            ElementoClasificacion c;
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
      

      
        public async Task<ElementoClasificacion> UnicoAsync(Expression<Func<ElementoClasificacion, bool>> predicado = null, Func<IQueryable<ElementoClasificacion>, IOrderedQueryable<ElementoClasificacion>> ordenarPor = null, Func<IQueryable<ElementoClasificacion>, IIncludableQueryable<ElementoClasificacion, object>> incluir = null, bool inhabilitarSegumiento = true)
        {
            ElementoClasificacion c = await this.repo.UnicoAsync(predicado);
            return c.Copia();
        }

        public Task<List<ElementoClasificacion>> ObtenerAsync(Expression<Func<ElementoClasificacion, bool>> predicado)
        {
            return this.repo.ObtenerAsync(predicado);
        }
        #region Sin implementar
        public async Task EjecutarSql(string sqlCommand)
        {
            throw new NotImplementedException();
        }
        public Task<IEnumerable<ElementoClasificacion>> CrearAsync(params ElementoClasificacion[] entities)
        {
            throw new NotImplementedException();
        }
        public Task EjecutarSqlBatch(List<string> sqlCommand)
        {
            throw new NotImplementedException();
        }
        public Task<List<ElementoClasificacion>> ObtenerAsync(string SqlCommand)
        {
            throw new NotImplementedException();
        }

        public Task<IPaginado<ElementoClasificacion>> ObtenerPaginadoAsync(Expression<Func<ElementoClasificacion, bool>> predicate = null, Func<IQueryable<ElementoClasificacion>, IOrderedQueryable<ElementoClasificacion>> orderBy = null, Func<IQueryable<ElementoClasificacion>, IIncludableQueryable<ElementoClasificacion, object>> include = null, int index = 0, int size = 20, bool disableTracking = true, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }



        public Task<IEnumerable<ElementoClasificacion>> CrearAsync(IEnumerable<ElementoClasificacion> entities, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
