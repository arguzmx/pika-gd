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
    public class MediaController : ControllerBase
    {
        private ILogger<VisorController> logger;
        private IServicioVolumen servicioVol;
        public MediaController(
            ILogger<VisorController> logger,
            IServicioVolumen servicioVol)
        {
            this.servicioVol = servicioVol;
            this.logger = logger;
        }

        // [ResponseCache(Duration = 300)]
        [HttpGet("pagina/{VolumenId}/{ElementoId}/{VersionId}/{ParteId}/{Extension}", Name = "getPagina")]
        public async Task<IActionResult> GetPage(string VolumenId, string ElementoId, 
            string VersionId, string ParteId, string Extension)
        {

            IGestorES gestor = await servicioVol.ObtienInstanciaGestor(VolumenId)
                             .ConfigureAwait(false);

            if (gestor == null) return UnprocessableEntity(VolumenId);
            Byte[] contenido = await gestor.LeeBytes(ElementoId, ParteId, VersionId, VolumenId, Extension).ConfigureAwait(false);

            if (contenido == null) return NotFound();

            this.HttpContext.Response.Headers.Add("Access-Control-Expose-Headers", "Content-Disposition");
            return File(contenido, MimeTypes.GetMimeType($"{ParteId}{Extension}"), $"{ParteId}{Extension}");
        }


        // [ResponseCache(Duration = 300)]
        [HttpGet("mini/{VolumenId}/{ElementoId}/{VersionId}/{ParteId}/{Extension}", Name = "getPaginaMiniature")]
        public async Task<IActionResult> GetPageMini(string VolumenId, string ElementoId,
            string VersionId, string ParteId, string Extension)
        {

            IGestorES gestor = await servicioVol.ObtienInstanciaGestor(VolumenId)
                             .ConfigureAwait(false);

            if (gestor == null) return UnprocessableEntity(VolumenId);
            Byte[] contenido = await gestor.LeeThumbnailBytes(ElementoId, ParteId, VersionId, VolumenId, Extension).ConfigureAwait(false);

            if (contenido == null) return NotFound();
            this.HttpContext.Response.Headers.Add("Access-Control-Expose-Headers", "Content-Disposition");
            return File(contenido, MimeTypes.GetMimeType($"{ParteId}{Extension}"), $"{ParteId}{Extension}");
        }

     }
}
