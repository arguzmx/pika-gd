using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using PIKA.Modelo.Aplicacion.Tareas;
using PIKA.Servicio.AplicacionPlugin.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ComunTareas = PIKA.Infraestructura.Comun.Tareas;

namespace PIKA.GD.API.Servicios.TareasAutomaticas
{
    public class TareaAutomaticaBackground : ITareaAutomaticaBackgroud
    {
        private readonly ILogger _logger;
        private IServicioTareaAutomatica servicioTarea;
        private IConfiguration configuracion;
        private IServiceProvider serviceProvider;
        private List<TareaAutomatica> tareas;
        private List<ComunTareas.ProcesadorTareaAutomatica> InstanciaTareas;
        private CancellationToken stoppingToken;
        private readonly object taskLock = new object();

        public TareaAutomaticaBackground(ILogger<TareaAutomaticaBackground> _logger,
            IServicioTareaAutomatica servicioTarea,
            IConfiguration configuracion,
            IServiceProvider serviceProvider)
        {
            this._logger = _logger;
            this.servicioTarea = servicioTarea;
            this.configuracion = configuracion;
            this.serviceProvider = serviceProvider;
        }

        public async Task DoWork(CancellationToken stoppingToken)
        {
            this.stoppingToken = stoppingToken;
            InstanciaTareas = new List<ComunTareas.ProcesadorTareaAutomatica>();
            await Inicializacion().ConfigureAwait(false);

            while (!stoppingToken.IsCancellationRequested)
            {
                await EjecutaTareas(stoppingToken).ConfigureAwait(false);
                await Task.Delay(60000, stoppingToken).ConfigureAwait(false);
            }
        }

        private async Task Inicializacion()
        {
            tareas = (await servicioTarea.ObtenerAsync(x => x.Estado != ComunTareas.EstadoTarea.Pausada).ConfigureAwait(false)).ToList();
            foreach (var tarea in tareas)
            {
                if (tarea.Estado == ComunTareas.EstadoTarea.Enejecucion)
                {
                    tarea.Estado = ComunTareas.EstadoTarea.Habilidata;
                }

                if (!tarea.ProximaEjecucion.HasValue || (tarea.ProximaEjecucion.HasValue && tarea.ProximaEjecucion.Value < DateTime.UtcNow))
                {
                    tarea.ProximaEjecucion = tarea.SiguienteFechaEjecucion();
                }
                await servicioTarea.ActualizaEjecucion(tarea).ConfigureAwait(false);
            }
        }
        
        private async Task EjecutaTareas(CancellationToken stoppingToken)
        {
            if (!stoppingToken.IsCancellationRequested)
            {
                tareas = (await servicioTarea.ObtenerAsync(x => x.Estado == ComunTareas.EstadoTarea.Habilidata).ConfigureAwait(false)).ToList();
                foreach(var tarea in tareas)
                {

                    // Excluye tareas de ejecución única que ya hayan sido ejecutadas
                    if(tarea.Periodo == ComunTareas.PeriodoProgramacion.Unico && tarea.UltimaEjecucion.HasValue)
                    {
                        break;
                    } 

                    if(!tarea.ProximaEjecucion.HasValue)
                    {
                        tarea.ProximaEjecucion = tarea.SiguienteFechaEjecucion();
                        await servicioTarea.ActualizaEjecucion(tarea).ConfigureAwait(false);
                    }

                    if (tarea.ProximaEjecucion.HasValue)
                    {
                        if(tarea.ProximaEjecucion < DateTime.UtcNow)
                        {
                            var procesador = InstanciaTareas.FirstOrDefault(x => x.Id == tarea.Id);
                            if (procesador != null)
                            {
                                _logger.LogInformation($"Reprogramando {tarea.Nombre}");
                                if (!procesador.EnEjecucion)
                                {
                                    tarea.Estado = ComunTareas.EstadoTarea.Enejecucion;
                                    await servicioTarea.ActualizaEjecucion(tarea).ConfigureAwait(false);
                                    _ = Task.Run(() => procesador.Instancia.EjecutarTarea());
                                }
                                else
                                {
                                    _logger.LogInformation($"Error al programar la tarea {tarea.Nombre} aún se encuentra en ejecución");
                                }

                            }
                            else
                            {
                                _logger.LogInformation($"Programando {tarea.Nombre}");
                                var (programada, errror) = ProgramaTarea(tarea);
                                if (programada)
                                {
                                    procesador = InstanciaTareas.First(x => x.Id == tarea.Id);
                                    tarea.Estado = ComunTareas.EstadoTarea.Enejecucion;
                                    await servicioTarea.ActualizaEjecucion(tarea).ConfigureAwait(false);
                                    _ = Task.Run(() => procesador.Instancia.EjecutarTarea());
                                }
                                else
                                {
                                    tarea.Estado = ComunTareas.EstadoTarea.ErrorConfiguracion;
                                    await servicioTarea.ActualizaEjecucion(tarea).ConfigureAwait(false);
                                    _logger.LogInformation($"Error al programar la tarea {tarea.Nombre} {errror}");
                                }
                            }

                        } else
                        {
                            _logger.LogInformation($"Intervalo espera {tarea.Nombre} {(DateTime.UtcNow - tarea.ProximaEjecucion.Value).TotalSeconds}");
                        }

                    } else
                    {
                        _logger.LogInformation($"No hay fecha de ejecución para {tarea.Nombre}");
                    }

                }

            }
        }

   

        private (bool programada, string errror) ProgramaTarea(TareaAutomatica t)
        {
            bool programada = false;
            string errror = null;
            lock (taskLock)
            {
                try
                {
                    ComunTareas.ProcesadorTareaAutomatica procesador = new ComunTareas.ProcesadorTareaAutomatica()
                    {
                        Id = t.Id,
                        Index = 0,
                        EnEjecucion = false,
                        SiguienteEjecucion = null,
                        TokenSeguimiento = Guid.NewGuid().ToString()
                    };

                    var objectType = Type.GetType(t.Ensamblado);
                    var instancia = (ComunTareas.IProveedorTareasAutomaticas)Activator.CreateInstance(objectType);
                    procesador.Instancia = instancia.InstanciaTarea(t.OrigenId, t.Id, procesador.TokenSeguimiento, configuracion, serviceProvider, stoppingToken);
                    if (procesador.Instancia != null)
                    {
                        procesador.Instancia.TareaFinalizada += Instancia_TareaFinalizada;
                        procesador.EnEjecucion = true;
                        InstanciaTareas.Add(procesador);
                        programada = true;
                    } else
                    {
                        errror = "No pudo crearse la instancia de la tarea";
                    }
                }
                catch (Exception ex)
                {
                    errror = ex.Message;
                }
                
            }
            return (programada, errror);
        }

  
        private void Instancia_TareaFinalizada(object sender, EventArgs e)
        {
            ComunTareas.TareaFinalizadaEventArgs et = (ComunTareas.TareaFinalizadaEventArgs)e;
            lock (taskLock)
            {
                _logger.LogInformation($"Fin de proceso {et.Resultado.Id} {et.TokenSeguimiento} {et.Resultado.Exito}");
                var instancia = InstanciaTareas.Where(x => x.Id == et.Resultado.Id).SingleOrDefault();
                var tarea = this.tareas.Where(x => x.Id == et.Resultado.Id).SingleOrDefault();
                if (instancia != null && tarea != null)
                {
                    instancia.EnEjecucion = false;

                    if(tarea.Periodo!= ComunTareas.PeriodoProgramacion.Unico)
                    {
                        if (tarea.TareaEjecucionContinua)
                        {
                            _logger.LogInformation($"Reprogramando tarea continua {tarea.Id}");
                            instancia.SiguienteEjecucion = DateTime.UtcNow.AddMinutes(tarea.TareaEjecucionContinuaMinutos);
                        }
                        else
                        {
                            _logger.LogInformation($"Reprogramando tarea no continua {tarea.Id}");
                            instancia.SiguienteEjecucion = tarea.SiguienteFechaEjecucion(DateTime.UtcNow);
                        }
                    }

                    tarea.Estado = ComunTareas.EstadoTarea.Habilidata;
                    tarea.CodigoError = et.Resultado.Error;
                    tarea.UltimaEjecucion = DateTime.UtcNow;
                    tarea.Exito = et.Resultado.Exito;
                    tarea.ProximaEjecucion = instancia.SiguienteEjecucion;
                    Task.Run(() => servicioTarea.ActualizaEjecucion(tarea));
                }
            }
        }

    }
}
