using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PIKA.GD.API.Filters;
using PIKA.GD.API.Model;
using PIKA.Infraestructura.Comun;
using PIKA.Modelo.Metadatos;
using PIKA.Servicio.Seguridad.Interfaces;
using RepositorioEntidades;

namespace PIKA.GD.API.Controllers.Seguridad
{
    [Authorize]
    [ApiVersion("1.0")]
    [ApiController]
    [Route("api/v{version:apiVersion}/seguridad/[controller]")]
    public class TraduccionAplicacionModuloController : ACLController
    {
        private ILogger<TraduccionAplicacionModuloController> logger;
        private IServicioTraduccionAplicacionModulo servicioTraduccionAplicacionModulo;
        private IProveedorMetadatos<TraduccionAplicacionModulo> metadataProvider;
        public TraduccionAplicacionModuloController(ILogger<TraduccionAplicacionModuloController> logger,
            IProveedorMetadatos<TraduccionAplicacionModulo> metadataProvider,
            IServicioTraduccionAplicacionModulo servicioTraduccionAplicacionModulo)
        {
            this.logger = logger;
            this.servicioTraduccionAplicacionModulo = servicioTraduccionAplicacionModulo;
            this.metadataProvider = metadataProvider;
        }

        [HttpGet("metadata", Name = "MetadataTraduccionAplicacionModulo")]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        [ProducesResponseType(StatusCodes.Status200OK)]

        public async Task<ActionResult<MetadataInfo>> GetMetadata([FromQuery]Consulta query = null)
        {
            return Ok(await metadataProvider.Obtener().ConfigureAwait(false));
        }



        [HttpPost]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        [ProducesResponseType(StatusCodes.Status200OK)]

        public async Task<ActionResult<TraduccionAplicacionModulo>> Post([FromBody]TraduccionAplicacionModulo entidad)
        {
            entidad = await servicioTraduccionAplicacionModulo.CrearAsync(entidad).ConfigureAwait(false);
            return Ok(CreatedAtAction("GetTraduccionAplicacionModulo", new { id = entidad.ModuloId }, entidad).Value);
        }


        [HttpPut("{id}")]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]

        public async Task<IActionResult> Put(string id, [FromBody]TraduccionAplicacionModulo entidad)
        {
            var x = ObtieneFiltrosIdentidad();


            if (id != entidad.Id)
            {
                return BadRequest();
            }

            await servicioTraduccionAplicacionModulo.ActualizarAsync(entidad).ConfigureAwait(false);
            return NoContent();

        }


        [HttpGet("page", Name = "GetPageTraduccionAplicacionModulo")]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        [ProducesResponseType(StatusCodes.Status200OK)]

        public async Task<ActionResult<IEnumerable<TraduccionAplicacionModulo>>> GetPage([ModelBinder(typeof(GenericDataPageModelBinder))][FromQuery]Consulta query = null)
        {
            
            ///Añade las propiedaes del contexto para el filtro de ACL vía ACL Controller
            query.Filtros.AddRange(ObtieneFiltrosIdentidad());
            var data = await servicioTraduccionAplicacionModulo.ObtenerPaginadoAsync(
           Query: query,
           include: null)
           .ConfigureAwait(false);

            return Ok(data);
        }



        [HttpGet("{id}")]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        [ProducesResponseType(StatusCodes.Status200OK)]

        public async Task<ActionResult<TraduccionAplicacionModulo>> Get(string id)
        {
            var o = await servicioTraduccionAplicacionModulo.UnicoAsync(x => x.ModuloId == id).ConfigureAwait(false);
            if (o != null) return Ok(o);
            return NotFound(id);
        }




        [HttpDelete("{id}")]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        [ProducesResponseType(StatusCodes.Status200OK)]

        public async Task<ActionResult> Delete([FromBody]string id)
        {
            string IdsTrim = "";
            foreach (string item in id.Split(',').ToList().Where(x => !string.IsNullOrEmpty(x)).ToArray())
            {
                IdsTrim += item.Trim() + ",";
            }
            string[] lids = IdsTrim.Split(',').ToList()
           .Where(x => !string.IsNullOrEmpty(x)).ToArray();
            return Ok(await servicioTraduccionAplicacionModulo.Eliminar(lids).ConfigureAwait(false));

        }
    }
}