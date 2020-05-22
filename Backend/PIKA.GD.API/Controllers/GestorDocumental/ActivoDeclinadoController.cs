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
    [Route("api/v{version:apiVersion}/gd/Transferencia/{TransferenciaId}/[controller]")]
    public class ActivoDeclinadoController : ACLController
    {
        private readonly ILogger<ActivoDeclinadoController> logger;
        private IServicioActivoDeclinado servicioActivoDeclinado;
        private IProveedorMetadatos<ActivoDeclinado> metadataProvider;
        public ActivoDeclinadoController(ILogger<ActivoDeclinadoController> logger,
            IProveedorMetadatos<ActivoDeclinado> metadataProvider,
            IServicioActivoDeclinado servicioActivoDeclinado)
        {
            this.logger = logger;
            this.servicioActivoDeclinado = servicioActivoDeclinado;
            this.metadataProvider = metadataProvider;
        }

        [HttpGet("metadata", Name = "MetadataActivoDeclinado")]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        public async Task<ActionResult<MetadataInfo>> GetMetadata([FromQuery]Consulta query = null)
        {
            return Ok(await metadataProvider.Obtener().ConfigureAwait(false));
        }



        [HttpPost]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        public async Task<ActionResult<ActivoDeclinado>> Post([FromBody]ActivoDeclinado entidad)
        {
            entidad = await servicioActivoDeclinado.CrearAsync(entidad).ConfigureAwait(false);
            return Ok(CreatedAtAction("GetActivoDeclinado", new { ActivoId = entidad.ActivoId }, entidad).Value);
        }


        [HttpPut("{ActivoId}")]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        public async Task<IActionResult> Put(string ActivoId, [FromBody]ActivoDeclinado entidad)
        {
            var x = ObtieneFiltrosIdentidad();


            if (ActivoId != entidad.ActivoId)
            {
                return BadRequest();
            }

            await servicioActivoDeclinado.ActualizarAsync(entidad).ConfigureAwait(false);
            return NoContent();

        }


        [HttpGet("page", Name = "GetPageActivoDeclinado")]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        public async Task<ActionResult<IEnumerable<ActivoDeclinado>>> GetPage([FromQuery]Consulta query = null)
        {
            ///Añade las propiedaes del contexto para el filtro de ACL vía ACL Controller
            query.Filtros.AddRange(ObtieneFiltrosIdentidad());
            var data = await servicioActivoDeclinado.ObtenerPaginadoAsync(query).ConfigureAwait(false);
            return Ok(data.Elementos.ToList<ActivoDeclinado>());
        }


        [HttpGet("{ActivoId}")]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        public async Task<ActionResult<ActivoDeclinado>> Get(string ActivoId)
        {
            var o = await servicioActivoDeclinado.UnicoAsync(x => x.ActivoId == ActivoId).ConfigureAwait(false);
            if (o != null) return Ok(o);
            return NotFound(ActivoId);
        }




        [HttpDelete]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        public async Task<ActionResult> Delete([FromBody]string[] ActivoId)
        {
            return Ok(await servicioActivoDeclinado.Eliminar(ActivoId).ConfigureAwait(false));
        }
    }
}