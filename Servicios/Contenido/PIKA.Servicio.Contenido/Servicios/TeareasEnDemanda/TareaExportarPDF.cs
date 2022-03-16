using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PIKA.Infraestructura.Comun;
using PIKA.Infraestructura.Comun.Tareas;
using PIKA.Modelo.Contenido;
using PIKA.Servicio.Contenido.ElasticSearch;
using PIKA.Servicio.Contenido.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PIKA.Servicio.Contenido.Servicios.TareasAutomaticas
{
    public class InputPayloadTareaExportarPDF {
        public string ElementoId { get; set; }
        public int PorcentajePorcientoEscala { get; set; }
    }

    public class OtputPayloadTareaExportarPDF
    {
        public string RutaPDF { get; set; }
        public string NombreElemento { get; set; }

    }

    public class TareaExportarPDF : IInstanciaTareaBackground
    {


        private readonly ILogger<TareaExportarPDF> _logger;
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

        public TareaExportarPDF(
        string DominioId,
        string Id,
        string TokenSeguimiento,
        ILogger<TareaExportarPDF> logger,
        IConfiguration configuracion,
        IServicioVolumen volumenes,
        IServicioElemento elementos,
        IRepoContenidoElasticSearch repoElastic,
        IOptions<ConfiguracionServidor> opciones,
        CancellationToken stoppingToken)
        {
            _opciones = opciones;
            _logger = logger;
            _configuracion = configuracion;
            _volumenes = volumenes;
            _repoElastic = repoElastic;
            _elementos = elementos;

            this.Id = Id;
            this.stoppingToken = stoppingToken;
            this.DominioId = DominioId;
            this.TokenSeguimiento = TokenSeguimiento;
            

            resultado = new ResultadoTareaBackground(DominioId) { PayloadOutput =null,  Error = null, Exito = false, Id = Id, SegundosDuracion = 0 };
        }


        public async Task<ResultadoTareaBackground> EjecutarTarea(string InputPayload)
        {
            InputPayloadTareaExportarPDF input = null;
            try
            {
                _logger.LogInformation("Iniciando proceso de generación de PDF");
                inicio = DateTime.UtcNow;

                string errores = "";

                input = System.Text.Json.JsonSerializer.Deserialize<InputPayloadTareaExportarPDF>(InputPayload);

                var elemento = await this._elementos.UnicoAsync(x => x.Id == input.ElementoId);
                if (elemento != null)
                {

                    IGestorES gestor = await _volumenes.ObtienInstanciaGestor(elemento.VolumenId)
                  .ConfigureAwait(false);

                    Modelo.Contenido.Version vElemento = await this._repoElastic.ObtieneVersion(elemento.VersionId).ConfigureAwait(false);
                    if (vElemento == null)
                    {
                       resultado.Error = $"No Content {input.ElementoId}";
                    } else
                    {
                        var archivo = await gestor.ObtienePDF(vElemento, null, input.PorcentajePorcientoEscala);
                        if (archivo != null)
                        {
                            fin = DateTime.UtcNow;
                            resultado.Exito = string.IsNullOrEmpty(errores);
                            resultado.Error = errores;
                            resultado.PayloadOutput = System.Text.Json.JsonSerializer.Serialize( new OtputPayloadTareaExportarPDF() { NombreElemento = elemento.Nombre, RutaPDF = archivo });
                            resultado.SegundosDuracion = (int)((fin - inicio).TotalSeconds);
                            _logger.LogInformation("Finalizando proceso de creación de PDF");
                        }
                        else
                        {
                            errores = $"Error al generar PDF {input.ElementoId}";
                        }
                    }
                   
                } else
                {
                    errores = $"No Localizado {input.ElementoId}";
                }

                
                fin = DateTime.UtcNow;
                resultado.Exito = string.IsNullOrEmpty(errores);
                resultado.Error = errores;
                resultado.SegundosDuracion = (int)((fin - inicio).TotalSeconds);
                _logger.LogInformation($"Finalizando proceso de generación de PDF para {input.ElementoId}");

                OnTareaFinalizada(resultado);

            }
            catch (Exception ex)
            {
                _logger.LogInformation($"Error de proceso de generación de PDF para {input?.ElementoId} {ex}");
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
            if(InputPayload != null && OutputPayload != null)
            {
                try
                {
                    var input = System.Text.Json.JsonSerializer.Deserialize<InputPayloadTareaExportarPDF>(InputPayload);
                    var output = System.Text.Json.JsonSerializer.Deserialize<OtputPayloadTareaExportarPDF>(OutputPayload);
                    var elemento = await this._elementos.UnicoAsync(x => x.Id == input.ElementoId);
                    if (elemento != null)
                    {

                        IGestorES gestor = await _volumenes.ObtienInstanciaGestor(elemento.VolumenId).ConfigureAwait(false);

                        if (gestor != null)
                        {
                            await gestor.Elimina(output.RutaPDF);
                        }
                    }
                    resultado.Exito = true;
                }
                catch (Exception ex)
                {

                    resultado.Exito = false;
                    resultado.Error = ex.ToString();
                }

            }
            return resultado;
        }

        public void Dispose()
        {
         
        }
    }
}
