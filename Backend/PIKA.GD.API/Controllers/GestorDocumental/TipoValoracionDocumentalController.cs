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
    public class TipoValoracionDocumentalController : ACLController
    {
        private ILogger<TipoValoracionDocumentalController> logger;
        private IServicioTipoValoracionDocumental servicioEntidad;
        private IProveedorMetadatos<TipoValoracionDocumental> metadataProvider;
        public TipoValoracionDocumentalController(ILogger<TipoValoracionDocumentalController> logger,
            IProveedorMetadatos<TipoValoracionDocumental> metadataProvider,
            IServicioTipoValoracionDocumental servicioEntidad)
        {
            this.logger = logger;
            this.servicioEntidad = servicioEntidad;
            this.metadataProvider = metadataProvider;
        }


        /// <summary>
        /// Obtiene los metadatos relacionados con la entidad Tipo de Valoracion Documental
        /// </summary>
        /// <returns></returns>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpGet("metadata", Name = "MetadataTipoValoracionDocumental")]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        public async Task<ActionResult<MetadataInfo>> GetMetadata([FromQuery] Consulta query = null)
        {
            return Ok(await metadataProvider.Obtener().ConfigureAwait(false));
        }



        /// <summary>
        /// Añade una nueva entidad de Tipo de Valoracion Documental
        /// </summary>
        /// <param name="entidad"></param>
        /// <returns></returns>
        [HttpPost]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<TipoValoracionDocumental>> Post([FromBody] TipoValoracionDocumental entidad)
        {
            entidad = await servicioEntidad.CrearAsync(entidad).ConfigureAwait(false);
            return Ok(CreatedAtAction("GetTipoValoracionDocumental", new { id = entidad.Id }, entidad).Value);
        }


        /// <summary>
        /// Actualiza unq entidad Tipo de Valoracion Documental , el Id debe incluirse en el Querystring así como en 
        /// el serializado para la petición PUT
        /// </summary>
        /// <param name="id">Identificador único del dominio</param>
        /// <param name="entidad">Datos serialziados de la OU</param>
        /// <returns></returns>
        [HttpPut("{id}")]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> Put(string id, [FromBody] TipoValoracionDocumental entidad)
        {

            if (id != entidad.Id)
            {
                return BadRequest();
            }

            await servicioEntidad.ActualizarAsync(entidad).ConfigureAwait(false);
            return NoContent();

        }


        /// <summary>
        /// Devulve una lista de Tipo de Disposicion de Documental asociadas al objeto del tipo especificado
        /// </summary>
        /// <param name="query">Consulta para la paginación y búsqueda</param>
        /// <returns></returns>
        [HttpGet("page", Name = "GetPageTipoValoracionDocumental")]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<Paginado<TipoValoracionDocumental>>> GetPage(
            [ModelBinder(typeof(GenericDataPageModelBinder))][FromQuery] Consulta query = null)
        {
            var data = await servicioEntidad.ObtenerPaginadoAsync(
                Query: query,
                include: null)
                .ConfigureAwait(false);

            return Ok(data);
        }



        /// <summary>
        /// Obtiene un país en base al Id único
        /// </summary>
        /// <param name="id">Id único del país</param>
        /// <returns></returns>
        [HttpGet("{id}")]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<TipoValoracionDocumental>> Get(string id)
        {
            var o = await servicioEntidad.UnicoAsync(
                predicado: x => x.Id == id)
                .ConfigureAwait(false);

            if (o != null) return Ok(o);
            return NotFound(id);
        }




        /// <summary>
        /// Elimina de manera permanente un Tipo de Disposicion de Documental en base al arreglo de identificadores recibidos
        /// </summary>
        /// <param name="ids">Arreglo de identificadores string</param>
        /// <returns></returns>
        [HttpDelete("{ids}")]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult> Delete(string ids)
        {
            string[] lids = ids.Split(',').ToList()
                .Where(x => !string.IsNullOrEmpty(x)).ToArray();
            return Ok(await servicioEntidad.Eliminar(lids).ConfigureAwait(false));
        }
        /// <summary>
        /// Obtiene una lista de Tipo de Disposicion de Documental en base a los parámetros de consulta
        /// </summary>
        /// <param name="query">Query de busqueda a la base de datos</param>
        /// <returns></returns>

        [HttpGet("pares", Name = "GetParesTipoValoracionDocumental")]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<List<ValorListaOrdenada>>> GetPares(
        [ModelBinder(typeof(GenericDataPageModelBinder))][FromQuery] Consulta query = null)
        {
            var data = await servicioEntidad.ObtenerParesAsync(query)
                .ConfigureAwait(false);

            return Ok(data);
        }
        /// <summary>
        /// Obtiene una lista de Tipo de Disposicion de Documental en base a con el parámetro ID de consulta
        /// </summary>
        /// <param name="ids">parametro Id para consulta a la base de datos</param>
        /// <returns></returns>

        [HttpGet("pares/{ids}", Name = "GetParesTipoValoracionDocumentalporId")]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<List<ValorListaOrdenada>>> GetParesporId(
              string ids)
        {

            List<string> lids = ids.Split(',').ToList()
               .Where(x => !string.IsNullOrEmpty(x)).ToList();
            var data = await servicioEntidad.ObtenerParesPorId(lids)
                .ConfigureAwait(false);

            return Ok(data);
        }

    }
}
