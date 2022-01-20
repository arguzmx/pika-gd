using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PIKA.GD.API.Filters;
using PIKA.GD.API.Model;
using PIKA.Modelo.Aplicacion.Tareas;
using PIKA.Modelo.Metadatos;
using PIKA.Servicio.AplicacionPlugin.Interfaces;
using RepositorioEntidades;


namespace PIKA.GD.API.Controllers.AplicacionBitacoraTarea
{
    [Authorize]
    [ApiVersion("1.0")]
    [ApiController]
    [Route("api/v{version:apiVersion}/Apli/[controller]")]
    public class BitacoraTareaController : ACLController
    {
        private ILogger<BitacoraTareaController> logger;
        private IServicioBitacoraTarea servicioBitacoraTarea;
        private IProveedorMetadatos<BitacoraTarea> metadataProvider;
        public BitacoraTareaController(ILogger<BitacoraTareaController> logger,
            IProveedorMetadatos<BitacoraTarea> metadataProvider,
            IServicioBitacoraTarea servicioBitacoraTarea)
        {
            this.logger = logger;
            this.servicioBitacoraTarea = servicioBitacoraTarea;
            this.metadataProvider = metadataProvider;
        }

        [HttpGet("metadata", Name = "MetadataBitacoraTarea")]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        public async Task<ActionResult<MetadataInfo>> GetMetadata([FromQuery]Consulta query = null)
        {
            return Ok(await metadataProvider.Obtener().ConfigureAwait(false));
        }



        [HttpPost]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        public async Task<ActionResult<BitacoraTarea>> Post([FromBody]BitacoraTarea entidad)
        {
            entidad = await servicioBitacoraTarea.CrearAsync(entidad).ConfigureAwait(false);
            return Ok(CreatedAtAction("GetBitacoraTarea", new { id = entidad.Id }, entidad).Value);

        }
      
        [HttpPut("{id}")]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        public async Task<IActionResult> Put(string id, [FromBody]BitacoraTarea entidad)
        {
            var x = ObtieneFiltrosIdentidad();


            if (id != entidad.Id)
            {
                return BadRequest();
            }

            await servicioBitacoraTarea.ActualizarAsync(entidad).ConfigureAwait(false);
            return NoContent();

        }

        [HttpGet("page/tareaautomatica/{id}", Name = "GetPageBitacoraTareaAutomatica")]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<Paginado<BitacoraTarea>>> GetPage(string id,
        [ModelBinder(typeof(GenericDataPageModelBinder))][FromQuery] Consulta query = null)
        {
            query.Filtros.Add(new FiltroConsulta() { Propiedad = "TareaId", Operador = FiltroConsulta.OP_EQ, Valor = id });
            var data = await servicioBitacoraTarea.ObtenerPaginadoAsync(
                Query: query,
                include: null)
                .ConfigureAwait(false);

            return Ok(data);
        }

        [HttpGet("{id}")]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        public async Task<ActionResult<BitacoraTarea>> Get(string id)
        {
            var o = await servicioBitacoraTarea.UnicoAsync(x => x.Id == id).ConfigureAwait(false);
            if (o != null) return Ok(o);
            return NotFound(id);
        }

    }
}