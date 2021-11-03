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
    public class ValidadorNumeroController : ACLController
    {
        private ILogger<ValidadorNumeroController> logger;
        private IServicioValidadorNumero servicioValidadorNumero;
        private IProveedorMetadatos<ValidadorNumero> metadataProvider;
        public ValidadorNumeroController(ILogger<ValidadorNumeroController> logger,
             IProveedorMetadatos<ValidadorNumero> metadataProvider,
             IServicioValidadorNumero servicioValidadorNumero)
        {
            this.logger = logger;
            this.servicioValidadorNumero = servicioValidadorNumero;
            this.metadataProvider = metadataProvider;
        }

        [HttpGet("metadata", Name = "MetadataValidadorNumero")]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        public async Task<ActionResult<MetadataInfo>> GetMetadata([FromQuery]Consulta query = null)
        {
            return Ok(await metadataProvider.Obtener().ConfigureAwait(false));
        }



        [HttpPost]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        public async Task<ActionResult<ValidadorNumero>> Post([FromBody]ValidadorNumero entidad)
        {

            
            entidad = await servicioValidadorNumero.CrearAsync(entidad).ConfigureAwait(false);
            return Ok(CreatedAtAction("GetValidadorNumero", new { id = entidad.Id }, entidad).Value);
        }


        [HttpPut("{id}")]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        public async Task<IActionResult> Put(string id, [FromBody]ValidadorNumero entidad)
        {
            var x = ObtieneFiltrosIdentidad();

            if (id != entidad.PropiedadId)
            {
                return BadRequest();
            }
            await servicioValidadorNumero.ActualizarAsync(entidad).ConfigureAwait(false);
            return NoContent();

        }


        [HttpGet("page", Name = "GetPageValidadorNumero")]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        public async Task<ActionResult<Paginado<ValidadorNumero>>> GetPage([ModelBinder(typeof(GenericDataPageModelBinder))][FromQuery]Consulta query = null)
        {
            ///Añade las propiedaes del contexto para el filtro de ACL vía ACL Controller
            var data = await servicioValidadorNumero.ObtenerPaginadoAsync(
                   Query: query,
                   include: null)
                   .ConfigureAwait(false);

            return Ok(data);
        }




        [HttpGet("{id}")]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        public async Task<ActionResult<ValidadorNumero>> Get(string id)
        {
            var o = await servicioValidadorNumero.UnicoAsync(x => x.PropiedadId == id).ConfigureAwait(false);
            if (o != null) return Ok(o);
            return NotFound(id);
        }




        [HttpDelete("{id}")]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        public async Task<ActionResult> Delete([FromBody]string id)
        {
            string IdsTrim = "";
            foreach (string item in id.Split(',').ToList().Where(x => !string.IsNullOrEmpty(x)).ToArray())
            {
                IdsTrim += item.Trim() + ",";
            }
            string[] lids = IdsTrim.Split(',').ToList()
           .Where(x => !string.IsNullOrEmpty(x)).ToArray();
            return Ok(await servicioValidadorNumero.Eliminar(lids).ConfigureAwait(false));

        }
    }

}