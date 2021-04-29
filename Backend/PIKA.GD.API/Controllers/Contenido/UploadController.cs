using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PIKA.Infraestructura.Comun;
using PIKA.Modelo.Contenido;
using PIKA.Modelo.Contenido.Extensiones;
using PIKA.Modelo.Contenido.ui;
using PIKA.Servicio.Contenido.ElasticSearch;
using PIKA.Servicio.Contenido.Interfaces;

namespace PIKA.GD.API.Controllers.Contenido
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    [Authorize]
    public class UploadController : ACLController
    {
        private ILogger<UploadController> logger;
        private IServicioVolumen servicioVol;
        private ConfiguracionServidor configuracionServidor;
        private IServicioElementoTransaccionCarga servicioTransaccionCarga;
        private IRepoContenidoElasticSearch repoContenido;
        public UploadController(
            IRepoContenidoElasticSearch repoContenido,
            IServicioElementoTransaccionCarga servicioTransaccionCarga,
            ILogger<UploadController> logger,
            IServicioVolumen servicioVol,
            IOptions<ConfiguracionServidor> opciones)
        {
            this.repoContenido = repoContenido;
            this.servicioTransaccionCarga = servicioTransaccionCarga;
            this.servicioVol = servicioVol;
            this.logger = logger;
            this.configuracionServidor = opciones.Value;
            FiltroArchivos.minimo = 1; //1 KB
            FiltroArchivos.maximo = long.MaxValue; //10 MB
            FiltroArchivos.addExt(".jpg");
            FiltroArchivos.addExt(".pdf");
            FiltroArchivos.addExt(".doc");
            FiltroArchivos.addExt(".docx");
            FiltroArchivos.addExt(".xls");
            FiltroArchivos.addExt(".xlsx");
            FiltroArchivos.addExt(".ppt");
            FiltroArchivos.addExt(".pptx");
            FiltroArchivos.addExt(".mp4");
            FiltroArchivos.addExt(".mp3");
            FiltroArchivos.addExt(".mov");
            FiltroArchivos.addExt(".avi");
            FiltroArchivos.addExt(".wmv");
            FiltroArchivos.addExt(".wav");
            FiltroArchivos.addExt(".ogg");

        }


        [RequestSizeLimit(long.MaxValue)]
        [HttpPost("completar/{TransaccionId}")]
        public async Task<ActionResult<List<Pagina>>> FinalizarLote(string TransaccionId)
        {
            await repoContenido.CreaRepositorio().ConfigureAwait(false);
            List<Pagina> paginas = new List<Pagina>();
            List<Parte> partes = new List<Parte>();
            string ruta = Path.Combine(configuracionServidor.ruta_cache_fisico, TransaccionId);

            this.logger.LogDebug($"{TransaccionId}");

            try
            {

                var elementos = await this.servicioTransaccionCarga.OtieneElementosTransaccion(TransaccionId).ConfigureAwait(false);

                if (elementos.Count > 0)
                {
                    string VolId = elementos[0].VolumenId;
                    string version = elementos[0].VersionId;
                    long conteoBytes = 0;
                    IGestorES gestor = await servicioVol.ObtienInstanciaGestor(VolId)
                           .ConfigureAwait(false);
                    
                    if(gestor==null)
                    {
                        return BadRequest("Gestor de volumen no válido");
                    }
                    
                    var v = await this.repoContenido.ObtieneVersion(version).ConfigureAwait(false);
                    if (v == null)
                    {
                        return NotFound();
                    }

                    int indice = 1;
                    if (v.Partes == null) v.Partes = new List<Parte>();

                    if (v.Partes.Count > 0)
                    {
                        indice = v.Partes.Max(x => x.Indice) + 1;
                    }

                    foreach (var el in elementos)
                    {
                        Parte p = el.ConvierteParte();
                        p.Indice = indice;
                        string filePath = Path.Combine(ruta, p.Id + Path.GetExtension(p.NombreOriginal));

                        if (System.IO.File.Exists(filePath))
                        {
                            FileInfo fi = new FileInfo(filePath);
                            
                            p.LongitudBytes = fi.Length;
                            p.Id = $"{p.Indice}"; //El is se sustituye por el indice para minimizar el payload al salvar en Elasticsearch
                            partes.Add(p);

                            await gestor.EscribeBytes(p.Id, p.ElementoId, p.VersionId, filePath, fi, false).ConfigureAwait(false);
                            
                            conteoBytes += p.LongitudBytes;
                            indice++;
                        }
                        paginas.Add(p.APagina($"{p.Indice}"));
                    }


                    v.TamanoBytes = v.Partes.Sum(x => x.LongitudBytes);
                    v.ConteoPartes = v.Partes.Count;
                    v.MaxIndicePartes = indice - 1;
                    v.Partes.AddRange(partes);

                    await this.repoContenido.ActualizaVersion(v.Id, v).ConfigureAwait(false);

                    try
                    {
                        Directory.Delete(ruta, true);
                    }
                    catch (Exception) { }

                    await this.servicioTransaccionCarga.EliminarTransaccion(TransaccionId, VolId, conteoBytes).ConfigureAwait(false);
                    return Ok(paginas);
                }
            }

            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                throw;
            }
            return BadRequest($"{TransaccionId}");
        }


        [HttpPost()]
        public async Task<IActionResult> PostContenido([FromForm] ElementoCargaContenido model)
        {

            if (tamanoValido(model.file.Length, FiltroArchivos.minimo, FiltroArchivos.maximo))
            {
                if (extensionValida(model.file, FiltroArchivos.extensionesValidas))
                {

                    var entrada = await servicioTransaccionCarga.CrearAsync(model.ConvierteETC()).ConfigureAwait(false);

                    string ruta = Path.Combine(configuracionServidor.ruta_cache_fisico, model.TransaccionId);
                    if (!Directory.Exists(ruta)) Directory.CreateDirectory(ruta);
                    string filePath = Path.Combine(ruta, entrada.Id + Path.GetExtension(model.file.FileName));

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await model.file.CopyToAsync(stream).ConfigureAwait(false);
                    }

                    if (System.IO.File.Exists(filePath))
                    {
                        return Ok();

                    }
                    else
                    {
                        return StatusCode((int)HttpStatusCode.InternalServerError, "Error de escritura");
                    }

                }
                else
                    ModelState.AddModelError(model.file.Name, "extensión inválida");
            }
            else
            {
                ModelState.AddModelError(model.file.Name, "tamaño inválido");
            }

            return BadRequest(ModelState);
        }

        private bool extensionValida(IFormFile file, ICollection<string> extensionesV)
        {
            var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (string.IsNullOrEmpty(ext) || !extensionesV.Contains(ext)) return false;
            if (!firmaValida(file.OpenReadStream(), ext)) return false;
            return true;
        }
        private bool firmaValida(Stream file, string ext)
        {
            bool valida = false;
            var firmasValidas = FiltroArchivos.firmasArchivo;
            using (var reader = new BinaryReader(file))
            {
                var signatures = firmasValidas[ext] != null ? firmasValidas[ext] : null;
                if (signatures != null)
                {
                    var headerBytes = reader.ReadBytes(signatures.Max(m => m.Length));
                    valida = signatures.Any(signature =>
                             headerBytes.Take(signature.Length).SequenceEqual(signature));
                }
            }
            return valida;
        }
        private bool tamanoValido(long size, long min, long max)
        {
            if (size > 0)
                if (size > min && size <= max)
                    return true;
            return false;
        }

    }
}

