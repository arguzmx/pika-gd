using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PIKA.GD.API.Filters;
using PIKA.Infraestructura.Comun;
using PIKA.Servicio.Usuarios;

namespace PIKA.GD.API.Controllers
{
    [Authorize]
    [ApiVersion("1.0")]
    [ApiController]
    [Route("api/v{version:apiVersion}/usuario/[controller]")]
    public class PerfilController : ACLController
    {

        private ILogger<PerfilController> logger;
        private IServicioPerfilUsuario servicioEntidad;

        public PerfilController(ILogger<PerfilController> logger,
            IServicioPerfilUsuario servicioEntidad)
        {
            this.logger = logger;
            this.servicioEntidad = servicioEntidad;
        }

    

        [HttpGet("dominios", Name = "DominiosUsuario")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<List<DominioActivo>>> Dominios()
        {
            var dominios = await this.servicioEntidad.Dominios(GetUserId()).ConfigureAwait(false);
            return Ok(dominios);
        }

    }

  
}
