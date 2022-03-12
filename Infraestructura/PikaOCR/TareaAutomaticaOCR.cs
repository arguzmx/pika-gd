using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PIKA.Infraestructura.Comun;
using PIKA.Infraestructura.Comun.Tareas;
using PIKA.Modelo.Contenido;
using PIKA.Servicio.Contenido.ElasticSearch;
using PIKA.Servicio.Contenido.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PikaOCR
{
    public class TareaAutomaticaOCR: IInstanciaTareaBackground
    {
   
        private readonly ILogger<TareaAutomaticaOCR> _logger;
        private readonly IConfiguration _configuracion;
        private readonly IServicioVolumen _volumenes;
        private readonly IServicioElemento _elementos;
        private readonly IRepoContenidoElasticSearch _repoElastic;
        private readonly IOptions<ConfiguracionServidor> _opciones;
        private readonly string Id;
        private readonly string DominioId;
        private readonly string TokenSeguimiento;
        private CancellationToken stoppingToken;
        private List<string> extensionesIndexado = new List<string>() {
            ".jpg", ".png", ".tif", ".bpm", ".gif", ".jpeg", ".jfif", ".pdf"
        };

        public event EventHandler TareaFinalizada;
        private ResultadoTareaBackground resultado;
        private DateTime inicio;
        private DateTime fin;
        protected virtual void OnTareaFinalizada(ResultadoTareaBackground t)
        {
            TareaFinalizadaEventArgs e = new TareaFinalizadaEventArgs() {
                TokenSeguimiento = this.TokenSeguimiento,
                Resultado = t
            };
            TareaFinalizada?.Invoke(this, e);
        }


        public TareaAutomaticaOCR(
             string DominioId,
             string Id,
             string TokenSeguimiento,
             ILogger<TareaAutomaticaOCR> logger,
             IConfiguration configuracion,
             IServicioVolumen volumenes,
             IRepoContenidoElasticSearch repoElastic,
             IServicioElemento elementos,
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
            this.DominioId  = DominioId;
            this.TokenSeguimiento = TokenSeguimiento;

            resultado = new ResultadoTareaBackground(DominioId) { Error = null, Exito =false, Id = Id, SegundosDuracion = 0 };
        }


        public async Task<ResultadoTareaBackground> EjecutarTarea(string inputPayload = null)
        {
            try
            {
                _logger.LogInformation("Iniciando proceso de OCR");
                inicio = DateTime.UtcNow;
                var siguiente = await this._repoElastic.SiguenteIndexar(null);

                while (!stoppingToken.IsCancellationRequested && siguiente != null)
                {
                    _logger.LogInformation($"Procesando OCR {siguiente.Id}@{siguiente.Partes.Count}");
                    await ProcesaVersion(siguiente);
                    siguiente = await this._repoElastic.SiguenteIndexar(null);
                }

                fin = DateTime.UtcNow;

                resultado.Exito = true;
                resultado.SegundosDuracion = (int)((fin - inicio).TotalSeconds);
                _logger.LogInformation("Finalizando proceso de OCR");
                
                OnTareaFinalizada(resultado);

            }
            catch (Exception ex)
            {
                _logger.LogInformation($"Error de proceso de OCR {ex}");
                fin = DateTime.UtcNow;
                resultado.Exito = false;
                resultado.Error = ex.Message;
                resultado.SegundosDuracion = (int)((fin - inicio).TotalSeconds);
                OnTareaFinalizada(resultado);
            }
            
            return resultado;
        }

        private async Task<PIKA.Modelo.Contenido.Version> ProcesaVersion(PIKA.Modelo.Contenido.Version version)
        {
            try
            {
                if (version.Partes != null && version.Partes.Count > 0)
                {
                    Volumen v = await _volumenes.UnicoAsync(v => v.Id == version.VolumenId);
                    Elemento elemento = await _elementos.UnicoAsync(x => x.Id == version.ElementoId);
                    IGestorES gestor = await _volumenes.ObtienInstanciaGestor(v.Id);

                    ExtractorTexto x = new ExtractorTexto(_logger, _opciones.Value, gestor);
                    foreach (var parte in version.Partes)
                    {

                        if(stoppingToken.IsCancellationRequested)
                        {
                            break;
                        }

                        // Estos datos no estan en la parte para economizr espacio en el documento de ElasticSearch
                        parte.VersionId = version.Id;
                        parte.ElementoId = version.ElementoId;

                        // Asigna el ID del proces
                        if (gestor.UtilizaIdentificadorExterno)
                        {
                            parte.Id = parte.IdentificadorExterno;
                        }


                        // la parte no jha sido indexada
                        string nombreTemporal = x.NombreArchivoTemporal(parte);

                        if (extensionesIndexado.IndexOf(parte.Extension.ToLower()) >= 0)
                        {
                            switch (parte.Extension.ToLower())
                            {
                                case ".pdf":
                                    var (Exito, Rutas) = await x.TextoPDF(parte, nombreTemporal);
                                    if (Exito)
                                    {
                                        int pagina = 1;
                                        foreach (var t in Rutas)
                                        {
                                            string idPaginaExistente = await _repoElastic.ExisteTextoCompleto(new ContenidoTextoCompleto() { ElementoId = parte.ElementoId, ParteId = parte.Id, VersionId = parte.VersionId, Pagina = pagina });
                                            if (string.IsNullOrEmpty(idPaginaExistente))
                                            {
                                                await _repoElastic.IndexarTextoCompleto(parte.ParteAContenidoTextoCompleto(File.ReadAllText(t), elemento.PuntoMontajeId, elemento.CarpetaId, pagina));

                                            }
                                            else
                                            {
                                                await _repoElastic.ActualizarTextoCompleto(idPaginaExistente, parte.ParteAContenidoTextoCompleto(File.ReadAllText(t), elemento.PuntoMontajeId, elemento.CarpetaId, pagina));
                                            }

                                            pagina++;
                                        }
                                        x.ElimninaArchivosOCR(nombreTemporal);
                                        parte.Indexada = true;
                                    }
                                    else
                                    {
                                        parte.Indexada = false;
                                    }

                                    break;

                                default:
                                    string idExistente = await _repoElastic.ExisteTextoCompleto(new ContenidoTextoCompleto() { ElementoId = parte.ElementoId, ParteId = parte.Id, VersionId = parte.VersionId, Pagina = 1 });
                                    var resultadoImagen = await x.TextoImagen(parte, nombreTemporal);
                                    if (resultadoImagen.Exito)
                                    {
                                        if (string.IsNullOrEmpty(idExistente))
                                        {
                                            await _repoElastic.IndexarTextoCompleto(parte.ParteAContenidoTextoCompleto(File.ReadAllText(resultadoImagen.Ruta), elemento.PuntoMontajeId, elemento.CarpetaId));
                                        }
                                        else
                                        {
                                            await _repoElastic.ActualizarTextoCompleto(idExistente, parte.ParteAContenidoTextoCompleto(File.ReadAllText(resultadoImagen.Ruta), elemento.PuntoMontajeId, elemento.CarpetaId));
                                        }

                                        parte.Indexada = true;
                                    }
                                    else
                                    {
                                        parte.Indexada = false;
                                    }
                                    x.ElimninaArchivosOCR(nombreTemporal);
                                    break;
                            }

                        }


                        // Elimina el espacio requerido en elasticsearch, esto mininmiza el tamaño del almacen
                        parte.VersionId = null;
                        parte.ElementoId = null;
                    }
                }

                version.EstadoIndexado = version.Partes.Any(p => p.Indexada == false) ? EstadoIndexado.FinalizadoError : EstadoIndexado.FinalizadoOK;


            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                version.EstadoIndexado = EstadoIndexado.FinalizadoError;
            }

            if (!stoppingToken.IsCancellationRequested)
            {
                await _repoElastic.ActualizaEstadoOCR(version.Id, version);
            }
   
            return version;
        }


        public async Task<ResultadoTareaBackground> CaducarTarea(string InputPayload = null, string OutputPayload = null)
        {
            await Task.Delay(10);
            return null;
        }
    }
}
