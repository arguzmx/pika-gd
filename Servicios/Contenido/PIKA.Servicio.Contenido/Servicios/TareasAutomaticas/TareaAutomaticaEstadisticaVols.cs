using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PIKA.Infraestructura.Comun;
using PIKA.Infraestructura.Comun.Tareas;
using PIKA.Servicio.Contenido.ElasticSearch;
using PIKA.Servicio.Contenido.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PIKA.Servicio.Contenido.Servicios.TareasAutomaticas
{
    public class TareaAutomaticaEstadisticaVols : IInstanciaTareaAutomatica
    {


        private readonly ILogger<TareaAutomaticaEstadisticaVols> _logger;
        private readonly IConfiguration _configuracion;
        private readonly IServicioVolumen _volumenes;
        private readonly IServicioElemento _elementos;
        private readonly IRepoContenidoElasticSearch _repoElastic;
        private readonly IOptions<ConfiguracionServidor> _opciones;
        private readonly string Id;
        private readonly string DominioId;
        private readonly string TokenSeguimiento;
        private CancellationToken stoppingToken;
        public event EventHandler TareaFinalizada;
        private ResultadoTareaAutomatica resultado;
        private DateTime inicio;
        private DateTime fin;

        protected virtual void OnTareaFinalizada(ResultadoTareaAutomatica t)
        {
            TareaFinalizadaEventArgs e = new TareaFinalizadaEventArgs()
            {
                TokenSeguimiento = this.TokenSeguimiento,
                Resultado = t
            };
            TareaFinalizada?.Invoke(this, e);
        }

        public TareaAutomaticaEstadisticaVols(
        string DominioId,
        string Id,
        string TokenSeguimiento,
        ILogger<TareaAutomaticaEstadisticaVols> logger,
        IConfiguration configuracion,
        IServicioVolumen volumenes,
        IRepoContenidoElasticSearch repoElastic,
        IOptions<ConfiguracionServidor> opciones,
        CancellationToken stoppingToken)
        {
            _opciones = opciones;
            _logger = logger;
            _configuracion = configuracion;
            _volumenes = volumenes;
            _repoElastic = repoElastic;
            this.Id = Id;
            this.stoppingToken = stoppingToken;
            this.DominioId = DominioId;
            this.TokenSeguimiento = TokenSeguimiento;

            resultado = new ResultadoTareaAutomatica(DominioId) { Error = null, Exito = false, Id = Id, SegundosDuracion = 0 };
        }


        public async Task<ResultadoTareaAutomatica> EjecutarTarea()
        {
            try
            {
                _logger.LogInformation("Iniciando proceso de actualización estadísticas de volumen");
                inicio = DateTime.UtcNow;

                string errores = "";

                var vols = await _volumenes.ObtenerAsync(x => x.Eliminada == false);
                foreach(var vol in vols)
                {
                    _logger.LogInformation($"Procesando volumen {vol.Nombre}");
                    if (stoppingToken.IsCancellationRequested)
                    {
                        break;
                    }

                    try
                    {
                        var stats = await _repoElastic.ObtieneEstadisticasVolumen(vol.Id);
                        vol.CanidadElementos = stats.ConteoElementos;
                        vol.CanidadPartes = stats.ConteoPartes;
                        vol.Tamano = stats.TamanoBytes;
                        await _volumenes.ActualizaEstadisticas(stats, vol.Id);

                    }
                    catch (Exception ex)
                    {
                        errores += $"Error estadísticas {vol.Id} {vol.Nombre} {ex.Message}\r\n";
                        _logger.LogError($"Error estadísticas {vol.Id} {vol.Nombre} {ex.Message}");
                        _logger.LogError($"{ex}");
                    }
                    

                }

                fin = DateTime.UtcNow;
                resultado.Exito = string.IsNullOrEmpty(errores);
                resultado.Error = errores;
                resultado.SegundosDuracion = (int)((fin - inicio).TotalSeconds);
                _logger.LogInformation("Finalizando proceso de actualización estadísticas de volumen");

                OnTareaFinalizada(resultado);

            }
            catch (Exception ex)
            {
                _logger.LogInformation($"Error de proceso de actualización estadísticas de volumen {ex}");
                fin = DateTime.UtcNow;
                resultado.Exito = false;
                resultado.Error = ex.Message;
                resultado.SegundosDuracion = (int)((fin - inicio).TotalSeconds);
                OnTareaFinalizada(resultado);
            }

            return resultado;
        }


    }
}
