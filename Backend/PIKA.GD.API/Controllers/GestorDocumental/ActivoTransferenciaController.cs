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
    public class ActivoTransferenciaController : ACLController
    {
        private readonly ILogger<ActivoTransferenciaController> logger;
        private IServicioActivoTransferencia servicioActivoTransferencia;
        private IProveedorMetadatos<ActivoTransferencia> metadataProvider;
        public ActivoTransferenciaController(ILogger<ActivoTransferenciaController> logger,
            IProveedorMetadatos<ActivoTransferencia> metadataProvider,
            IServicioActivoTransferencia servicioActivoTransferencia)
        {
            this.logger = logger;
            this.servicioActivoTransferencia = servicioActivoTransferencia;
            this.metadataProvider = metadataProvider;
        }

        [HttpGet("metadata", Name = "MetadataActivoTransferencia")]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        public async Task<ActionResult<MetadataInfo>> GetMetadata([FromQuery]Consulta query = null)
        {
            return Ok(await metadataProvider.Obtener().ConfigureAwait(false));
        }



        [HttpPost]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        public async Task<ActionResult<ActivoTransferencia>> Post([FromBody]ActivoTransferencia entidad)
        {
            entidad = await servicioActivoTransferencia.CrearAsync(entidad).ConfigureAwait(false);
            return Ok(CreatedAtAction("GetActivoTransferencia", new { ActivoId = entidad.ActivoId }, entidad).Value);
        }


        [HttpPut("{ActivoId}")]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        public async Task<IActionResult> Put(string ActivoId, [FromBody]ActivoTransferencia entidad)
        {
            var x = ObtieneFiltrosIdentidad();


            if (ActivoId != entidad.ActivoId)
            {
                return BadRequest();
            }

            await servicioActivoTransferencia.ActualizarAsync(entidad).ConfigureAwait(false);
            return NoContent();

        }


        [HttpGet("page", Name = "GetPageActivoTransferencia")]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        public async Task<ActionResult<IEnumerable<ActivoTransferencia>>> GetPage([FromQuery]Consulta query = null)
        {
            ///Añade las propiedaes del contexto para el filtro de ACL vía ACL Controller
            query.Filtros.AddRange(ObtieneFiltrosIdentidad());
            var data = await servicioActivoTransferencia.ObtenerPaginadoAsync(query).ConfigureAwait(false);
            return Ok(data.Elementos.ToList<ActivoTransferencia>());
        }


        [HttpGet("{ActivoId}")]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        public async Task<ActionResult<ActivoTransferencia>> Get(string ActivoId)
        {
            var o = await servicioActivoTransferencia.UnicoAsync(x => x.ActivoId == ActivoId).ConfigureAwait(false);
            if (o != null) return Ok(o);
            return NotFound(ActivoId);
        }




        [HttpDelete]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        public async Task<ActionResult> Delete([FromBody]string[] ActivoId)
        {
            return Ok(await servicioActivoTransferencia.Eliminar(ActivoId).ConfigureAwait(false));
        }
    }
}