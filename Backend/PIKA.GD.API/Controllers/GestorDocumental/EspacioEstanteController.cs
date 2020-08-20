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
using PIKA.Modelo.GestorDocumental.Topologia;
using PIKA.Modelo.Metadatos;
using PIKA.Servicio.GestionDocumental.Interfaces;
using RepositorioEntidades;


namespace PIKA.GD.API.Controllers.GestorDocumental
{
    [Authorize]
    [ApiVersion("1.0")]
    [ApiController]
    [Route("api/v{version:apiVersion}/gd/almacen/{AlmacenId}/estante/{EstanteId}/[controller]")]
    public class EspacioEstanteController : ACLController
    {
        private readonly ILogger<EspacioEstanteController> logger;
        private IServicioEspacioEstante servicioEspacioEstante;
        private IProveedorMetadatos<EspacioEstante> metadataProvider;
        public EspacioEstanteController(ILogger<EspacioEstanteController> logger,
            IProveedorMetadatos<EspacioEstante> metadataProvider,
            IServicioEspacioEstante servicioEspacioEstante)
        {
            this.logger = logger;
            this.servicioEspacioEstante = servicioEspacioEstante;
            this.metadataProvider = metadataProvider;
        }

        [HttpGet("metadata", Name = "MetadataEspacioEstante")]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        public async Task<ActionResult<MetadataInfo>> GetMetadata([FromQuery]Consulta query = null)
        {
            return Ok(await metadataProvider.Obtener().ConfigureAwait(false));
        }



        [HttpPost]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        public async Task<ActionResult<EspacioEstante>> Post([FromBody]EspacioEstante entidad)
        {
            entidad = await servicioEspacioEstante.CrearAsync(entidad).ConfigureAwait(false);
            return Ok(CreatedAtAction("GetEspacioEstante", new { id = entidad.Id }, entidad).Value);
        }


        [HttpPut("{id}")]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        public async Task<IActionResult> Put(string id, [FromBody]EspacioEstante entidad)
        {
            var x = ObtieneFiltrosIdentidad();


            if (id != entidad.Id)
            {
                return BadRequest();
            }

            await servicioEspacioEstante.ActualizarAsync(entidad).ConfigureAwait(false);
            return NoContent();

        }


        [HttpGet("page", Name = "GetPageEspacioEstante")]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        public async Task<ActionResult<IEnumerable<EspacioEstante>>> GetPage([ModelBinder(typeof(GenericDataPageModelBinder))][FromQuery]Consulta query = null)
        {
            ///Añade las propiedaes del contexto para el filtro de ACL vía ACL Controller
            var data = await servicioEspacioEstante.ObtenerPaginadoAsync(
                  Query: query,
                  include: null)
                  .ConfigureAwait(false);

            return Ok(data);
        }


        [HttpGet("{id}")]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        public async Task<ActionResult<EspacioEstante>> Get(string id)
        {
            var o = await servicioEspacioEstante.UnicoAsync(x => x.Id == id).ConfigureAwait(false);
            if (o != null) return Ok(o);
            return NotFound(id);
        }




        [HttpDelete]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        public async Task<ActionResult> Delete([FromBody]string[] id)
        {
            return Ok(await servicioEspacioEstante.Eliminar(id).ConfigureAwait(false));
        }
    }
}