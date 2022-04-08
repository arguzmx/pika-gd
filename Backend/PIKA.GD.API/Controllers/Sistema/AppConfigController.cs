using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LazyCache;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PIKA.Constantes.Aplicaciones.Organizacion;
using PIKA.GD.API.Filters;
using PIKA.GD.API.Model;
using PIKA.Infraestructura.Comun;
using PIKA.Infraestructura.Comun.Seguridad;
using PIKA.Modelo.Organizacion.Estructura;
using PIKA.Servicio.AplicacionPlugin;
using PIKA.Servicio.AplicacionPlugin.Interfaces;
using PIKA.Servicio.Contenido.ElasticSearch;
using PIKA.Servicio.Organizacion;
using PIKA.Servicio.Seguridad.Interfaces;
using RepositorioEntidades;

namespace PIKA.GD.API.Controllers.Sistema
{
    [ApiController]
    [Authorize]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/Sistema/[controller]")]
    public class AppConfigController : ACLController
    {

        private readonly IAppCache appCache;
        private ILogger<AppConfigController> logger;
        private IServicioDominio servDominio;
        IRepoContenidoElasticSearch repoContenido;

        public AppConfigController(
            ILogger<AppConfigController> logger,
            IAppCache appCache,
            IServicioDominio servDominio,
            IRepoContenidoElasticSearch repoContenido)
        {
            this.logger = logger;
            this.servDominio = servDominio;
            this.appCache = appCache;
            this.repoContenido = repoContenido;
        }

        [HttpGet("estadoocr", Name = "ObtieneEstadoOCR")]
        [TypeFilter(typeof(AsyncACLActionFilter),
            Arguments = new object[] { ConstantesAppOrganizacion.APP_ID, ConstantesAppOrganizacion.MODULO_DOMINIO })]
        public async Task<ActionResult> ObtieneEstadoOCR()
        {
            var data = await repoContenido.OntieneEstadoOCR().ConfigureAwait(false);
            return Ok(data);
        }

        [HttpPost("ocrerroneos", Name = "ReiniciaOCRErroneos")]
        [TypeFilter(typeof(AsyncACLActionFilter),
        Arguments = new object[] { ConstantesAppOrganizacion.APP_ID, ConstantesAppOrganizacion.MODULO_DOMINIO })]
        public async Task<ActionResult> ReiniciaOCRErroneos()
        {
            await repoContenido.ReiniciarOCRErroneos().ConfigureAwait(false);
            return Ok();
        }


        [HttpGet("dominiouo", Name = "ObtieneDomininOU")]
        [TypeFilter(typeof(AsyncACLActionFilter),
            Arguments = new object[] { ConstantesAppOrganizacion.APP_ID, ConstantesAppOrganizacion.MODULO_DOMINIO })]
        public async Task<ActionResult> ObtieneDomininOU()
        {
            var data = await servDominio.OntieneDominioOU(this.DominioId, this.TenantId).ConfigureAwait(false);
            if (data == null) return NotFound();
            return Ok(data);
        }

        [HttpPost("dominiouo", Name = "ActualizaDomininOU")]
        [TypeFilter(typeof(AsyncACLActionFilter),
            Arguments = new object[] { ConstantesAppOrganizacion.APP_ID, ConstantesAppOrganizacion.MODULO_DOMINIO })]
        public async  Task<ActionResult> ActualizaDomininOU([FromBody] ActDominioOU request)
        {
            var result = await servDominio.ActualizaDominioOU(request,  this.DominioId, this.TenantId).ConfigureAwait(false);
            if(result)
            {
                return Ok();
            }
            return BadRequest();
        }

        [HttpGet("ruteotipos", Name = "ObtieneRutasPorTipo")]
        public ActionResult<List<RutaTipo>> ObtieneRutasPorTipo()
        {
            List<RutaTipo> rutas = appCache.GetOrAdd<List<RutaTipo>>(
                ConstantesCache.RUTEOAPITIPO,
                () => LocalizadorEnsamblados.ObtieneControladoresACL()
                );
            return Ok(rutas);
        }

    }
}
