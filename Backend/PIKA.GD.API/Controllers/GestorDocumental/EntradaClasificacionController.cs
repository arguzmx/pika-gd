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
    public class EntradaClasificacionController : ACLController
    {
        private readonly ILogger<EntradaClasificacionController> logger;
        private IServicioEntradaClasificacion servicioCuadro;
        private IProveedorMetadatos<EntradaClasificacion> metadataProvider;
        public EntradaClasificacionController(ILogger<EntradaClasificacionController> logger,
            IProveedorMetadatos<EntradaClasificacion> metadataProvider,
            IServicioEntradaClasificacion servicioCuadro)
        {
            this.logger = logger;
            this.servicioCuadro = servicioCuadro;
            this.metadataProvider = metadataProvider;
        }
        /// <summary>
        /// Obtiene los metadatos relacionados con la entidad Entrada Clasificacion
        /// </summary>
        /// <returns></returns>
        [HttpGet("metadata", Name = "MetadataEntradaClasificacion")]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        public async Task<ActionResult<MetadataInfo>> GetMetadata([FromQuery] Consulta query = null)
        {
            return Ok(await metadataProvider.Obtener().ConfigureAwait(false));
        }


        /// <summary>
        /// Añade una nueva entidad del Entrada Clasificacion
        /// </summary>
        /// <param name="entidad"></param>
        /// <returns></returns>
        [HttpPost]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        public async Task<ActionResult<EntradaClasificacion>> Post([FromBody] EntradaClasificacion entidad)
        {
            entidad = await servicioCuadro.CrearAsync(entidad).ConfigureAwait(false);
            return Ok(CreatedAtAction("GetEntradaClasificacion", new { id = entidad.Id }, entidad).Value);
        }
        /// <summary>
        /// Actualiza unq entidad Entrada Clasificacion, el Id debe incluirse en el Querystring así como en 
        /// el serializado para la petición PUT
        /// </summary>
        /// <param name="id">Identificador único del dominio</param>
        /// <param name="entidad">Datos serialziados de la OU</param>
        /// <returns></returns>

        [HttpPut("{id}")]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        public async Task<IActionResult> Put(string id, [FromBody] EntradaClasificacion entidad)
        {
            var x = ObtieneFiltrosIdentidad();


            if (id != entidad.Id)
            {
                return BadRequest();
            }

            await servicioCuadro.ActualizarAsync(entidad).ConfigureAwait(false);
            return NoContent();

        }


        /// <summary>
        /// Devulve un alista de paises asociadas al objeto del tipo especificado
        /// </summary>
        /// <param name="query">Consulta para la paginación y búsqueda</param>
        /// <returns></returns>
        [HttpGet("page", Name = "GetPageEntradaClasificacion")]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<Paginado<EntradaClasificacion>>> GetPage(
            [ModelBinder(typeof(GenericDataPageModelBinder))][FromQuery] Consulta query = null)
        {
            var data = await servicioCuadro.ObtenerPaginadoAsync(
                Query: query,
                include: null)
                .ConfigureAwait(false);

            return Ok(data);
        }

        /// <summary>
        /// Obtiene un Entrada Clasificacion en base al Id único
        /// </summary>
        /// <param name="id">Id único del país</param>
        /// <returns></returns>

        [HttpGet("{id}")]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<EntradaClasificacion>> Get(string id)
        {
            var o = await servicioCuadro.UnicoAsync(x => x.Id == id).ConfigureAwait(false);
            if (o != null) return Ok(o);
            return NotFound(id);
        }

        /// <summary>
        /// Elimina de manera permanente un Entrada Clasificacion en base al arreglo de identificadores recibidos
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
            return Ok(await servicioCuadro.Eliminar(lids).ConfigureAwait(false));
        }


        /// <summary>
        /// Restaura una lista dede dominiios eliminados en base al arreglo de identificadores recibidos
        /// </summary>
        /// <param name="ids">Arreglo de identificadores string</param>
        /// <returns></returns>
        [HttpPatch("restaurar/{ids}", Name = "restaurarEntradaClasifiacion")]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult> Undelete(string ids)
        {
            string[] lids = ids.Split(',').ToList()
            .Where(x => !string.IsNullOrEmpty(x)).ToArray();
            return Ok(await servicioCuadro.Restaurar(lids).ConfigureAwait(false));
        }

    }
}
