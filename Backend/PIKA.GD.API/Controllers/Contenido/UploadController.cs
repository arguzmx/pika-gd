using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PIKA.GD.API.Model;
using PIKA.Infraestructura.Comun;
using PIKA.Modelo.Contenido;
using PIKA.Servicio.Contenido.Interfaces;

namespace PIKA.GD.API.Controllers.Contenido
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    public class UploadController : ControllerBase
    {
        private ILogger<UploadController> logger;
        private IServicioVolumen servicioVol;
        private ConfiguracionServidor configuracionServidor;

        public UploadController(ILogger<UploadController> logger,
            IServicioVolumen servicioVol, 
            IOptions<ConfiguracionServidor> opciones)
        {
            this.servicioVol = servicioVol;
            this.logger = logger;
            this.configuracionServidor = opciones.Value;
            FiltroArchivos.minimo = 1024; //1 KB
            FiltroArchivos.maximo = 1048576; //1 MB
            FiltroArchivos.addExt(".jpg");
            FiltroArchivos.addExt(".pdf");
            FiltroArchivos.addExt(".doc");
            FiltroArchivos.addExt(".docx");
        }



        [HttpPost("ContenidoFiltrado")]
        public async Task<IActionResult> PostContenidoFiltrado([FromForm] ElementoContenido model)
        {


   
            logger.LogWarning(model.VolumenId);
            logger.LogWarning(model.ElementoId);
            logger.LogWarning(model.PuntoMontajeId);


            if (tamanoValido(model.file.Length, FiltroArchivos.minimo, FiltroArchivos.maximo))
            {
                if (extensionValida(model.file, FiltroArchivos.extensionesValidas))
                {

                    IGestorES gestor = await servicioVol.ObtienInstanciaGestor(model.VolumenId).ConfigureAwait(false);
                   

                    var filePath = Path.Combine(configuracionServidor.ruta_cache_fisico, 
                        Path.GetRandomFileName() + 
                        Path.GetExtension(model.file.FileName));

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        logger.LogWarning($"copiando {filePath}");
                        await model.file.CopyToAsync(stream).ConfigureAwait(false);
                    }

                    if (System.IO.File.Exists(filePath))
                    {
                        long resultado = await
                        gestor.EscribeBytes(model.ElementoId,
                        System.IO.File.ReadAllBytes(filePath),
                        new FileInfo(filePath), true).ConfigureAwait(false);

                        logger.LogWarning($"r {resultado}");
                        if (resultado > 0)
                        {
                            logger.LogWarning($"eliminando {filePath}");
                            System.IO.File.Delete(filePath);
                        }
                        return Ok(new { resultado });
                    } else
                    {
                        throw new Exception("Error al escribir en cache");
                    }
                 

                }
                else
                    ModelState.AddModelError(model.file.Name, "extensión inválida");
            }
            else
                ModelState.AddModelError(model.file.Name, "tamaño inválido");
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

