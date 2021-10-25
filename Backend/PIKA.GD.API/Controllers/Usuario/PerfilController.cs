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
using PIKA.Infraestructura.Comun.Menus;
using PIKA.Infraestructura.Comun.Seguridad;
using PIKA.Modelo.Seguridad;
using PIKA.Servicio.Seguridad.Interfaces;
using PIKA.Servicio.Usuarios;
using PIKA.Servicio.Usuarios.Entidades;

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
        private ICacheSeguridad CacheSeguridad;
        private IServicioMenuAplicacion ServicioMenuAplicacion;
        private IServicioTokenSeguridad ServicioTokenSeguridad;
        private IServicioUsuarios servicioUsuarios;

        public PerfilController(
            IServicioTokenSeguridad ServicioTokenSeguridad,
            IServicioMenuAplicacion ServicioMenuAplicacion,
            ICacheSeguridad CacheSeguridad,
            ILogger<PerfilController> logger,
            IServicioPerfilUsuario servicioEntidad,
            IServicioUsuarios servicioUsuarios)
        {
            this.logger = logger;
            this.ServicioTokenSeguridad = ServicioTokenSeguridad;
            this.CacheSeguridad = CacheSeguridad;
            this.servicioEntidad = servicioEntidad;
            this.ServicioMenuAplicacion = ServicioMenuAplicacion;
            this.servicioUsuarios = servicioUsuarios;
        }

        [HttpGet()]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<PropiedadesUsuario>> ObtieneUsuario()
        {
            var u = await this.servicioUsuarios.UnicoAsync(x => x.UsuarioId == this.GetUserId()).ConfigureAwait(false);
            if (u != null)
            {
                return Ok(u);
            }
            return NotFound();
        }

        [HttpPut()]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult> ActualizaPropieades([FromBody] PropiedadesUsuario propiedades)
        {
            var u = await this.servicioUsuarios.UnicoAsync(x => x.UsuarioId == this.GetUserId()).ConfigureAwait(false);
            if (u != null)
            {
                propiedades.UsuarioId = this.GetUserId();
                await this.servicioUsuarios.ActualizarAsync(propiedades).ConfigureAwait(false);
                return Ok();
            }
            return NotFound();
        }



        [HttpPost("contrasena/actualizar", Name = "ActualizarContrasena")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult> ActualizarContrasena([FromBody] ActualizarContrasena request)
        {
            return StatusCode(await this.servicioUsuarios.ActutalizarContrasena(this.GetUserId(), request.Actual, request.Nueva).ConfigureAwait(false));
        }

        [HttpGet("dominios", Name = "DominiosUsuario")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<List<DominioActivo>>> Dominios()
        {
            var dominios = await this.servicioEntidad.Dominios(GetUserId()).ConfigureAwait(false);
            return Ok(dominios);
        }



        [HttpGet("acl", Name = "ACLAplicacion")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [TypeFilter(typeof(AsyncIdentityFilter))]
        public async Task<ActionResult<DefinicionSeguridadUsuario>> ObtieneACLAplicacion()
        {
            Console.WriteLine($"{this.UsuarioId}:{this.DominioId}");
            var acl = await ServicioTokenSeguridad.ObtenerSeguridadUsuario(this.UsuarioId, this.DominioId)
                .ConfigureAwait(false);
            return Ok(acl);

        }

        [HttpGet("menu/{id}", Name = "MenuAplicacion")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [TypeFilter(typeof(AsyncIdentityFilter))]
        public async Task<ActionResult<MenuAplicacion>> ObtieneMenuAplicacion(string id)
        {
            var menu = await ServicioMenuAplicacion.ObtieneMenuApp(id).ConfigureAwait(false);
            return Ok(menu);

        }


    }

  
}
