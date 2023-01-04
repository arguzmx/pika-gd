using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PIKA.Constantes.Aplicaciones.Aplicaciones;
using PIKA.Constantes.Aplicaciones.Seguridad;
using PIKA.GD.API.Filters;
using PIKA.GD.API.Model;
using PIKA.Infraestructura.Comun;
using PIKA.Infraestructura.Comun.Seguridad;
using PIKA.Modelo.Seguridad;
using PIKA.Servicio.Seguridad.Interfaces;
using PIKA.Servicio.Seguridad.Servicios;
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
        private IServicioEventoAuditoriaActivo servicioEventoAuditoriaActivo;
        private IServicioEventoAuditoria servicioEventoAuditoria;
        public SeguridadController(
            IServicioEventoAuditoriaActivo servicioEventoAuditoriaActivo,
            IServicioSeguridadAplicaciones servicioSeguridad,
            IServicioUsuarios servicioUsuarios,
            ILogger<SeguridadController> logger,
            IServicioInfoAplicacion servicioAplicacion,
            IServicioEventoAuditoria servicioEventoAuditoria)
        {
            this.servicioSeguridad = servicioSeguridad;
            this.servicioUsuarios = servicioUsuarios;
            this.logger = logger;
            this.servicioAplicacion = servicioAplicacion;
            this.servicioEventoAuditoriaActivo = servicioEventoAuditoriaActivo;
            this.servicioEventoAuditoria = servicioEventoAuditoria;
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


        public override void EmiteConfiguracionSeguridad(UsuarioAPI usuario, ContextoRegistroActividad RegistroActividad, List<EventoAuditoriaActivo> Eventos)
        {

            servicioUsuarios.EstableceContextoSeguridad(usuario, RegistroActividad, Eventos);
            servicioSeguridad.EstableceContextoSeguridad(usuario, RegistroActividad, Eventos);
            servicioEventoAuditoriaActivo.EstableceContextoSeguridad(usuario, RegistroActividad, Eventos);
            servicioEventoAuditoria.EstableceContextoSeguridad(usuario, RegistroActividad, Eventos);
        }

        [HttpGet("eventosauditoria", Name = "GetEventosBitacora")]
        [TypeFilter(typeof(AsyncACLActionFilter),
            Arguments = new object[] { ConstantesAppSeguridad.APP_ID, ConstantesAppSeguridad.MODULO_AUDITORIA })]
        public async Task<ActionResult<IEnumerable<EventoAuditoriaActivo>>> GetEventosBitacora()
        {
            var data = await servicioEventoAuditoriaActivo.ObtieneEventosActivos().ConfigureAwait(false);
            return Ok(data);
        }

        [HttpPost("eventosauditoria", Name = "SetEventosBitacora")]
        [TypeFilter(typeof(AsyncACLActionFilter),
            Arguments = new object[] { ConstantesAppSeguridad.APP_ID, ConstantesAppSeguridad.MODULO_AUDITORIA })]
        public async Task<ActionResult> PostEventosBitacora([FromBody] List<EventoAuditoriaActivo> eventos)
        {
            await servicioEventoAuditoriaActivo.ActualizaEventosActivos(eventos).ConfigureAwait(false);
            return Ok();
        }

        [HttpPost("eventosauditoria/buscar", Name = "GetQueryBitacora")]
        [TypeFilter(typeof(AsyncACLActionFilter),
        Arguments = new object[] { ConstantesAppSeguridad.APP_ID, ConstantesAppSeguridad.MODULO_AUDITORIA })]
        public async Task<ActionResult<IPaginado<EventoAuditoria>>> GetQueryBitacora([FromBody] QueryBitacora q)
        {
            var data = await servicioEventoAuditoria.Buscar(q).ConfigureAwait(false);
            return Ok(data);
        }


        [HttpPost("permisos/aplicar", Name = "PostPermisosCrear")]
        [TypeFilter(typeof(AsyncACLActionFilter),
            Arguments = new object[] { ConstantesAppSeguridad.APP_ID, ConstantesAppSeguridad.MODULO_ACL })]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<int>> PostPermisosCrear([FromBody] List<PermisoAplicacion> permisos)
        {
            await servicioSeguridad.CrearActualizarAsync(permisos.ToArray()).ConfigureAwait(false);
            return Ok();
        }

        [HttpPost("permisos/eliminar", Name = "PostPermisosEliminar")]
        [TypeFilter(typeof(AsyncACLActionFilter),
            Arguments = new object[] { ConstantesAppSeguridad.APP_ID, ConstantesAppSeguridad.MODULO_ACL })]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<int>> PostPermisosEliminar([FromBody] List<PermisoAplicacion> permisos)
        {
            await servicioSeguridad.EliminarAsync(permisos.ToArray()).ConfigureAwait(false);
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
                r.EsAdmmin = await servicioUsuarios.EsAdmin(this.DominioId, this.TenantId, id);
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
