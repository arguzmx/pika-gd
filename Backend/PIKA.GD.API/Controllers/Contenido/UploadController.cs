using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using PIKA.Modelo.Contenido;

namespace PIKA.GD.API.Controllers.Contenido
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    public class UploadController : ControllerBase
    {
        private ILogger<UploadController> logger;
        private IConfiguration Config;


        public UploadController(ILogger<UploadController> logger, IConfiguration Config)
        {
            this.logger = logger;
            this.Config = Config;
            FiltroArchivos.minimo = 1024; //1 KB
            FiltroArchivos.maximo = 1048576; //1 MB
            FiltroArchivos.addExt(".jpg");
            FiltroArchivos.addExt(".pdf");
            FiltroArchivos.addExt(".doc");
            FiltroArchivos.addExt(".docx");
        }

        [HttpPost]
        public async Task<IActionResult> Post(IFormFile file)
        {
            long size = file.Length;
            var filePath = "";
            if (size > 0)
            {
                var path = Config["FILE_PATH"];
                filePath = Path.Combine(path, Path.GetRandomFileName() + Path.GetExtension(file.FileName));
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream).ConfigureAwait(false);
                }
            }
            return Ok(new { size, filePath });
        }

        [HttpPost("ContenidoFiltrado")]
        public async Task<IActionResult> PostContenidoFiltrado(IFormFile file)
        {
            if (tamanoValido(file.Length, FiltroArchivos.minimo, FiltroArchivos.maximo))
            {
                if (extensionValida(file, FiltroArchivos.extensionesValidas))
                {
                    var filePath = Path.Combine(Config["FILE_PATH"], Path.GetRandomFileName() + Path.GetExtension(file.FileName));
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await file.CopyToAsync(stream).ConfigureAwait(false);
                        return Ok(new { file.Length, filePath });
                    }
                }
                else
                    ModelState.AddModelError(file.Name, "extensión inválida");
            }
            else
                ModelState.AddModelError(file.Name, "tamaño inválido");
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

