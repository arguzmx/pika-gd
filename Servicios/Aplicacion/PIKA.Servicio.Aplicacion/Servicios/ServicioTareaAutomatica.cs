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
    public class ServicioTareaAutomatica : ContextoServicioAplicacion
        , IServicioInyectable, IServicioTareaAutomatica
    {

        private UnidadDeTrabajo<DbContextAplicacion> UDT;
        private IRepositorioAsync<TareaAutomatica> repo;

        private const string DEFAULT_SORT_COL = "Nombre";
        private const string DEFAULT_SORT_DIRECTION = "asc";

        public UsuarioAPI usuario { get; set; }
        public PermisoAplicacion permisos { get; set; }
        
        public ServicioTareaAutomatica(
            IProveedorOpcionesContexto<DbContextAplicacion> proveedorOpciones,
            ILogger<ServicioLog> Logger,
            IServicioCache servicioCache) : base(proveedorOpciones, Logger, servicioCache)

        {
            this.UDT = new UnidadDeTrabajo<DbContextAplicacion>(contexto);
            this.repo = UDT.ObtenerRepositoryAsync(new QueryComposer<TareaAutomatica>());
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

        private (bool valido, string error) PeriodoValido(TareaAutomatica d)
        {

            switch (d.Periodo)
            {
                case Infraestructura.Comun.Tareas.PeriodoProgramacion.Hora:
                    if (!d.HoraEjecucion.HasValue)
                    {
                        return (false, "Hora de ejecución no válida");
                    }
                    if (d.Intervalo<=0)
                    {
                        return (false, "El intervalo de horas debe ser mayor a 0");
                    }
                    break;

                case Infraestructura.Comun.Tareas.PeriodoProgramacion.Unico:
                    if (!d.FechaHoraEjecucion.HasValue)
                    {
                        return (false, "Debe especificar una fecha y hora de ejecución");
                    }
                    if (d.FechaHoraEjecucion <= DateTime.Now)
                    {
                        return (false, "La fecha debe ser superior al día de hoy");
                    }
                    break;

                case Infraestructura.Comun.Tareas.PeriodoProgramacion.DiaSemana:
                    if (!d.HoraEjecucion.HasValue)
                    {
                        return (false, "Hora de ejecución no válida");
                    }
                    if (!d.DiaSemana.HasValue)
                    {
                        return (false, "Día de la semana no válido");
                    }
                    break;

                case Infraestructura.Comun.Tareas.PeriodoProgramacion.DiaMes:
                    if (!d.HoraEjecucion.HasValue)
                    {
                        return (false, "Hora de ejecución no válida");
                    }
                    if (!d.DiaMes.HasValue)
                    {
                        return (false, "Día de la semana no válido");
                    } else
                    {
                        if(d.DiaMes<=0 || d.DiaMes >= 31)
                        {
                            return (false, "Día del mes no válido");
                        }
                    }
                    break;

                case Infraestructura.Comun.Tareas.PeriodoProgramacion.Diario:
                    if (!d.HoraEjecucion.HasValue)
                    {
                        return (false, "Hora de ejecución no válida");
                    }
                    if (!d.FechaHoraEjecucion.HasValue)
                    {
                        return (false, "Debe especificar una hora de ejecución");
                    }
                    break;

            }

            return (true, "");
        }

        public async Task<TareaAutomatica> CrearAsync(TareaAutomatica entity, CancellationToken cancellationToken = default)
        {
            var valido = PeriodoValido(entity);
            if (!valido.valido)
            {
                throw new ExDatosNoValidos(valido.error);
            }

            if(await this.UDT.Context.TareasAutomaticas.AnyAsync(x=>x.Id == entity.Id && x.OrigenId == entity.OrigenId))
            {
                throw new ExElementoExistente($"{entity.Nombre}@{entity.OrigenId}");
            }

            this.UDT.Context.TareasAutomaticas.Add(entity);
            this.UDT.SaveChanges();
            return entity.EstableceEtiquetas(this.usuario.gmtOffset);
        }


        public async Task ActualizarAsync(TareaAutomatica entity)
        {
            var valido = PeriodoValido(entity);
            if (!valido.valido)
            {
                throw new ExDatosNoValidos(valido.error);
            }

            var tarea = await this.UDT.Context.TareasAutomaticas.Where(x => x.Id == entity.Id && x.OrigenId == entity.OrigenId).FirstOrDefaultAsync(); 
            if(tarea == null)
            {
                throw new EXNoEncontrado($"{entity.Nombre}@{entity.OrigenId}");
            }

            tarea.Periodo = entity.Periodo;
            switch (entity.Periodo)
            {
                case Infraestructura.Comun.Tareas.PeriodoProgramacion.Unico:
                    tarea.FechaHoraEjecucion = entity.FechaHoraEjecucion;
                    tarea.Intervalo = 0;
                    break;

                case Infraestructura.Comun.Tareas.PeriodoProgramacion.DiaSemana:
                    tarea.FechaHoraEjecucion = new DateTime(2000,1,1, entity.HoraEjecucion.Value.Hour, entity.HoraEjecucion.Value.Minute, 0);
                    tarea.Intervalo = entity.DiaSemana.Value;
                    break;

                case Infraestructura.Comun.Tareas.PeriodoProgramacion.Hora:
                    tarea.FechaHoraEjecucion = new DateTime(2000, 1, 1, entity.HoraEjecucion.Value.Hour, entity.HoraEjecucion.Value.Minute, 0);
                    tarea.Intervalo = entity.Intervalo;
                    break;

                case Infraestructura.Comun.Tareas.PeriodoProgramacion.DiaMes:
                    tarea.FechaHoraEjecucion = new DateTime(2000, 1, 1, entity.HoraEjecucion.Value.Hour, entity.HoraEjecucion.Value.Minute, 0);
                    tarea.Intervalo = entity.DiaMes;
                    break;

                case Infraestructura.Comun.Tareas.PeriodoProgramacion.Diario:
                    tarea.FechaHoraEjecucion = new DateTime(2000, 1, 1, entity.HoraEjecucion.Value.Hour, entity.HoraEjecucion.Value.Minute, 0);
                    tarea.Intervalo = 0;
                    break;

            }

            this.UDT.Context.SaveChanges();
        }

        public async Task<TareaAutomatica> UnicoAsync(Expression<Func<TareaAutomatica, bool>> predicado = null, Func<IQueryable<TareaAutomatica>, IOrderedQueryable<TareaAutomatica>> ordenarPor = null, Func<IQueryable<TareaAutomatica>, IIncludableQueryable<TareaAutomatica, object>> incluir = null, bool inhabilitarSegumiento = true)
        {
            return (await repo.UnicoAsync(predicado)).EstableceEtiquetas(this.usuario.gmtOffset);
            
        }
        public async Task<bool> Existe(Expression<Func<TareaAutomatica, bool>> predicado)
        {
            return await repo.UnicoAsync(predicado) != null; ;
        }

        public async Task<IPaginado<TareaAutomatica>> ObtenerPaginadoAsync(Consulta Query, Func<IQueryable<TareaAutomatica>, IIncludableQueryable<TareaAutomatica, object>> include = null, bool disableTracking = true, CancellationToken cancellationToken = default)
        {
            
            Query = GetDefaultQuery(Query);
            var respuesta = await this.repo.ObtenerPaginadoAsync(Query, null);
            respuesta.Elementos.ToList().ForEach(o => {
                o.EstableceEtiquetas(this.usuario.gmtOffset);
            });
            return respuesta;
        }


        #region NoImplementados

        public Task<IEnumerable<TareaAutomatica>> CrearAsync(params TareaAutomatica[] entities)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<TareaAutomatica>> CrearAsync(IEnumerable<TareaAutomatica> entities, CancellationToken cancellationToken = default)
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

        

        public Task<List<TareaAutomatica>> ObtenerAsync(Expression<Func<TareaAutomatica, bool>> predicado)
        {
            throw new NotImplementedException();
        }

        public Task<List<TareaAutomatica>> ObtenerAsync(string SqlCommand)
        {
            throw new NotImplementedException();
        }

        public Task<IPaginado<TareaAutomatica>> ObtenerPaginadoAsync(Expression<Func<TareaAutomatica, bool>> predicate = null, Func<IQueryable<TareaAutomatica>, IOrderedQueryable<TareaAutomatica>> orderBy = null, Func<IQueryable<TareaAutomatica>, IIncludableQueryable<TareaAutomatica, object>> include = null, int index = 0, int size = 20, bool disableTracking = true, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }


        public Task<IEnumerable<string>> Restaurar(string[] ids)
        {
            throw new NotImplementedException();
        }

 

        #endregion
    }
}
