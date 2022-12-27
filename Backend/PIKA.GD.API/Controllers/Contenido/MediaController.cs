using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PIKA.Constantes.Aplicaciones.Contenido;
using PIKA.Constantes.Aplicaciones.Organizacion;
using PIKA.GD.API.Filters;
using PIKA.Infraestructura.Comun.Seguridad;
using PIKA.Modelo.Contenido;
using PIKA.Servicio.Contenido.Interfaces;


namespace PIKA.GD.API.Controllers.Contenido
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    public class MediaController : ACLController
    {
        private ILogger<VisorController> logger;
        private IServicioVolumen servicioVol;
        private IServicioElemento servicioElemento;
        public MediaController(
            ILogger<VisorController> logger,
            IServicioVolumen servicioVol,
            IServicioElemento servicioElemento)
        {
            this.servicioElemento = servicioElemento;
            this.servicioVol = servicioVol;
            this.logger = logger;
        }

        public override void EmiteConfiguracionSeguridad(UsuarioAPI usuario, ContextoRegistroActividad RegistroActividad, List<EventoAuditoriaActivo> Eventos)
        {
            servicioElemento.EstableceContextoSeguridad(usuario, RegistroActividad, Eventos);
            servicioVol.EstableceContextoSeguridad(usuario, RegistroActividad, Eventos);
        }


        // [ResponseCache(Duration = 300)]
        [HttpGet("pagina/{VolumenId}/{ElementoId}/{VersionId}/{ParteId}/{Extension}", Name = "getPagina")]
        [TypeFilter(typeof(AsyncACLActionFilter), Arguments = new object[] { ConstantesAppContenido.APP_ID, ConstantesAppContenido.MODULO_ESTRUCTURA_CONTENIDO })]
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
        [TypeFilter(typeof(AsyncACLActionFilter), Arguments = new object[] { ConstantesAppContenido.APP_ID, ConstantesAppContenido.MODULO_ESTRUCTURA_CONTENIDO })]
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
