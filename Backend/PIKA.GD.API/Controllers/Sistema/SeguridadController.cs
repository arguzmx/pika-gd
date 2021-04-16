using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Nest;
using PIKA.Constantes.Aplicaciones.Aplicaciones;
using PIKA.Constantes.Aplicaciones.Seguridad;
using PIKA.GD.API.Filters;
using PIKA.GD.API.Model;
using PIKA.Infraestructura.Comun;
using PIKA.Infraestructura.Comun.Seguridad;
using PIKA.Servicio.AplicacionPlugin;
using PIKA.Servicio.Seguridad.Interfaces;
using RepositorioEntidades;

namespace PIKA.GD.API.Controllers.Sistema
{
    [ApiController]
    [Authorize]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/Sistema/[controller]")]
    public class SeguridadController : ACLController
    {

        private ILogger<SeguridadController> logger;
        private IServicioInfoAplicacion servicioAplicacion;
        private IServicioSeguridadAplicaciones servicioSeguridad;
        private IServicioUsuarios servicioUsuarios;
        public SeguridadController(
            IServicioSeguridadAplicaciones servicioSeguridad,
            IServicioUsuarios servicioUsuarios,
            ILogger<SeguridadController> logger,
            IServicioInfoAplicacion servicioAplicacion)
        {
            this.servicioSeguridad = servicioSeguridad;
            this.servicioUsuarios = servicioUsuarios;
            this.logger = logger;
            this.servicioAplicacion = servicioAplicacion;
        }

        [HttpGet("aplicaciones", Name = "GetPageAplicaciones")]
        [TypeFilter(typeof(AsyncACLActionFilter),
            Arguments = new object[] { ConstantesAppAplicacionPlugin.APP_ID, ConstantesAppAplicacionPlugin.MODULO_APLICACIONES })]
        public async Task<ActionResult<IEnumerable<Aplicacion>>> GetPage()
        {
            var data = await servicioAplicacion.ObtieneAplicaciones(LocalizadorEnsamblados.ObtieneRutaBin())
                .ConfigureAwait(false);

            return Ok(data);
        }


        [HttpPost("permisos/aplicar", Name = "PostPermisosCrear")]
        [TypeFilter(typeof(AsyncACLActionFilter),
            Arguments = new object[] { ConstantesAppSeguridad.APP_ID, ConstantesAppSeguridad.MODULO_ACL })]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<int>> PostPermisosCrear([FromBody] List<PermisoAplicacion> permisos)
        {
            await servicioSeguridad.CrearActualizarAsync(this.DominioId, permisos.ToArray()).ConfigureAwait(false);
            return Ok();
        }

        [HttpPost("permisos/eliminar", Name = "PostPermisosEliminar")]
        [TypeFilter(typeof(AsyncACLActionFilter),
            Arguments = new object[] { ConstantesAppSeguridad.APP_ID, ConstantesAppSeguridad.MODULO_ACL })]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<int>> PostPermisosEliminar([FromBody] List<PermisoAplicacion> permisos)
        {
            await servicioSeguridad.EliminarAsync(this.DominioId, permisos.ToArray()).ConfigureAwait(false);
            return Ok();
        }


        [HttpGet("permisos/{tipo}/{id}", Name = "GetPermisosPorTipo")]
        [TypeFilter(typeof(AsyncACLActionFilter),
            Arguments = new object[] { ConstantesAppSeguridad.APP_ID, ConstantesAppSeguridad.MODULO_ACL })]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<RespuestaPermisos>> GetPermisosPorTipo(string  tipo, string id)
        {
            RespuestaPermisos r = new RespuestaPermisos();

            r.Permisos = (await servicioSeguridad.ObtienePermisosAsync(tipo, id, this.DominioId).ConfigureAwait(false)).ToList();
            r.Id = id;
            r.EsAdmmin = false;
            if (tipo.ToLower() == "u")
            {
                r.EsAdmmin = await servicioUsuarios.EsAdmin(this.DominioId, this.TenatId, id);
            }
            return Ok(r);
        }

        [HttpGet("usuarios/pares", Name = "GetUsuarios")]
        [TypeFilter(typeof(AsyncACLActionFilter),
           Arguments = new object[] { ConstantesAppSeguridad.APP_ID, ConstantesAppSeguridad.MODULO_ACL })]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<ICollection<PermisoAplicacion>>> GetUsuarios(Consulta query)
        {
            var data = await this.servicioUsuarios.ObtenerParesAsync(query).ConfigureAwait(false);
            return Ok(data);
        }



    }
}
