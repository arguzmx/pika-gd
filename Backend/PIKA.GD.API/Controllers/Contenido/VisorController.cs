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
    public class VisorController : ACLController
    {
        private ILogger<VisorController> logger;
        private IServicioVisor servicioVisor;
        private IRepoContenidoElasticSearch repoContenido;
        public VisorController(
            IRepoContenidoElasticSearch repoContenido,
            ILogger<VisorController> logger,
            IServicioVisor servicioVisor)
        {
            this.logger = logger;
            this.servicioVisor = servicioVisor;
            this.repoContenido = repoContenido;
        }

        [HttpGet("documento/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<Documento>> GetDocumento(string Id)
        {
            try
            {
                Documento d = await servicioVisor.ObtieneDocumento(Id)
                .ConfigureAwait(false);

                if (d == null) return NotFound(Id);

                this.logger.LogError(Id);
                var v = await repoContenido.ObtieneVersion(d.Id).ConfigureAwait(false);

                if(v!=null && v.Partes != null)
                {
                    foreach (var p in v.Partes)
                    {
                        d.Paginas.Add(p.APagina());
                    }
                }

                return Ok(d);
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex.ToString());
                throw;
            }
            
        }

    
    }
}
