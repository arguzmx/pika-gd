using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PIKA.GD.API.Filters;
using PIKA.GD.API.Model;
using PIKA.Modelo.GestorDocumental;
using PIKA.Modelo.Metadatos;
using PIKA.Servicio.GestionDocumental.Interfaces;
using RepositorioEntidades;


namespace PIKA.GD.API.Controllers.GestorDocumental
{
    [Authorize]
    [ApiVersion("1.0")]
    [ApiController]
    [Route("api/v{version:apiVersion}/gd/[controller]")]
    public class AlmacenController : ACLController
    {
        private readonly ILogger<AlmacenController> logger;
        private IServicioAlmacenArchivo servicioAlmacen;
        private IProveedorMetadatos<AlmacenArchivo> metadataProvider;
        public AlmacenController(ILogger<AlmacenController> logger,
            IProveedorMetadatos<AlmacenArchivo> metadataProvider,
            IServicioAlmacenArchivo servicioAlmacen)
        {
            this.logger = logger;
            this.servicioAlmacen = servicioAlmacen;
            this.metadataProvider = metadataProvider;
        }

        [HttpGet("metadata", Name = "MetadataAlmacen")]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        public async Task<ActionResult<MetadataInfo>> GetMetadata([FromQuery]Consulta query = null)
        {
            return Ok(await metadataProvider.Obtener().ConfigureAwait(false));
        }



        [HttpPost]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        public async Task<ActionResult<AlmacenArchivo>> Post([FromBody]AlmacenArchivo entidad)
        {
            entidad = await servicioAlmacen.CrearAsync(entidad).ConfigureAwait(false);
            return Ok(CreatedAtAction("GetAlmacen", new { id = entidad.Id }, entidad).Value);
        }


        [HttpPut("{id}")]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        public async Task<IActionResult> Put(string id, [FromBody]AlmacenArchivo entidad)
        {
            var x = ObtieneFiltrosIdentidad();


            if (id != entidad.Id)
            {
                return BadRequest();
            }

            await servicioAlmacen.ActualizarAsync(entidad).ConfigureAwait(false);
            return NoContent();

        }


        [HttpGet("page", Name = "GetPageAlmacen")]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        public async Task<ActionResult<IEnumerable<AlmacenArchivo>>> GetPage([ModelBinder(typeof(GenericDataPageModelBinder))][FromQuery]Consulta query = null)
        {
            var data = await servicioAlmacen.ObtenerPaginadoAsync(
                  Query: query,
                  include: null)
                  .ConfigureAwait(false);

            return Ok(data);
        }


        //----------------------------------------------------------------------------------------------------------
        //----------------------------------------------------------------------------------------------------------
        //----------------------------------------------------------------------------------------------------------
        //----------------------------------------------------------------------------------------------------------




        [HttpGet("{id}")]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        public async Task<ActionResult<AlmacenArchivo>> Get(string id)
        {
            var o = await servicioAlmacen.UnicoAsync(x => x.Id == id).ConfigureAwait(false);
            if (o != null) return Ok(o);
            return NotFound(id);
        }




        [HttpDelete]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        public async Task<ActionResult> Delete([FromBody]string[] id)
        {
            return Ok(await servicioAlmacen.Eliminar(id).ConfigureAwait(false));
        }
    }
}