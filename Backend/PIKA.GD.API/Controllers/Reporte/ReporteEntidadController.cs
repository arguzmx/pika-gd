﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Connections;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PIKA.GD.API.Filters;
using PIKA.GD.API.Model;
using PIKA.Modelo.Metadatos;
using PIKA.Modelo.Reportes;
using PIKA.Servicio.Reportes.Interfaces;
using RepositorioEntidades;

namespace PIKA.GD.API.Controllers.Reporte
{

    [Authorize]
    [ApiVersion("1.0")]
    [ApiController]
    [Route("api/v{version:apiVersion}/Repo/[controller]")]
    public class ReporteEntidadController : ACLController
    {

        private ILogger<ReporteEntidadController> logger;
        private IServicioReporteEntidad servicioEntidad;
        private IProveedorMetadatos<ReporteEntidad> metadataProvider;
        public ReporteEntidadController(ILogger<ReporteEntidadController> logger,
            IProveedorMetadatos<ReporteEntidad> metadataProvider,
            IServicioReporteEntidad servicioEntidad)
        {
            this.logger = logger;
            this.servicioEntidad = servicioEntidad;
            this.metadataProvider = metadataProvider;
        }

        /// <summary>
        /// Obtiene los metadatos relacionados con la entidad Dominio
        /// </summary>
        /// <returns></returns>
        [HttpGet("metadata", Name = "MetadataReporteEntidad")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        public async Task<ActionResult<MetadataInfo>> GetMetadata()
        {
            return Ok(await metadataProvider.Obtener().ConfigureAwait(false));
        }


        /// <summary>
        /// Añade una nueva entidad del tipo Dominio
        /// </summary>
        /// <param name = "entidad" ></ param >
        /// < returns ></ returns >
        [HttpPost]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<ReporteEntidad>> Post([FromBody] ReporteEntidad entidad)
        {
            entidad = await servicioEntidad.CrearAsync(entidad).ConfigureAwait(false);
            return Ok(CreatedAtAction("GetReporteEntidad", new { id = entidad.Id }, entidad).Value);
        }

        /// <summary>
        /// Actualoza uan entidad dominio, el Id debe incluirse en el querystring así como en 
        /// el serializado para la petición PUT
        /// </summary>
        /// <param name="id">Identificador único del dominio</param>
        /// <param name="entidad">Datos serialziados del dominio</param>
        /// <returns></returns>
        [HttpPut("{id}")]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> Put(string id, [FromBody] ReporteEntidad entidad)
        {
            var x = ObtieneFiltrosIdentidad();

            if (id.Trim() != entidad.Id.Trim())
            {
                return BadRequest();
            }

            await servicioEntidad.ActualizarAsync(entidad).ConfigureAwait(false);
            return NoContent();

        }

        /// <summary>
        /// Obtiene una página de datos del dominio 
        /// </summary>
        /// <param name="query">Consulta para el paginado</param>
        /// <returns></returns>
        [HttpGet("page", Name = "GetPageReporteEntidad")]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<Paginado<ReporteEntidad>>> GetPage([ModelBinder(typeof(GenericDataPageModelBinder))][FromQuery] Consulta query = null)
        {
            ///Añade las propiedaes del contexto para el filtro de ACL vía ACL Controller
            var data = await servicioEntidad.ObtenerPaginadoAsync(
                 Query: query,
                 include: null)
                 .ConfigureAwait(false);

            return Ok(data);
        }


        /// <summary>
        /// Obtiene un dominio en base al Id único
        /// </summary>
        /// <param name="id">Id único del dominio</param>
        /// <returns></returns>
        [HttpGet("{id}")]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ReporteEntidad>> Get(string id)
        {
            var o = await servicioEntidad.UnicoAsync(x => x.Id == id.Trim()).ConfigureAwait(false);
            if (o != null) return Ok(o);
            return NotFound(id);
        }



        /// <summary>
        /// Marca como eliminados una lista de dominiios en base al arreglo de identificadores recibidos
        /// </summary>
        /// <param name="ids">Arreglo de identificadores string</param>
        /// <returns></returns>
        [HttpDelete("{ids}")]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult> Delete(string ids)
        {
            string IdsTrim = "";
            foreach (string item in ids.Split(',').ToList().Where(x => !string.IsNullOrEmpty(x)).ToArray())
            {
                IdsTrim += item.Trim() + ",";
            }
            string[] lids = IdsTrim.Split(',').ToList()
           .Where(x => !string.IsNullOrEmpty(x)).ToArray();
            return Ok(await servicioEntidad.Eliminar(lids).ConfigureAwait(false));
        }


        /// <summary>
        /// Restaura una lista dede dominiios eliminados en base al arreglo de identificadores recibidos
        /// </summary>
        /// <param name="ids">Arreglo de identificadores string</param>
        /// <returns></returns>
        [HttpPatch("restaurar/{ids}", Name = "restaurarReporteEntidad")]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult> Undelete(string ids)
        {
            string IdsTrim = "";
            foreach (string item in ids.Split(',').ToList().Where(x => !string.IsNullOrEmpty(x)).ToArray())
            {
                IdsTrim += item.Trim() + ",";
            }
            string[] lids = IdsTrim.Split(',').ToList()
           .Where(x => !string.IsNullOrEmpty(x)).ToArray();
            return Ok(await servicioEntidad.Eliminar(lids).ConfigureAwait(false));

        }

    }
}