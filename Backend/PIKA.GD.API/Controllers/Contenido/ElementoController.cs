using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PIKA.GD.API.Filters;
using PIKA.GD.API.Model;
using PIKA.Modelo.Contenido;
using PIKA.Modelo.Metadatos;
using PIKA.Servicio.Contenido.Interfaces;
using RepositorioEntidades;


namespace PIKA.GD.API.Controllers.Contenido
{

    [Authorize]
    [ApiVersion("1.0")]
    [ApiController]
    [Route("api/v{version:apiVersion}/cont/[controller]")]
    public class ElementoController : ACLController
    {

        private ILogger<ElementoController> logger;
        private IServicioElemento servicioEntidad;
        private IProveedorMetadatos<Elemento> metadataProvider;
        public ElementoController(ILogger<ElementoController> logger,
            IProveedorMetadatos<Elemento> metadataProvider,
            IServicioElemento servicioEntidad)
        {
            this.logger = logger;
            this.servicioEntidad = servicioEntidad;
            this.metadataProvider = metadataProvider;
        }

        [HttpGet("metadata", Name = "MetadataElemento")]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        public async Task<ActionResult<MetadataInfo>> GetMetadata([FromQuery]Consulta query = null)
        {
            return Ok(await metadataProvider.Obtener().ConfigureAwait(false));
        }



        [HttpPost]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        public async Task<ActionResult<Elemento>> Post([FromBody]Elemento entidad)
        {
            entidad = await servicioEntidad.CrearAsync(entidad).ConfigureAwait(false);
            return Ok(CreatedAtAction("GetElemento", new { id = entidad.Id }, entidad).Value);
        }


        [HttpPut("{id}")]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        public async Task<IActionResult> Put(string id, [FromBody]Elemento entidad)
        {
            var x = ObtieneFiltrosIdentidad();


            if (id != entidad.Id)
            {
                return BadRequest();
            }

            await servicioEntidad.ActualizarAsync(entidad).ConfigureAwait(false);
            return NoContent();

        }


        [HttpGet("page", Name = "GetPageElemento")]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        public async Task<ActionResult<IEnumerable<Elemento>>> GetPage([ModelBinder(typeof(GenericDataPageModelBinder))][FromQuery]Consulta query = null)
        {
            ///Añade las propiedaes del contexto para el filtro de ACL vía ACL Controller
            query.Filtros.AddRange(ObtieneFiltrosIdentidad());
            var data = await servicioEntidad.ObtenerPaginadoAsync(query).ConfigureAwait(false);
           
            return Ok(data.Elementos.ToList<Elemento>());
        }


        [HttpGet("{id}")]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        public async Task<ActionResult<Elemento>> Get(string id)
        {
            var o = await servicioEntidad.UnicoAsync(x => x.Id == id).ConfigureAwait(false);
            if (o != null) return Ok(o);
            return NotFound(id);
        }




        //[HttpDelete("{id}")]
        [HttpDelete]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        public async Task<ActionResult> Delete([FromBody]string[] id)
        {
            return Ok(await servicioEntidad.Eliminar(id).ConfigureAwait(false));
        }

    }
}