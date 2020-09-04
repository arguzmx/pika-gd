using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PIKA.GD.API.Filters;
using PIKA.GD.API.Model;
using PIKA.Infraestructura.Comun;
using PIKA.Servicio.AplicacionPlugin;
using PIKA.Servicio.AplicacionPlugin.Interfaces;
using RepositorioEntidades;

namespace PIKA.GD.API.Controllers.Aplicaciones
{
    [ApiController]
    [Authorize]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/Sistema/[controller]")]
    public class AplicacionesController : ACLController
    {

        private ILogger<AplicacionesController> logger;
        private IServicioAplicacion servicioAplicacion;
        public AplicacionesController(
            ILogger<AplicacionesController> logger,
            IServicioAplicacion servicioAplicacion)
        {
            this.logger = logger;
            this.servicioAplicacion = servicioAplicacion;
        }

        [HttpGet(Name = "GetPageAplicaciones")]
        [TypeFilter(typeof(AsyncACLActionFilter),
            Arguments = new object[] { AplicacionRaiz.APP_ID, AplicacionAplicaciones.MODULO_APPS })]
        public async Task<ActionResult<IEnumerable<Aplicacion>>> GetPage()
        {
            logger.LogError(LocalizadorEnsamblados.ObtieneRutaBin());
            var data = await servicioAplicacion.OntieneAplicaciones(LocalizadorEnsamblados.ObtieneRutaBin())
                .ConfigureAwait(false);

            return Ok(data);
        }


    }
}
