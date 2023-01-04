using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PIKA.GD.API.Filters;
using PIKA.Infraestructura.Comun.Seguridad;
using PIKA.Modelo.GestorDocumental;
using PIKA.Modelo.Metadatos;
using PIKA.Servicio.GestionDocumental.Interfaces;
using RepositorioEntidades;


namespace PIKA.GD.API.Controllers.GestorDocumental
{
    [Authorize]
    [ApiVersion("1.0")]
    [ApiController]
    [Route("api/v{version:apiVersion}/gd/Transferencia/{TransferenciaId}/[controller]")]
    public class EventoTransferenciaController : ACLController
    {
        private readonly ILogger<EventoTransferenciaController> logger;
        private IServicioEventoTransferencia servicioEventoTransferencia;
        private IProveedorMetadatos<EventoTransferencia> metadataProvider;
        public EventoTransferenciaController(ILogger<EventoTransferenciaController> logger,
            IProveedorMetadatos<EventoTransferencia> metadataProvider,
            IServicioEventoTransferencia servicioEventoTransferencia)
        {
            this.logger = logger;
            this.servicioEventoTransferencia = servicioEventoTransferencia;
            this.metadataProvider = metadataProvider;
        }


        public override void EmiteConfiguracionSeguridad(UsuarioAPI usuario, ContextoRegistroActividad RegistroActividad, List<EventoAuditoriaActivo> Eventos)
        {
            servicioEventoTransferencia.EstableceContextoSeguridad(usuario, RegistroActividad, Eventos);
        }


        [HttpGet("metadata", Name = "MetadataEventoTransferencia")]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        public async Task<ActionResult<MetadataInfo>> GetMetadata([FromQuery]Consulta query = null)
        {
            return Ok(await metadataProvider.Obtener().ConfigureAwait(false));
        }



        [HttpPost]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        public async Task<ActionResult<EventoTransferencia>> Post([FromBody]EventoTransferencia entidad)
        {
            entidad = await servicioEventoTransferencia.CrearAsync(entidad).ConfigureAwait(false);
            return Ok(CreatedAtAction("GetEventoTransferencia", new { id = entidad.Id }, entidad).Value);
        }


        [HttpPut("{id}")]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        public async Task<IActionResult> Put(string id, [FromBody]EventoTransferencia entidad)
        {
            var x = ObtieneFiltrosIdentidad();


            if (id != entidad.Id)
            {
                return BadRequest();
            }

            await servicioEventoTransferencia.ActualizarAsync(entidad).ConfigureAwait(false);
            return NoContent();

        }


        [HttpGet("page", Name = "GetPageEventoTransferencia")]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        public async Task<ActionResult<IEnumerable<EventoTransferencia>>> GetPage([FromQuery]Consulta query = null)
        {
            ///Añade las propiedaes del contexto para el filtro de ACL vía ACL Controller
            var data = await servicioEventoTransferencia.ObtenerPaginadoAsync(
                 Query: query,
                 include: null)
                 .ConfigureAwait(false);

            return Ok(data);
        }


        [HttpGet("{id}")]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        public async Task<ActionResult<EventoTransferencia>> Get(string id)
        {
            var o = await servicioEventoTransferencia.UnicoAsync(x => x.Id == id).ConfigureAwait(false);
            if (o != null) return Ok(o);
            return NotFound(id);
        }




        [HttpDelete("{ids}")]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        [ProducesResponseType(StatusCodes.Status200OK)]

        public async Task<ActionResult> Delete(string ids)
        {
            string IdsTrim = "";
            foreach (string item in ids.Split(',').ToList().Where(x => !string.IsNullOrEmpty(x)).ToArray())
            {
                IdsTrim += item.Trim() + ",";
            }
            string[] lids = IdsTrim.Split(',').ToList()
           .Where(x => !string.IsNullOrEmpty(x)).ToArray();
            return Ok(await servicioEventoTransferencia.Eliminar(lids).ConfigureAwait(false));
        }
    }
}