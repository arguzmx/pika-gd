using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PIKA.Infraestructura.Comun;
using PIKA.Infraestructura.Comun.Tareas;
using PIKA.Servicio.GestionDocumental.Interfaces;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace PIKA.Servicio.GestionDocumental.TareasAutomaticas
{
    public class TareaAutomaticaEstadisticaGuiaSimple : IInstanciaTareaBackground
    {


        private readonly ILogger<TareaAutomaticaEstadisticaGuiaSimple> _logger;
        private readonly IConfiguration _configuracion;
        private readonly IServicioArchivo _archivos;
        private readonly IServicioEstadisticaClasificacionAcervo _estadisticas;
        private readonly IOptions<ConfiguracionServidor> _opciones;
        private readonly string Id;
        private readonly string DominioId;
        private readonly string TokenSeguimiento;
        private CancellationToken stoppingToken;
        public event EventHandler TareaFinalizada;
        private ResultadoTareaBackground resultado;
        private DateTime inicio;
        private DateTime fin;

        protected virtual void OnTareaFinalizada(ResultadoTareaBackground t)
        {
            TareaFinalizadaEventArgs e = new TareaFinalizadaEventArgs()
            {
                TokenSeguimiento = this.TokenSeguimiento,
                Resultado = t
            };
            TareaFinalizada?.Invoke(this, e);
        }

        public TareaAutomaticaEstadisticaGuiaSimple(
        string DominioId,
        string Id,
        string TokenSeguimiento,
        ILogger<TareaAutomaticaEstadisticaGuiaSimple> logger,
        IConfiguration configuracion,
        IServicioArchivo archivos,
        IServicioEstadisticaClasificacionAcervo estadisticas,
        IOptions<ConfiguracionServidor> opciones,
        CancellationToken stoppingToken)
        {
            _opciones = opciones;
            _logger = logger;
            _configuracion = configuracion;
            _archivos = archivos;
            _estadisticas = estadisticas;
            this.Id = Id;
            this.stoppingToken = stoppingToken;
            this.DominioId = DominioId;
            this.TokenSeguimiento = TokenSeguimiento;

            resultado = new ResultadoTareaBackground(DominioId) { Error = null, Exito = false, Id = Id, SegundosDuracion = 0 };
        }


        public async Task<ResultadoTareaBackground> EjecutarTarea(string inputPayload = null)
        {
            try
            {
                _logger.LogInformation("Iniciando proceso de actualización estadísticas de archivo");
                inicio = DateTime.UtcNow;

                string errores = "";

                var archivos = await _archivos.ObtenerAsync(x => x.Eliminada == false);
                foreach(var archivo in archivos)
                {
                    _logger.LogInformation($"Procesando archivo {archivo.Nombre}");
                    if (stoppingToken.IsCancellationRequested)
                    {
                        break;
                    }

                    try
                    {
                        await _estadisticas.ActualizarConteos(archivo.Id);

                    }
                    catch (Exception ex)
                    {
                        errores += $"Error estadísticas {archivo.Id} {archivo.Nombre} {ex.Message}\r\n";
                        _logger.LogError($"Error estadísticas {archivo.Id} {archivo.Nombre} {ex.Message}");
                        _logger.LogError($"{ex}");
                    }
                    

                }

                fin = DateTime.UtcNow;
                resultado.Exito = string.IsNullOrEmpty(errores);
                resultado.Error = errores;
                resultado.SegundosDuracion = (int)((fin - inicio).TotalSeconds);
                _logger.LogInformation("Finalizando proceso de actualización estadísticas de archivo");

                OnTareaFinalizada(resultado);

            }
            catch (Exception ex)
            {
                _logger.LogInformation($"Error de proceso de actualización estadísticas de archivo {ex}");
                fin = DateTime.UtcNow;
                resultado.Exito = false;
                resultado.Error = ex.Message;
                resultado.SegundosDuracion = (int)((fin - inicio).TotalSeconds);
                OnTareaFinalizada(resultado);
            }

            return resultado;
        }

        public async Task<ResultadoTareaBackground> CaducarTarea(string InputPayload = null, string OutputPayload = null)
        {
            await Task.Delay(10);
            return null;
        }
    }
}
