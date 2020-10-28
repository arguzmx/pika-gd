using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PIKA.Infraestructura.Comun;
using PIKA.Modelo.Contenido;
using PIKA.Modelo.Contenido.ui;
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

        public UploadController(
            IServicioElementoTransaccionCarga servicioTransaccionCarga,
            ILogger<UploadController> logger,
            IServicioVolumen servicioVol, 
            IOptions<ConfiguracionServidor> opciones)
        {
            this.servicioTransaccionCarga = servicioTransaccionCarga;
            this.servicioVol = servicioVol;
            this.logger = logger;
            this.configuracionServidor = opciones.Value;
            FiltroArchivos.minimo = 1024; //1 KB
            FiltroArchivos.maximo = 10485760; //10 MB
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

        [HttpPost("completar/{TransaccionId}")]
        public async Task<ActionResult<List<Pagina>>> FinalizarLote(string TransaccionId)
        {
            string VolId = await servicioTransaccionCarga.ObtieneVolumenIdTransaccion(TransaccionId)
                .ConfigureAwait(false);
            if (VolId!=null)
            {
                IGestorES gestor = await servicioVol.ObtienInstanciaGestor(VolId)
                               .ConfigureAwait(false);
                if (gestor != null)
                {
                    List<Pagina> paginas = await this.servicioTransaccionCarga.ProcesaTransaccion(TransaccionId, VolId, gestor)
                        .ConfigureAwait(false);

                    return Ok(paginas);

                }

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

                    } else
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
        private bool tamanoValido(long size, int min, int max)
        {
            if (size > 0)
                if (size > min && size <= max)
                    return true;
            return false;
        }

    }
}

