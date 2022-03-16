using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using PIKA.Modelo.Aplicacion.Tareas;
using PIKA.Servicio.AplicacionPlugin.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using ComunTareas = PIKA.Infraestructura.Comun.Tareas;

namespace PIKA.GD.API.Servicios.TareasAutomaticas
{

    public class TareaEnDemandaBackground : ITareaEnDemandaBackground
    {
        private readonly ILogger _logger;
        private IServicioTareaEnDemanda servicioTarea;
        private IConfiguration configuracion;
        private IServiceProvider serviceProvider;
        private List<TareaEnDemanda> tareas;
        private CancellationToken stoppingToken;
        private readonly object taskLock = new object();
        private int MaxThreads;
        private List<TareaEnEjecucion> TareasEnEjecucion;
        private Dictionary<string, string> Proveedores;
        private int WaitingGaps = 0;
        private int minutoscancelacion;
        private int minutoscaducidad;
        private DateTime VerificacionCaducidad;
        private bool debug = false;

        List<ComunTareas.ProcesadorTareaBackground> procesadores = new List<ComunTareas.ProcesadorTareaBackground>();

        protected class TareaEnEjecucion
        {
            public string Id { get; set; }
            public DateTime Inicio { get; set; }
        }

        #region Inicialización 

        private void CreaDiccionario()
        {
            LogDebug($"Creando diccionario de procesadores de tareas en demanda");
            Proveedores = new Dictionary<string, string>();
            var l = ObtieneTiposTareasEnDemanda();
            foreach (var tipo in l)
            {
                LogDebug($"Adicionando {tipo.FullName}");
                var instancia = (ComunTareas.IProveedorTareasEnDemanda)Activator.CreateInstance(tipo);
                try
                {
                    var lista = instancia.ObtieneTareasEnDemanda();
                    foreach (var item in lista)
                    {
                        LogDebug($"{item.Id} {tipo.FullName}");
                        Proveedores.Add(item.Id, tipo.FullName);
                    }
                }
                catch (Exception ex)
                {
                    
                }
            }
        }

        public static string ObtieneRutaBin()
        {
            return new FileInfo(System.Reflection.Assembly.GetEntryAssembly().Location).Directory.FullName;
        }

        public static List<Type> ObtieneTiposTareasEnDemanda()
        {
            List<Type> l = new List<Type>();
            string Ruta = ObtieneRutaBin();

            var assemblies = Directory.GetFiles(Ruta, "*.dll",
                new EnumerationOptions() { RecurseSubdirectories = true });

            foreach (var item in assemblies)
            {
                try
                {
                    var assembly = Assembly.LoadFile(item);
                    var Tipos = assembly.GetTypes()
                            .Where(t =>
                            !t.IsAbstract &&
                            typeof(ComunTareas.IProveedorTareasEnDemanda).IsAssignableFrom(t))
                            .ToArray();

                    l.AddRange(Tipos);


                }
                catch (Exception ex)
                {

                }
            }

            return l;
        }

        #endregion


        public TareaEnDemandaBackground(ILogger<TareaAutomaticaBackground> _logger,
            IServicioTareaEnDemanda servicioTarea,
            IConfiguration configuracion,
            IServiceProvider serviceProvider)
        {
            this._logger = _logger;
            this.servicioTarea = servicioTarea;
            this.configuracion = configuracion;
            this.serviceProvider = serviceProvider;
            this.TareasEnEjecucion = new List<TareaEnEjecucion>();
            MaxThreads = 1;


            string maxt =configuracion.GetValue<string>("TareasBackground:endemanda:hilos");
            
            if (!int.TryParse(configuracion.GetValue<string>("TareasBackground:endemanda:minutoscancelacion"), out this.minutoscancelacion))
            {
                this.minutoscancelacion = 15;
            };

            if (!int.TryParse(configuracion.GetValue<string>("TareasBackground:endemanda:hilos"), out this.MaxThreads))
            {
                this.MaxThreads = 1;
            };

            if (!int.TryParse(configuracion.GetValue<string>("TareasBackground:endemanda:minutoscaducidad"), out this.minutoscaducidad))
            {
                this.minutoscaducidad = 60;
            };

            if (!bool.TryParse(configuracion.GetValue<string>("TareasBackground:endemanda:debug"), out this.debug))
            {
                this.debug = false;
            };

            VerificacionCaducidad = DateTime.UtcNow;

            _logger.LogDebug($"Threads tareas en demanda {this.MaxThreads}");
            _logger.LogDebug($"Debug tareas en demanda {this.debug}");
            _logger.LogDebug($"Minutos de cancelacion {this.minutoscancelacion}");
            _logger.LogDebug($"Minutos de verificación caducidad {this.minutoscaducidad}");

            CreaDiccionario();
        }

        private void LogDebug(string Msg)
        {
            if (this.debug) _logger.LogInformation(Msg);
        }

        public async Task DoWork(CancellationToken stoppingToken)
        {
            this.stoppingToken = stoppingToken;

            while (!stoppingToken.IsCancellationRequested)
            {
                await EjecutaTareas(stoppingToken).ConfigureAwait(false);

                int milis = WaitingGaps == 0 ? 500 : WaitingGaps * 1000;

                LogDebug($"Tareas en demanda esperando {milis}ms");

                await Task.Delay(milis, stoppingToken).ConfigureAwait(false);
                EliminaTareasZombie();

                if((DateTime.UtcNow - VerificacionCaducidad).TotalMinutes> minutoscaducidad)
                {
                    // Elimina la posibilidad lanzar más de un proceso
                    VerificacionCaducidad = DateTime.Now.AddYears(1);

                    await CaducaTareas(stoppingToken).ConfigureAwait(false);
                } 
            }
        }

        private async Task CaducaTareas(CancellationToken stoppingToken)
        {
            LogDebug($"Ejecutando Búsqueda Tareas On Demand caducas");
            if (!stoppingToken.IsCancellationRequested)
            {
                var tareas = (await servicioTarea.ObtenerAsync(x => x.Completada == true).ConfigureAwait(false)).ToList();
                LogDebug($"Tareas On Demand caducas encontradas {tareas.Count}");
                foreach (var tarea in tareas)
                {

                    if(DateTime.UtcNow > tarea.FechaCaducidad)
                    {
                        LogDebug($"Eliminando Tarea On Demand caduca {tarea.Id}");
                        var (programada, procesador, errror) = ProgramaTarea(tarea);
                        if (programada)
                        {
                            _ = Task.Run(() => procesador.Instancia.CaducarTarea(tarea.InputPayload, tarea.OutputPayload));

                        }
                        await servicioTarea.EliminarTarea(tarea.Id).ConfigureAwait(false);
                    }
                    
                    if (stoppingToken.IsCancellationRequested)
                    {
                        break;
                    }
                }
            }

            VerificacionCaducidad = DateTime.UtcNow;
        }

        private void EliminaTareasZombie()
        {
            lock (taskLock)
            {
                var tareas = TareasEnEjecucion.Where(x => x.Inicio.AddMinutes(60) < DateTime.UtcNow  ).ToList();
                if (tareas.Count>0)
                {
                    foreach(var item in tareas)
                    {
                        TareasEnEjecucion.Remove(item);
                    }
                }
            }
        }

        private async Task EjecutaTareas(CancellationToken stoppingToken)
        {
            LogDebug($"Ejecutando Búsqueda Tareas On Demand");
            if (!stoppingToken.IsCancellationRequested)
            {
                LogDebug($"No hay taoken de cancelación");
                if (this.TareasEnEjecucion.Count < MaxThreads)
                {
                    LogDebug($"Existen {(MaxThreads- this.TareasEnEjecucion.Count)} hilos disponibles");

                    tareas = (await servicioTarea.ObtenerAsync(x => x.Completada == false && x.Estado == ComunTareas.EstadoTarea.Habilidata).ConfigureAwait(false)).ToList();
                    LogDebug($"Ejecutando {tareas.Count} tareas");
                    if (tareas.Count > 0)
                    {
                        foreach (var tarea in tareas)
                        {
                            LogDebug($"Ejecutando {tarea.Id}@{tarea.TareaProcesoId}");
                            var (programada, procesador, errror) = ProgramaTarea(tarea);
                            if (programada)
                            {
                                await servicioTarea.ActualizaEstadoTarea(tarea.Id, ComunTareas.EstadoTarea.Enejecucion).ConfigureAwait(false);
                                lock (taskLock)
                                {
                                    // Añade a la cola para mantener el número de threads
                                    this.TareasEnEjecucion.Add(new TareaEnEjecucion() { Id = tarea.Id.ToString(), Inicio = DateTime.UtcNow });
                                }
                                _ = Task.Run(() => procesador.Instancia.EjecutarTarea(tarea.InputPayload));

                                if (this.TareasEnEjecucion.Count >= MaxThreads)
                                {
                                    break;
                                }
                            }
                            else
                            {
                                tarea.Error = errror;
                                tarea.Completada = true;
                                await servicioTarea.CompletarTarea(tarea.Id, false, null, errror).ConfigureAwait(false);
                                LogDebug($"Error al programar la tarea {tarea.TareaProcesoId} {errror}");
                            }


                        }
                    } else
                    {
                        WaitingGaps++;
                        if(WaitingGaps>14)
                        {
                            WaitingGaps = 15;
                        }
                    }
                }
            }
        }



        private (bool programada, ComunTareas.ProcesadorTareaBackground procesador, string errror) ProgramaTarea(TareaEnDemanda t)
        {
            bool programada = false;
            string errror = null;
            ComunTareas.ProcesadorTareaBackground procesador = new ComunTareas.ProcesadorTareaBackground()
            {
                Id = t.Id.ToString(),
                Index = 0,
                EnEjecucion = false,
                SiguienteEjecucion = null,
                TokenSeguimiento = Guid.NewGuid().ToString()
            };

            try
            {
                    var objectType = Type.GetType(t.NombreEnsamblado);
                    var instancia = (ComunTareas.IProveedorTareasEnDemanda)Activator.CreateInstance(objectType);
                    procesador.Instancia = instancia.InstanciaTarea(t.DominioId, t.TareaProcesoId, t.Id.ToString(), procesador.TokenSeguimiento, configuracion, serviceProvider, stoppingToken);

                    if (procesador.Instancia != null)
                    {
                        procesador.Instancia.TareaFinalizada += Instancia_TareaFinalizada;
                        procesador.EnEjecucion = true;
                        programada = true;
                        this.procesadores.Add(procesador);
                    }
                    else
                    {
                        errror = "No pudo crearse la instancia de la tarea";
                    }
                }
                catch (Exception ex)
                {
                    errror = ex.Message;
                }

            return (programada, procesador, errror);
        }


        private void Instancia_TareaFinalizada(object sender, EventArgs e)
        {
            ComunTareas.TareaFinalizadaEventArgs et = (ComunTareas.TareaFinalizadaEventArgs)e;

            try
            {

                lock (taskLock)
                {
                    LogDebug($"Fin de proceso {et.Resultado.Id} {et.TokenSeguimiento} {et.Resultado.Exito}");
                    Task.Run(() => servicioTarea.CompletarTarea(Guid.Parse(et.Resultado.Id), et.Resultado.Exito, et.Resultado.PayloadOutput, et.Resultado.Error));

                    var procesador = this.procesadores.Where(x => x.Id == et.Resultado.Id).SingleOrDefault();
                    if(procesador != null)
                    {
                        procesador.Instancia.TareaFinalizada -= Instancia_TareaFinalizada;
                        procesador.Dispose();
                        procesador = null;
                    }

                    var t = TareasEnEjecucion.Where(x => x.Id == et.Resultado.Id).FirstOrDefault();
                    if(t!= null)
                    {
                        this.TareasEnEjecucion.Remove(t);
                    }
                }
            }
            catch (Exception ex)
            {
                LogDebug($"{ex}");
            }

            this.WaitingGaps = 0;
        }

    }
}
