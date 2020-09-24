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
using OpenXmlPowerTools;
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
    public class VisorController : ACLController
    {
        private ILogger<VisorController> logger;
        private IServicioVisor servicioVisor;
        public VisorController(
            ILogger<VisorController> logger,
            IServicioVisor servicioVisor)
        {
            this.logger = logger;
            this.servicioVisor = servicioVisor;
        }

        [HttpGet("documento/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<Documento>> GetDocumento(string Id)
        {
            Documento d = await servicioVisor.ObtieneDocumento(Id)
                .ConfigureAwait(false);

            if (d == null) return NotFound(Id);

            return Ok(d);
        }

    
    }
}
