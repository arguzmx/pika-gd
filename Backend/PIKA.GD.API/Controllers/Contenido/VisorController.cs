using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PIKA.Constantes.Aplicaciones.Contenido;
using PIKA.GD.API.Filters;
using PIKA.Infraestructura.Comun;
using PIKA.Infraestructura.Comun.Seguridad;
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

        public override void EmiteConfiguracionSeguridad(UsuarioAPI usuario, ContextoRegistroActividad RegistroActividad, List<EventoAuditoriaActivo> Eventos)
        {
            servicioVisor.EstableceContextoSeguridad(usuario, RegistroActividad, Eventos);
        }

        [HttpPost("documento/{id}/ordernar/alfabetico")]
        [TypeFilter(typeof(AsyncACLActionFilter), Arguments = new object[] { ConstantesAppContenido.APP_ID, ConstantesAppContenido.MODULO_ESTRUCTURA_CONTENIDO })]
        public async Task<ActionResult> OrdenarContenidoAlfabetico(string id)
        {
            Documento d = await servicioVisor.ObtieneDocumento(id)
            .ConfigureAwait(false);

            if (d == null) return NotFound(id);

            this.logger.LogError(id);
            var v = await repoContenido.ObtieneVersion(d.Id).ConfigureAwait(false);
            if(v!=null)
            {
                v.Partes = v.Partes.OrderBy(p => p.NombreOriginal).ToList();
                int idx = 1;
                foreach(var p in v.Partes)
                {
                    p.Indice = idx;
                    idx++;
                }
                await repoContenido.EliminaOCRVersion(v).ConfigureAwait(false);
                await repoContenido.ActualizaVersion(v.Id, v, true).ConfigureAwait(false);
            }

            return Ok();
        }


        [HttpGet("documento/{id}")]
        [TypeFilter(typeof(AsyncACLActionFilter), Arguments = new object[] { ConstantesAppContenido.APP_ID, ConstantesAppContenido.MODULO_ESTRUCTURA_CONTENIDO })]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<Documento>> GetDocumento(string id)
        {
            try
            {
                Documento d = await servicioVisor.ObtieneDocumento(id)
                .ConfigureAwait(false);

                if (d == null) return NotFound(id);

                this.logger.LogError(id);
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
