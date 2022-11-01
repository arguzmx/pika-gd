using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using LazyCache;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PIKA.Constantes.Aplicaciones.Organizacion;
using PIKA.GD.API.Filters;
using PIKA.GD.API.Model;
using PIKA.GD.API.Servicios.Registro;
using PIKA.Modelo.Organizacion.Estructura;
using PIKA.Servicio.Contenido.ElasticSearch;
using PIKA.Servicio.Organizacion;

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
        private IRepoContenidoElasticSearch repoContenido;
        private IRegistroPIKA registroPIKA;

        public AppConfigController(
            ILogger<AppConfigController> logger,
            IAppCache appCache,
            IRegistroPIKA registroPIKA,
            IServicioDominio servDominio,
            IRepoContenidoElasticSearch repoContenido)
        {
            this.registroPIKA = registroPIKA;
            this.logger = logger;
            this.servDominio = servDominio;
            this.appCache = appCache;
            this.repoContenido = repoContenido;
        }

        [HttpGet("fingerprint")]
        [AllowAnonymous]
        public async Task<ActionResult<string>> ObtenerFingerprint()
        {
            var fp = await this.registroPIKA.ObtenerFingerprint();
            return Ok(fp);
        }

        [HttpGet("activado")]
        [AllowAnonymous]
        public async Task<ActionResult<bool>> AplicacionActivada()
        {
            var valida = await this.registroPIKA.LicenciaValida();
            return Ok(valida);
        }


        [HttpPost("activar")]
        [AllowAnonymous]
        public async Task<ActionResult> ActivarAplicacion()
        {
            string mylic = "";
            using (StreamReader reader = new StreamReader(Request.Body, Encoding.UTF8))
            {
                mylic = await reader.ReadToEndAsync();
            }

            var valida = await this.registroPIKA.LicenciaValida();
            if(!valida)
            {
                var resultado = await this.registroPIKA.ActivarLicencia(mylic);
                if (resultado)
                {
                    return Ok();
                }
                else
                {
                    return BadRequest();
                }
            } else
            {
                return Ok();
            }
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
