using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DocumentFormat.OpenXml.Bibliography;
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
    public class PrestamoController : ACLController
    {
        private readonly ILogger<PrestamoController> logger;
        private IServicioPrestamo servicioPrestamo;
        private IProveedorMetadatos<Prestamo> metadataProvider;

        private IServicioActivoPrestamo servicioActivoPrestamo;
        private IProveedorMetadatos<ActivoPrestamo> metadataProviderActivo;
        public PrestamoController(ILogger<PrestamoController> logger,
            IProveedorMetadatos<Prestamo> metadataProvider,
            IProveedorMetadatos<ActivoPrestamo> metadataProviderActivo,
            IServicioPrestamo servicioPrestamo,
            IServicioActivoPrestamo servicioActivoPrestamo)
        {
            this.logger = logger;
            this.servicioPrestamo = servicioPrestamo;
            this.servicioActivoPrestamo = servicioActivoPrestamo;
            this.metadataProvider = metadataProvider;
            this.metadataProviderActivo = metadataProviderActivo;
        }


        #region Prestamos
        [HttpGet("metadata", Name = "MetadataPrestamo")]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        public async Task<ActionResult<MetadataInfo>> GetMetadata([FromQuery]Consulta query = null)
        {
            return Ok(await metadataProvider.Obtener().ConfigureAwait(false));
        }

        [HttpPost]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        public async Task<ActionResult<Prestamo>> Post([FromBody]Prestamo entidad)
        {
            entidad = await servicioPrestamo.CrearAsync(entidad).ConfigureAwait(false);
            return Ok(CreatedAtAction("GetPrestamo", new { id = entidad.Id }, entidad).Value);
        }

        [HttpPut("{id}")]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        public async Task<IActionResult> Put(string id, [FromBody]Prestamo entidad)
        {
            var x = ObtieneFiltrosIdentidad();


            if (id != entidad.Id)
            {
                return BadRequest();
            }

            await servicioPrestamo.ActualizarAsync(entidad).ConfigureAwait(false);
            return NoContent();

        }


        [HttpGet("page", Name = "GetPagePrestamo")]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        public async Task<ActionResult<IEnumerable<Prestamo>>> GetPage([ModelBinder(typeof(GenericDataPageModelBinder))][FromQuery]Consulta query = null)
        {
            ///Añade las propiedaes del contexto para el filtro de ACL vía ACL Controller
            var data = await servicioPrestamo.ObtenerPaginadoAsync(
                  Query: query,
                  include: null)
                  .ConfigureAwait(false);

            return Ok(data);
        }



        [HttpGet("{id}")]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        public async Task<ActionResult<Prestamo>> Get(string id)
        {
            var o = await servicioPrestamo.UnicoAsync(x => x.Id == id).ConfigureAwait(false);
            if (o != null) return Ok(o);
            return NotFound(id);
        }

        [HttpDelete("delete",Name ="DeletePrestamo")]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        public async Task<ActionResult> Delete(string ids)
        {
            string IdsTrim = "";
            foreach (string item in ids.Split(',').ToList().Where(x => !string.IsNullOrEmpty(x)).ToArray())
            {
                IdsTrim += item.Trim() + ",";
            }
            string[] Ids = IdsTrim.Split(',').ToList().Where(x => !string.IsNullOrEmpty(x)).ToArray();

            return Ok(await servicioPrestamo.Eliminar(Ids).ConfigureAwait(false));
        }
        /// <summary>
        /// Este metodo  puerga todos los elementos
        /// </summary>
        /// <returns></returns>
        [HttpDelete("purgar", Name = "DeletePurgarPrestamo")]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<Prestamo>> DeletePurgar()
        {
            return Ok(await servicioPrestamo.Purgar().ConfigureAwait(false));
        }
        #endregion

        #region Activos de prestamo

        [HttpGet("metadataActivo", Name = "MetadataActivoPrestamo")]
        [Route("{PrestamoId}/Activo/metadata")]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        public async Task<ActionResult<MetadataInfo>> GetMetadataActivo([FromQuery]Consulta query = null)
        {
            return Ok(await metadataProviderActivo.Obtener().ConfigureAwait(false));
        }

        [HttpPost]
        [Route("{PrestamoId}/Activo")]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        public async Task<ActionResult<ActivoPrestamo>> Post([FromBody]ActivoPrestamo entidad)
        {
            entidad = await servicioActivoPrestamo.CrearAsync(entidad).ConfigureAwait(false);
            return Ok(CreatedAtAction("GetActivoPrestamo", new { id = entidad.ActivoId, entidad.PrestamoId }, entidad).Value);
        }


        [HttpPut("Activo/{ActivoId}")]
        [Route("{PrestamoId}/Activo/{ActivoId}")]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        public async Task<IActionResult> Put(string PrestamoId, string ActivoId, [FromBody]ActivoPrestamo entidad)
        {
            var x = ObtieneFiltrosIdentidad();


            if (ActivoId != entidad.ActivoId || PrestamoId != entidad.PrestamoId)
            {
                return BadRequest();
            }

            await servicioActivoPrestamo.ActualizarAsync(entidad).ConfigureAwait(false);
            return NoContent();

        }


        [HttpGet("ActivoPage", Name = "GetPageActivoPrestamo")]
        [Route("{PrestamoId}/Activo/page")]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        public async Task<ActionResult<IEnumerable<ActivoPrestamo>>> GetPageActivo(string PrestamoId, [ModelBinder(typeof(GenericDataPageModelBinder))][FromQuery]Consulta query = null)
        {
            query.Filtros.Add(new FiltroConsulta() 
            { 
                Propiedad = "PrestamoId",
                Operador = "eq",
                Valor = PrestamoId 
            });
            ///Añade las propiedaes del contexto para el filtro de ACL vía ACL Controller
            var data = await servicioActivoPrestamo.ObtenerPaginadoAsync(
                       Query: query,
                       include: null)
                       .ConfigureAwait(false);

            return Ok(data);
        }


        [HttpGet("Activo/{ActivoId}")]
        [Route("{PrestamoId}/Activo/{ActivoId}/get")]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        public async Task<ActionResult<ActivoPrestamo>> Get(string ActivoId, string PrestamoId)
        {
            var o = await servicioActivoPrestamo.UnicoAsync(x => x.ActivoId == ActivoId && x.PrestamoId == PrestamoId).ConfigureAwait(false);
            if (o != null) return Ok(o);
            return NotFound(new { ActivoId, PrestamoId });
        }


        [HttpDelete("{ids}", Name ="DeleteActivo")]
        [Route("{PrestamoId}/Activo")]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        public async Task<ActionResult> DeleteActivo(string ids)
        {
            string IdsTrim = "";
            foreach (string item in ids.Split(',').ToList().Where(x => !string.IsNullOrEmpty(x)).ToArray())
            {
                IdsTrim += item.Trim() + ",";
            }
            string[] lids = IdsTrim.Split(',').ToList()
           .Where(x => !string.IsNullOrEmpty(x)).ToArray();
            return Ok(await servicioActivoPrestamo.Eliminar(lids).ConfigureAwait(false));
        }
        #endregion
    }
}