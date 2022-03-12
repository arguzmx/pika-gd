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
        private IRepositorioAsync<TareaEnDemanda> repo;

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
            this.repo = UDT.ObtenerRepositoryAsync(new QueryComposer<TareaEnDemanda>());
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


        public async Task<List<Infraestructura.Comun.Tareas.PostTareaEnDemanda>> TareasUsuario(string UsuarioId, string DominioId, string TenantId)
        {
            var tareas = await this.UDT.Context.TareasEnDemanda.Where(x => x.UsuarioId == UsuarioId && x.DominioId == DominioId && x.TenantId == TenantId).ToListAsync();

            List<Infraestructura.Comun.Tareas.PostTareaEnDemanda> l = new List<Infraestructura.Comun.Tareas.PostTareaEnDemanda>();
            tareas.ForEach(t => {
                l.Add(t.ToPostTareEnDemanda());
            });

            return l;
        }


        public async Task<TareaEnDemanda> CrearAsync(TareaEnDemanda entity, CancellationToken cancellationToken = default)
        {
            
            this.UDT.Context.TareasEnDemanda.Add(entity);
            this.UDT.SaveChanges();
            return entity;

        }
        public async Task CompletarTarea(Guid Id, bool Exito, string OutputPayload, string Error)
        {
            this.UDT.Context.TareasEnDemanda.AsTracking();
            var t = await this.UDT.Context.TareasEnDemanda.Where(x => x.Id == Id).SingleOrDefaultAsync();
            if(t != null)
            {
                t.Completada = true;
                t.Estado = Exito ? Infraestructura.Comun.Tareas.EstadoTarea.Finalizada : Infraestructura.Comun.Tareas.EstadoTarea.Error;
                t.FechaEjecucion = DateTime.UtcNow;
                t.Error = Error;
                t.FechaCaducidad = t.FechaEjecucion.Value.AddHours(t.HorasCaducidad);
                t.OutputPayload = OutputPayload;
                t.Error = Error;
                this.UDT.Context.Entry(t).State = EntityState.Modified;
                this.UDT.Context.SaveChanges();
            }
            
        }

        public async Task EliminarTarea(Guid Id)
        {
            this.UDT.Context.TareasEnDemanda.AsTracking();
            var t = await this.UDT.Context.TareasEnDemanda.Where(x => x.Id == Id).SingleOrDefaultAsync();
            if (t != null)
            {
                this.UDT.Context.TareasEnDemanda.Remove(t);
                this.UDT.Context.SaveChanges();
            }
        }

        public async Task ActualizaEstadoTarea(Guid Id, Infraestructura.Comun.Tareas.EstadoTarea Estado)
        {
            this.UDT.Context.TareasEnDemanda.AsTracking();
            var t = await this.UDT.Context.TareasEnDemanda.Where(x => x.Id == Id).SingleOrDefaultAsync();
            if (t != null)
            {
                t.Estado = Estado;
                this.UDT.Context.Entry(t).State = EntityState.Modified;
                this.UDT.Context.SaveChanges();
            }
        }


        public async Task<TareaEnDemanda> UnicoAsync(Expression<Func<TareaEnDemanda, bool>> predicado = null, Func<IQueryable<TareaEnDemanda>, IOrderedQueryable<TareaEnDemanda>> ordenarPor = null, Func<IQueryable<TareaEnDemanda>, IIncludableQueryable<TareaEnDemanda, object>> incluir = null, bool inhabilitarSegumiento = true)
        {
            return await repo.UnicoAsync(predicado);
            
        }
        public async Task<bool> Existe(Expression<Func<TareaEnDemanda, bool>> predicado)
        {
            return await repo.UnicoAsync(predicado) != null; ;
        }

        public async Task<IPaginado<TareaEnDemanda>> ObtenerPaginadoAsync(Consulta Query, Func<IQueryable<TareaEnDemanda>, IIncludableQueryable<TareaEnDemanda, object>> include = null, bool disableTracking = true, CancellationToken cancellationToken = default)
        {
            
            Query = GetDefaultQuery(Query);
            var respuesta = await this.repo.ObtenerPaginadoAsync(Query, null);
            respuesta.Elementos.ToList();
            return respuesta;
        }

        public async Task<List<TareaEnDemanda>> ObtenerAsync(Expression<Func<TareaEnDemanda, bool>> predicado)
        {
            return await  this.repo.ObtenerAsync(predicado);
        }


        #region NoImplementados

        public Task<IEnumerable<TareaEnDemanda>> CrearAsync(params TareaEnDemanda[] entities)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<TareaEnDemanda>> CrearAsync(IEnumerable<TareaEnDemanda> entities, CancellationToken cancellationToken = default)
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


           public Task<List<TareaEnDemanda>> ObtenerAsync(string SqlCommand)
        {
            throw new NotImplementedException();
        }

        public Task<IPaginado<TareaEnDemanda>> ObtenerPaginadoAsync(Expression<Func<TareaEnDemanda, bool>> predicate = null, Func<IQueryable<TareaEnDemanda>, IOrderedQueryable<TareaEnDemanda>> orderBy = null, Func<IQueryable<TareaEnDemanda>, IIncludableQueryable<TareaEnDemanda, object>> include = null, int index = 0, int size = 20, bool disableTracking = true, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }


        public Task<IEnumerable<string>> Restaurar(string[] ids)
        {
            throw new NotImplementedException();
        }

        public async Task ActualizarAsync(TareaEnDemanda entity)
        {
            throw new NotImplementedException();

        }

        public async Task<List<TareaEnDemanda>> TareasPendientesUsuario(string UsuarioId, string DominioId, string TenantId)
        {
            var tareas = await this.UDT.Context.TareasEnDemanda.Where(x => x.UsuarioId == UsuarioId && x.DominioId == DominioId
            && x.TenantId == TenantId && x.Completada == false).ToListAsync();

            return tareas;
        }

        public async Task<bool> EliminaTareaUsuario(string UsuarioId, string DominioId, string TenantId, Guid TareaId)
        {
            var tarea = await this.UDT.Context.TareasEnDemanda.Where(x => x.UsuarioId == UsuarioId && x.DominioId == DominioId
            && x.TenantId == TenantId && x.Id == TareaId).SingleOrDefaultAsync();

            if (tarea != null)
            {
                // si la tarea ya ha sido completada os e encuentra en ejecución no es eliminada para que el ciclo de recolección
                // de basura se haga cargo del contenido generado
                if (
                    (tarea.Completada && tarea.Estado == Infraestructura.Comun.Tareas.EstadoTarea.Finalizada)
                    || tarea.Estado == Infraestructura.Comun.Tareas.EstadoTarea.Enejecucion)
                {
                    tarea.UsuarioId = "ELIMINADA";
                    tarea.FechaCaducidad = DateTime.UtcNow;
                    this.UDT.Context.Entry(tarea).State = EntityState.Modified;
                    await this.UDT.Context.SaveChangesAsync();
                }  
                else
                {
                    this.UDT.Context.Entry(tarea).State = EntityState.Deleted;
                    await this.UDT.Context.SaveChangesAsync();
                }
               
            }
            return true;
        }


        #endregion
    }
}
