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
using PIKA.GD.API.Filters;
using PIKA.GD.API.Model;
using PIKA.Infraestructura.Comun;
using PIKA.Infraestructura.Comun.Seguridad;
using PIKA.Servicio.AplicacionPlugin;
using PIKA.Servicio.AplicacionPlugin.Interfaces;
using PIKA.Servicio.Seguridad.Interfaces;
using RepositorioEntidades;

namespace PIKA.GD.API.Controllers.Sistema
{
    [ApiController]
    [Authorize]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/Sistema/[controller]")]
    public class AppConfigController : ControllerBase
    {

        private readonly IAppCache appCache;
        public AppConfigController(IAppCache appCache)
        {
            this.appCache = appCache;
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
