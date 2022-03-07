using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.Extensions.Logging;
using PIKA.Infraestructura.Comun;
using PIKA.Infraestructura.Comun.Excepciones;
using PIKA.Infraestructura.Comun.Interfaces;
using PIKA.Infraestructura.Comun.Seguridad;
using PIKA.Infraestructura.Comun.Servicios;
using PIKA.Modelo.Aplicacion;
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
    public class ServicioTareaEnDemanda : ContextoServicioAplicacion
        , IServicioInyectable, IServicioTareaEnDemanda
    {

        private UnidadDeTrabajo<DbContextAplicacion> UDT;
        private IRepositorioAsync<ColaTareaEnDemanda> repo;

        private const string DEFAULT_SORT_COL = "Nombre";
        private const string DEFAULT_SORT_DIRECTION = "asc";

        public UsuarioAPI usuario { get; set; }
        public PermisoAplicacion permisos { get; set; }
        
        public ServicioTareaEnDemanda(
            IProveedorOpcionesContexto<DbContextAplicacion> proveedorOpciones,
            ILogger<ServicioLog> Logger,
            IServicioCache servicioCache) : base(proveedorOpciones, Logger, servicioCache)

        {
            this.UDT = new UnidadDeTrabajo<DbContextAplicacion>(contexto);
            this.repo = UDT.ObtenerRepositoryAsync(new QueryComposer<ColaTareaEnDemanda>());
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

        public async Task<ColaTareaEnDemanda> CrearAsync(ColaTareaEnDemanda entity, CancellationToken cancellationToken = default)
        {
            
            this.UDT.Context.TareasEnDemanda.Add(entity);
            this.UDT.SaveChanges();
            return entity;

        }


        public async Task<ColaTareaEnDemanda> UnicoAsync(Expression<Func<ColaTareaEnDemanda, bool>> predicado = null, Func<IQueryable<ColaTareaEnDemanda>, IOrderedQueryable<ColaTareaEnDemanda>> ordenarPor = null, Func<IQueryable<ColaTareaEnDemanda>, IIncludableQueryable<ColaTareaEnDemanda, object>> incluir = null, bool inhabilitarSegumiento = true)
        {
            return await repo.UnicoAsync(predicado);
            
        }
        public async Task<bool> Existe(Expression<Func<ColaTareaEnDemanda, bool>> predicado)
        {
            return await repo.UnicoAsync(predicado) != null; ;
        }

        public async Task<IPaginado<ColaTareaEnDemanda>> ObtenerPaginadoAsync(Consulta Query, Func<IQueryable<ColaTareaEnDemanda>, IIncludableQueryable<ColaTareaEnDemanda, object>> include = null, bool disableTracking = true, CancellationToken cancellationToken = default)
        {
            
            Query = GetDefaultQuery(Query);
            var respuesta = await this.repo.ObtenerPaginadoAsync(Query, null);
            respuesta.Elementos.ToList();
            return respuesta;
        }

        public async Task<List<ColaTareaEnDemanda>> ObtenerAsync(Expression<Func<ColaTareaEnDemanda, bool>> predicado)
        {
            return await  this.repo.ObtenerAsync(predicado);
        }


        #region NoImplementados

        public Task<IEnumerable<ColaTareaEnDemanda>> CrearAsync(params ColaTareaEnDemanda[] entities)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<ColaTareaEnDemanda>> CrearAsync(IEnumerable<ColaTareaEnDemanda> entities, CancellationToken cancellationToken = default)
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


           public Task<List<ColaTareaEnDemanda>> ObtenerAsync(string SqlCommand)
        {
            throw new NotImplementedException();
        }

        public Task<IPaginado<ColaTareaEnDemanda>> ObtenerPaginadoAsync(Expression<Func<ColaTareaEnDemanda, bool>> predicate = null, Func<IQueryable<ColaTareaEnDemanda>, IOrderedQueryable<ColaTareaEnDemanda>> orderBy = null, Func<IQueryable<ColaTareaEnDemanda>, IIncludableQueryable<ColaTareaEnDemanda, object>> include = null, int index = 0, int size = 20, bool disableTracking = true, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }


        public Task<IEnumerable<string>> Restaurar(string[] ids)
        {
            throw new NotImplementedException();
        }

        public Task ActualizarAsync(ColaTareaEnDemanda entity)
        {
            throw new NotImplementedException();
        }



        #endregion
    }
}
