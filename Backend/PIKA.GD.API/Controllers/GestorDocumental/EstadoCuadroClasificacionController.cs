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
    public class EstadoCuadroClasificacionController : ACLController
    {
        private readonly ILogger<EstadoCuadroClasificacionController> logger;
        private IServicioEstadoCuadroClasificacion servicioEstado;
        private IProveedorMetadatos<EstadoCuadroClasificacion> metadataProvider;
        public EstadoCuadroClasificacionController(ILogger<EstadoCuadroClasificacionController> logger,
            IProveedorMetadatos<EstadoCuadroClasificacion> metadataProvider,
            IServicioEstadoCuadroClasificacion servicioEstado)
        {
            this.logger = logger;
            this.servicioEstado = servicioEstado;
            this.metadataProvider = metadataProvider;
        }

        [HttpGet("metadata", Name = "MetadataEstadoCuadroClasificacion")]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        public async Task<ActionResult<MetadataInfo>> GetMetadata([FromQuery]Consulta query = null)
        {
            return Ok(await metadataProvider.Obtener().ConfigureAwait(false));
        }



        [HttpPost]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        public async Task<ActionResult<EstadoCuadroClasificacion>> Post([FromBody]EstadoCuadroClasificacion entidad)
        {
            entidad = await servicioEstado.CrearAsync(entidad).ConfigureAwait(false);
            return Ok(CreatedAtAction("GetEstadoCuadroClasificacion", new { id = entidad.Id }, entidad).Value);
        }


        [HttpPut("{id}")]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        public async Task<IActionResult> Put(string id, [FromBody]EstadoCuadroClasificacion entidad)
        {
            var x = ObtieneFiltrosIdentidad();


            if (id != entidad.Id)
            {
                return BadRequest();
            }

            await servicioEstado.ActualizarAsync(entidad).ConfigureAwait(false);
            return NoContent();

        }


        [HttpGet("page", Name = "GetPageEstadoCuadroClasificacion")]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        public async Task<ActionResult<IEnumerable<EstadoCuadroClasificacion>>> GetPage([FromQuery]Consulta query = null)
        {
            ///Añade las propiedaes del contexto para el filtro de ACL vía ACL Controller
            query.Filtros.AddRange(ObtieneFiltrosIdentidad());
            var data = await servicioEstado.ObtenerPaginadoAsync(query).ConfigureAwait(false);
            return Ok(data.Elementos.ToList<EstadoCuadroClasificacion>());
        }


        [HttpGet("{id}")]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        public async Task<ActionResult<EstadoCuadroClasificacion>> Get(string id)
        {
            var o = await servicioEstado.UnicoAsync(x => x.Id == id).ConfigureAwait(false);
            if (o != null) return Ok(o);
            return NotFound(id);
        }




        [HttpDelete("{ids}")]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult> Delete(string ids)
        {
            string[] lids = ids.Split(',').ToList()
               .Where(x => !string.IsNullOrEmpty(x)).ToArray();
            return Ok(await servicioEstado.Eliminar(lids).ConfigureAwait(false));
        }


        [HttpGet("pares", Name = "GetParesEstadoCuadroClasificacion")]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<List<ValorListaOrdenada>>> GetPares(
        [ModelBinder(typeof(GenericDataPageModelBinder))][FromQuery] Consulta query = null)
        {
            var data = await servicioEstado.ObtenerParesAsync(query)
                .ConfigureAwait(false);

            return Ok(data);
        }

        [HttpGet("pares/{ids}", Name = "GetParesEstadoCuadroClasificacionPorId")]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<List<ValorListaOrdenada>>> GetParesporId(
              string ids)
        {

            List<string> lids = ids.Split(',').ToList()
               .Where(x => !string.IsNullOrEmpty(x)).ToList();
            var data = await servicioEstado.ObtenerParesPorId(lids)
                .ConfigureAwait(false);

            return Ok(data);
        }

    }
}