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
using RepositorioEntidades.DatatablesPlugin;

namespace PIKA.GD.API.Controllers.Metadatos
{
    [Authorize]
    [ApiVersion("1.0")]
    [ApiController]
    [Route("api/v{version:apiVersion}/metadatos/[controller]")]
    public class TipoAlmacenMetadatosController : ACLController
    {
        private ILogger<TipoAlmacenMetadatosController> logger;
        private IServicioTipoAlmacenMetadatos servicioTipoAlmacenMetadatos;
        private IProveedorMetadatos<TipoAlmacenMetadatos> metadataProvider;
        public TipoAlmacenMetadatosController(ILogger<TipoAlmacenMetadatosController> logger,
            IProveedorMetadatos<TipoAlmacenMetadatos> metadataProvider,
            IServicioTipoAlmacenMetadatos servicioTipoAlmacenMetadatos)
        {
            this.logger = logger;
            this.servicioTipoAlmacenMetadatos = servicioTipoAlmacenMetadatos;
            this.metadataProvider = metadataProvider;
        }

        [HttpGet("metadata", Name = "MetadataTipoAlmacenMetadatos")]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        public async Task<ActionResult<MetadataInfo>> GetMetadata([FromQuery]Consulta query = null)
        {
            return Ok(await metadataProvider.Obtener().ConfigureAwait(false));
        }



        [HttpPost]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        public async Task<ActionResult<TipoAlmacenMetadatos>> Post([FromBody]TipoAlmacenMetadatos entidad)
        {

            entidad = await servicioTipoAlmacenMetadatos.CrearAsync(entidad).ConfigureAwait(false);
            return Ok(CreatedAtAction("GetTipoAlmacenMetadatos", new { id = entidad.Id }, entidad).Value);
        }


        [HttpPut("{id}")]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        public async Task<IActionResult> Put(string id, [FromBody]TipoAlmacenMetadatos entidad)
        {
            var x = ObtieneFiltrosIdentidad();


            if (id != entidad.Id)
            {
                return BadRequest();
            }

            await servicioTipoAlmacenMetadatos.ActualizarAsync(entidad).ConfigureAwait(false);
            return NoContent();

        }


        [HttpGet("page", Name = "GetPageTipoAlmacenMetadatos")]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        public async Task<ActionResult<IEnumerable<TipoAlmacenMetadatos>>> GetPage([ModelBinder(typeof(GenericDataPageModelBinder))][FromQuery]Consulta query = null)
        {
            ///Añade las propiedaes del contexto para el filtro de ACL vía ACL Controller
            query.Filtros.AddRange(ObtieneFiltrosIdentidad());
            var data = await servicioTipoAlmacenMetadatos.ObtenerPaginadoAsync(query).ConfigureAwait(false);
            return Ok(data.Elementos.ToList<TipoAlmacenMetadatos>());
        }




        [HttpGet("{id}")]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        public async Task<ActionResult<TipoAlmacenMetadatos>> Get(string id)
        {
            var o = await servicioTipoAlmacenMetadatos.UnicoAsync(x => x.Id == id).ConfigureAwait(false);
            if (o != null) return Ok(o);
            return NotFound(id);
        }




        [HttpDelete("{id}")]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        public async Task<ActionResult> Delete([FromBody]string[] id)
        {
            await servicioTipoAlmacenMetadatos.Eliminar(id).ConfigureAwait(false);
            return NoContent();
        }
    }
}