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
        private int maxThreads;
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
                string tmp = _configuracion.GetValue<string>("TareasBackground:ocr:hilos");
                if (!int.TryParse(tmp, out maxThreads))
                {
                    maxThreads = 1;
                }
                int count = 0;

                _logger.LogInformation($"Procesando en  {maxThreads} hilos");
                List<Task<ResultadoOCR>> TareasOCR = new List<Task<ResultadoOCR>>();

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


                        // la parte no ha sido indexada
                        string nombreTemporal = x.NombreArchivoTemporal(parte);

                        if (extensionesIndexado.IndexOf(parte.Extension.ToLower()) >= 0)
                        {
                            switch (parte.Extension.ToLower())
                            {
                                case ".pdf":
                                    TareasOCR.Add(x.TextoPDF(parte, nombreTemporal));
                                    break;

                                default:
                                    TareasOCR.Add(x.TextoImagen(parte, nombreTemporal));
                                    break;
                            }

                        }

                        // Procesa por lotes de tamaño acorde al número máximo de hilos
                        if(TareasOCR.Count >= maxThreads || version.Partes.Count == TareasOCR.Count)
                        {
                            Task.WaitAll(TareasOCR.ToArray());

                            // Procesa los resultados
                            foreach(var t in TareasOCR)
                            {
                                switch (t.Result.Parte.Extension.ToLower())
                                    {
                                        case ".pdf":
                                        if (t.Result.Exito)
                                        {
                                            int pagina = 1;
                                            foreach (var r in t.Result.Rutas)
                                            {
                                                string idPaginaExistente = await _repoElastic.ExisteTextoCompleto(new ContenidoTextoCompleto() { 
                                                    ElementoId = t.Result.Parte.ElementoId, 
                                                    ParteId = t.Result.Parte.Id, 
                                                    VersionId = t.Result.Parte.VersionId, 
                                                    Pagina = pagina });

                                                if (string.IsNullOrEmpty(idPaginaExistente))
                                                {
                                                    await _repoElastic.IndexarTextoCompleto(t.Result.Parte.ParteAContenidoTextoCompleto(File.ReadAllText(r), elemento.PuntoMontajeId, elemento.CarpetaId, pagina));

                                                }
                                                else
                                                {
                                                    await _repoElastic.ActualizarTextoCompleto(idPaginaExistente, t.Result.Parte.ParteAContenidoTextoCompleto(File.ReadAllText(r), elemento.PuntoMontajeId, elemento.CarpetaId, pagina));
                                                }

                                                pagina++;
                                            }
                                            x.ElimninaArchivosOCR(t.Result.NombreTemporal);
                                        }
                                        var pocr = version.Partes.Where(p => p.Id == t.Result.Parte.Id).First();
                                        pocr.Indexada = t.Result.Exito;
                                        // Elimina el espacio requerido en elasticsearch, esto mininmiza el tamaño del almacen
                                        pocr.VersionId = null;
                                        pocr.ElementoId = null;
                                        break;


                                        default:
                                        string idExistente = await _repoElastic.ExisteTextoCompleto(new ContenidoTextoCompleto() { 
                                            ElementoId = t.Result.Parte.ElementoId, ParteId = t.Result.Parte.Id, 
                                            VersionId = t.Result.Parte.VersionId, 
                                            Pagina = 1 });

                                        if (t.Result.Exito)
                                        {

                                            if (string.IsNullOrEmpty(idExistente))
                                            {
                                                await _repoElastic.IndexarTextoCompleto(parte.ParteAContenidoTextoCompleto(File.ReadAllText(t.Result.Rutas[0]), elemento.PuntoMontajeId, elemento.CarpetaId));
                                            }
                                            else
                                            {
                                                await _repoElastic.ActualizarTextoCompleto(idExistente, parte.ParteAContenidoTextoCompleto(File.ReadAllText(t.Result.Rutas[0]), elemento.PuntoMontajeId, elemento.CarpetaId));
                                            }


                                        }
                                        x.ElimninaArchivosOCR(t.Result.NombreTemporal);
                                        var pimg = version.Partes.Where(p => p.Id == t.Result.Parte.Id).First();
                                        pimg.Indexada = t.Result.Exito;
                                        // Elimina el espacio requerido en elasticsearch, esto mininmiza el tamaño del almacen
                                        pimg.VersionId = null;
                                        pimg.ElementoId = null;
                                        break;
                                    }

                            }

                            TareasOCR.Clear();
                        }

                    }
                }

                version.EstadoIndexado = version.Partes.Any(p => p.Indexada == false) ? EstadoIndexado.FinalizadoError : EstadoIndexado.FinalizadoOK;


            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                version.EstadoIndexado = EstadoIndexado.FinalizadoError;
            }

            await _repoElastic.ActualizaEstadoOCR(version.Id, version);
   
            return version;
        }

        public async Task<ResultadoTareaBackground> CaducarTarea(string InputPayload = null, string OutputPayload = null)
        {
            await Task.Delay(10);
            return null;
        }

        public void Dispose()
        {
   
        }
    }
}
