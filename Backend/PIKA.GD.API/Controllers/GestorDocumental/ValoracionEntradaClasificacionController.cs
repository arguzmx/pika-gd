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
    public class ValoracionEntradaClasificacionController : ACLController
    {
        private readonly ILogger<ValoracionEntradaClasificacionController> logger;
        private IServicioValoracionEntradaClasificacion ServicioValoracion;
        private IProveedorMetadatos<ValoracionEntradaClasificacion> metadataProvider;
        public ValoracionEntradaClasificacionController(ILogger<ValoracionEntradaClasificacionController> logger,
            IProveedorMetadatos<ValoracionEntradaClasificacion> metadataProvider,
            IServicioValoracionEntradaClasificacion ServicioValoracion)
        {
            this.logger = logger;
            this.ServicioValoracion = ServicioValoracion;
            this.metadataProvider = metadataProvider;
        }
        /// <summary>
        /// Obtiene los metadatos relacionados con la entidad Valoracion Entrada Clasificacion 
        /// </summary>
        /// <returns></returns>
        [HttpGet("metadata", Name = "MetadataValoracionEntradaClasificacion")]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        [ProducesResponseType(StatusCodes.Status200OK)]

        public async Task<ActionResult<MetadataInfo>> GetMetadata([FromQuery] Consulta query = null)
        {
            return Ok(await metadataProvider.Obtener().ConfigureAwait(false));
        }


        /// <summary>
        /// Añade una nueva entidad del tipo Valoracion Entrada Clasificacion
        /// </summary>
        /// <param name="entidad"></param>
        /// <returns></returns>
        [HttpPost]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        [ProducesResponseType(StatusCodes.Status200OK)]

        public async Task<ActionResult<ValoracionEntradaClasificacion>> Post([FromBody] ValoracionEntradaClasificacion entidad)
        {
            entidad = await ServicioValoracion.CrearAsync(entidad).ConfigureAwait(false);
            return Ok(CreatedAtAction("GetValoracionEntradaClasificacion", new { id = entidad.EntradaClasificacionId }, entidad).Value);
        }

        /// <summary>
        /// Actualiza unq entidad Valoracion Entrada Clasificacion, el Id debe incluirse en el Querystring así como en 
        /// el serializado para la petición PUT
        /// </summary>
        /// <param name="id">Identificador único del dominio</param>
        /// <param name="entidad">Datos serialziados de la OU</param>
        /// <returns></returns>
        [HttpPut("{id}")]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> Put(string id, [FromBody] ValoracionEntradaClasificacion entidad)
        {
            var x = ObtieneFiltrosIdentidad();


            if (id != entidad.EntradaClasificacionId)
            {
                return BadRequest();
            }

            await ServicioValoracion.ActualizarAsync(entidad).ConfigureAwait(false);
            return NoContent();

        }

        /// <summary>
        /// Devulve un alista de Valoracion Entrada Clasificacion asociadas al objeto del tipo especificado
        /// </summary>
        /// <param name="query">Consulta para la paginación y búsqueda</param>
        /// <returns></returns>
        [HttpGet("page", Name = "GetPageValoracionEntradaClasificacion")]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        [ProducesResponseType(StatusCodes.Status200OK)]

        public async Task<ActionResult<Paginado<ValoracionEntradaClasificacion>>> GetPage(
            [ModelBinder(typeof(GenericDataPageModelBinder))][FromQuery] Consulta query = null)
        {
            var data = await ServicioValoracion.ObtenerPaginadoAsync(
                Query: query,
                include: null)
                .ConfigureAwait(false);

            return Ok(data);
        }

        /// <summary>
        /// Obtiene un Valoracion Entrada Clasificacion en base al Id único
        /// </summary>
        /// <param name="id">Id único del Valoracion Entrada Clasificacion</param>
        /// <returns></returns>
        [HttpGet("{id}")]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ValoracionEntradaClasificacion>> Get(string id)
        {
            var o = await ServicioValoracion.UnicoAsync(x => x.EntradaClasificacionId == id).ConfigureAwait(false);
            if (o != null) return Ok(o);
            return NotFound(id);
        }


        /// <summary>
        /// Elimina de manera permanente un Valoracion Entrada Clasificacion en base al arreglo de identificadores recibidos
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
            return Ok(await ServicioValoracion.Eliminar(lids).ConfigureAwait(false));
        }


        /// <summary>
        /// Restaura una lista dede dominiios eliminados en base al arreglo de identificadores recibidos
        /// </summary>
        /// <param name="ids">Arreglo de identificadores string</param>
        /// <returns></returns>
        [HttpPatch("restaurar/{ids}", Name = "restaurarValoracionEntradaClasificacion")]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult> Undelete(string ids)
        {
            string[] lids = ids.Split(',').ToList()
            .Where(x => !string.IsNullOrEmpty(x)).ToArray();
            return Ok(await ServicioValoracion.Restaurar(lids).ConfigureAwait(false));
        }

    }
}

