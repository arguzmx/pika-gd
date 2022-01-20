using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.Extensions.Logging;
using PIKA.Infraestructura.Comun;
using PIKA.Infraestructura.Comun.Excepciones;
using PIKA.Infraestructura.Comun.Interfaces;
using PIKA.Infraestructura.Comun.Seguridad;
using PIKA.Infraestructura.Comun.Servicios;
using PIKA.Modelo.Aplicacion.Tareas;
using PIKA.Servicio.AplicacionPlugin.Interfaces;
using RepositorioEntidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PIKA.Servicio.AplicacionPlugin.Servicios
{
    public class ServicioBitacoraTarea : ContextoServicioAplicacion
        , IServicioInyectable, IServicioBitacoraTarea
    {

        private UnidadDeTrabajo<DbContextAplicacion> UDT;
        private IRepositorioAsync<BitacoraTarea> repo;

        private const string DEFAULT_SORT_COL = "FechaEjecucion";
        private const string DEFAULT_SORT_DIRECTION = "desc";

        public UsuarioAPI usuario { get; set; }
        public PermisoAplicacion permisos { get; set; }

        public ServicioBitacoraTarea(IProveedorOpcionesContexto<DbContextAplicacion> proveedorOpciones,
            ILogger<ServicioLog> Logger, IServicioCache servicioCache) : base(proveedorOpciones, Logger, servicioCache)
        {
            this.UDT = new UnidadDeTrabajo<DbContextAplicacion>(contexto);
            this.repo = UDT.ObtenerRepositoryAsync(new QueryComposer<BitacoraTarea>());
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

        public async Task<BitacoraTarea> CrearAsync(BitacoraTarea entity, CancellationToken cancellationToken = default)
        {
      
            this.UDT.Context.BitacoraTareas.Add(entity);
            this.UDT.SaveChanges();
            return entity;
        }



        public async Task<IPaginado<BitacoraTarea>> ObtenerPaginadoAsync(Consulta Query, Func<IQueryable<BitacoraTarea>, IIncludableQueryable<BitacoraTarea, object>> include = null, bool disableTracking = true, CancellationToken cancellationToken = default)
        {
            
            Query = GetDefaultQuery(Query);
            var respuesta = await this.repo.ObtenerPaginadoAsync(Query, null);
            return respuesta;
        }


        #region NoImplementados
        public async Task ActualizarAsync(BitacoraTarea entity)
        {
            throw new NotImplementedException();
        }
        public Task<IEnumerable<BitacoraTarea>> CrearAsync(params BitacoraTarea[] entities)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<BitacoraTarea>> CrearAsync(IEnumerable<BitacoraTarea> entities, CancellationToken cancellationToken = default)
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

        public Task<ICollection<string>> Eliminar(string[] ids)
        {
            throw new NotImplementedException();
        }

        public Task<bool> Existe(Expression<Func<BitacoraTarea, bool>> predicado)
        {
            throw new NotImplementedException();
        }

        public Task<List<BitacoraTarea>> ObtenerAsync(Expression<Func<BitacoraTarea, bool>> predicado)
        {
            throw new NotImplementedException();
        }

        public Task<List<BitacoraTarea>> ObtenerAsync(string SqlCommand)
        {
            throw new NotImplementedException();
        }

        public Task<IPaginado<BitacoraTarea>> ObtenerPaginadoAsync(Expression<Func<BitacoraTarea, bool>> predicate = null, Func<IQueryable<BitacoraTarea>, IOrderedQueryable<BitacoraTarea>> orderBy = null, Func<IQueryable<BitacoraTarea>, IIncludableQueryable<BitacoraTarea, object>> include = null, int index = 0, int size = 20, bool disableTracking = true, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }


        public Task<IEnumerable<string>> Restaurar(string[] ids)
        {
            throw new NotImplementedException();
        }

        public Task<BitacoraTarea> UnicoAsync(Expression<Func<BitacoraTarea, bool>> predicado = null, Func<IQueryable<BitacoraTarea>, IOrderedQueryable<BitacoraTarea>> ordenarPor = null, Func<IQueryable<BitacoraTarea>, IIncludableQueryable<BitacoraTarea, object>> incluir = null, bool inhabilitarSegumiento = true)
        {
            throw new NotImplementedException();
        }


        #endregion
    }
}
