using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PIKA.Infraestructura.Comun;
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
    public class OCRHostedService : IHostedService, IDisposable
    {
        private Timer _timer;
        private readonly ILogger<OCRHostedService> _logger;
        private readonly IConfiguration _configuracion;
        private readonly IServicioVolumen _volumenes;
        private readonly IServicioElemento _elementos;
        private readonly IRepoContenidoElasticSearch _repoElastic;
        private readonly IOptions<ConfiguracionServidor> _opciones;
        private bool Procesando = false;
        private List<string> extensionesIndexado = new List<string>() {
            ".jpg", ".png", ".tif", ".bpm", ".gif", ".jpeg", ".jfif", ".pdf"
        };


        public OCRHostedService(
            ILogger<OCRHostedService> logger,
            IConfiguration configuracion, 
            IServicioVolumen volumenes,
            IRepoContenidoElasticSearch repoElastic,
            IServicioElemento elementos,
            IOptions<ConfiguracionServidor> opciones)
        {
            _opciones = opciones;
            _logger = logger;
            _configuracion = configuracion;
            _volumenes = volumenes;
            _repoElastic = repoElastic;
            _elementos = elementos;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogDebug("OCR inicialiando");
            _timer = new Timer(DoWork, null, TimeSpan.Zero, TimeSpan.FromSeconds(5));
            return Task.CompletedTask;
        }

        private void DoWork(object state)
        {
            if (!Procesando)
            {
                _logger.LogDebug("Buscando pendientes de OCR");
                var siguiente = Task.Run(() => this._repoElastic.SiguenteIndexar(null));
                if (siguiente.Result != null)
                {
                    Procesando = true;

                    var task = Task.Run(async () => await ProcesaVersion(siguiente.Result));
                    task.Wait();

                    // var resultado = Task.Run(() => ProcesaVersion(siguiente.Result));

                    _logger.LogDebug($"OCR Finalizado {task.Result.EstadoIndexado}");
                    Procesando = false;
                }

            }
        }

        private async Task<PIKA.Modelo.Contenido.Version> ProcesaVersion(PIKA.Modelo.Contenido.Version version)
        {

            Volumen v = await _volumenes.UnicoAsync(v => v.Id == version.VolumenId);
            Elemento elemento = await _elementos.UnicoAsync(x => x.Id == version.ElementoId);
            IGestorES gestor = await _volumenes.ObtienInstanciaGestor(v.Id);

            ExtractorTexto x = new ExtractorTexto(_logger, _opciones.Value, gestor);
            foreach (var parte in version.Partes)
            {
                // Estos datos no estan en la parte para economizr espacio en el documento de ElasticSearch
                parte.VersionId = version.Id;
                parte.ElementoId = version.ElementoId;

                if (!parte.Indexada)
                {
                    string nombreTemporal = x.NombreArchivoTemporal(parte);

                    if (extensionesIndexado.IndexOf(parte.Extension.ToLower()) >= 0)
                    {
                        _logger.LogDebug($"Indexando {parte.Extension} {parte.Id}");

                        switch (parte.Extension.ToLower())
                        {
                            case ".pdf":
                                
                                var resultadoPDF = await x.TextoPDF(parte, nombreTemporal);
                                _logger.LogDebug($"R {resultadoPDF.Item1} {parte.Extension} {parte.Id}");
                                if (resultadoPDF.Item1)
                                {
                                    int pagina = 1;
                                    foreach(var t in resultadoPDF.Item2)
                                    {
                                        await _repoElastic.IndexarTextoCompleto(parte.ParteAContenidoTextoCompleto(File.ReadAllText(t), elemento.PuntoMontajeId, elemento.CarpetaId, pagina));
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
                                var resultadoImagen = await x.TextoImagen(parte, nombreTemporal);
                                _logger.LogDebug($"R {resultadoImagen.Item1} {parte.Extension} {parte.Id}");
                                if (resultadoImagen.Item1)
                                {
                                    await _repoElastic.IndexarTextoCompleto(parte.ParteAContenidoTextoCompleto(File.ReadAllText(resultadoImagen.Item2), elemento.PuntoMontajeId, elemento.CarpetaId));
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

                    
                }

                parte.VersionId = null;
                parte.ElementoId = null;
            }

            version.EstadoIndexado = version.Partes.Any(p => p.Indexada == false) ? EstadoIndexado.FinalizadoError : EstadoIndexado.FinalizadoOK;

            await _repoElastic.ActualizaEstadoOCR(version.Id, version);
            return version;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Timed Hosted Service is stopping.");

            _timer?.Change(Timeout.Infinite, 0);

            return Task.CompletedTask;
        }
    }
}
