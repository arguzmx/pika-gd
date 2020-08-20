using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PIKA.GD.API.Filters;
using PIKA.GD.API.Model;
using PIKA.Modelo.Metadatos;
using PIKA.Servicio.Metadatos.Interfaces;
using RepositorioEntidades;


namespace PIKA.GD.API.Controllers.Metadatos
{
    [Authorize]
    [ApiVersion("1.0")]
    [ApiController]
    [Route("api/v{version:apiVersion}/metadatos/[controller]")]
    public class PropiedadPlantillaController : ACLController
    {

        private ILogger<PropiedadPlantillaController> logger;
        private IServicioPropiedadPlantilla servicioPropiedadPlantilla;
        private IProveedorMetadatos<PropiedadPlantilla> metadataProvider;
        public PropiedadPlantillaController(ILogger<PropiedadPlantillaController> logger,
            IProveedorMetadatos<PropiedadPlantilla> metadataProvider,
            IServicioPropiedadPlantilla servicioPropiedadPlantilla
            )
        {
            this.logger = logger;
            this.servicioPropiedadPlantilla = servicioPropiedadPlantilla;
            this.metadataProvider = metadataProvider;
        }

        [HttpGet("metadata", Name = "MetadataPropiedadPlantilla")]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        public async Task<ActionResult<MetadataInfo>> GetMetadata([FromQuery]Consulta query = null)
        {
            return Ok(await metadataProvider.Obtener().ConfigureAwait(false));
        }



        [HttpPost]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        public async Task<ActionResult<PropiedadPlantilla>> Post([FromBody]PropiedadPlantilla entidad)
        {


            entidad = await servicioPropiedadPlantilla.CrearAsync(entidad).ConfigureAwait(false);


            if (entidad.TipoDato != null)
            {

            }

            return Ok(CreatedAtAction("GetPropiedadPlantilla", new { id = entidad.Id }, entidad).Value);
        }


        [HttpPut("{id}")]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        public async Task<IActionResult> Put(string id, [FromBody]PropiedadPlantilla entidad)
        {
            var x = ObtieneFiltrosIdentidad();


            if (id != entidad.Id)
            {
                return BadRequest();
            }

            await servicioPropiedadPlantilla.ActualizarAsync(entidad).ConfigureAwait(false);
            return NoContent();

        }


        [HttpGet("page", Name = "GetPagePropiedadPlantilla")]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        public async Task<ActionResult<IEnumerable<PropiedadPlantilla>>> GetPage([ModelBinder(typeof(GenericDataPageModelBinder))][FromQuery]Consulta query = null)
        {
            ///Añade las propiedaes del contexto para el filtro de ACL vía ACL Controller
            var data = await servicioPropiedadPlantilla.ObtenerPaginadoAsync(
                      Query: query,
                      include: null)
                      .ConfigureAwait(false);

            return Ok(data);
        }



        [HttpGet("{id}")]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        public async Task<ActionResult<PropiedadPlantilla>> Get(string id)
        {
            var o = await servicioPropiedadPlantilla.UnicoAsync(x => x.Id == id).ConfigureAwait(false);
            if (o != null) return Ok(o);
            return NotFound(id);
        }




        [HttpDelete("{id}")]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        public async Task<ActionResult> Delete([FromBody]string[] id)
        {
            await servicioPropiedadPlantilla.Eliminar(id).ConfigureAwait(false);
            return NoContent();
        }
    }
}