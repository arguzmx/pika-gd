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

        /// <summary>
        /// Obtiene los metadatos relacionados con la 
        /// entidad Activos Declinados
        /// </summary>
        /// <returns></returns>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpGet("metadata", Name = "MetadataActivoDeclinado")]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        public async Task<ActionResult<MetadataInfo>> GetMetadata([FromQuery]Consulta query = null)
        {
            return Ok(await metadataProvider.Obtener().ConfigureAwait(false));
        }

        /// <summary>
        /// Añade una nueva entidad 
        /// del Transferencia
        /// </summary>
        /// <param name="entidad"></param>
        /// <returns></returns>

        [HttpPost()]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        [ProducesResponseType(StatusCodes.Status200OK)]

        public async Task<ActionResult<ActivoDeclinado>> Post(string TransferenciaId, [FromBody]ActivoDeclinado entidad)
        {
            //if (TransferenciaId != entidad.TransferenciaId)
            //{
            //    return BadRequest();
            //}
            Console.WriteLine(TransferenciaId);
            entidad = await servicioActivoDeclinado.CrearAsync(entidad).ConfigureAwait(false);
            return Ok(CreatedAtAction("GetActivoDeclinado", new { ActivoId = entidad.ActivoId }, entidad).Value);
        }


        [HttpPut("{ActivoId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]

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

       /// <summary>
        /// Devulve un alista de Estado Transferencia asociadas al objeto del tipo especificado
        /// </summary>
        /// <param name="query">Consulta para la paginación y búsqueda</param>
        /// <returns></returns>
        [HttpGet("page", Name = "GetPageActivoDeclinado")]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        [ProducesResponseType(StatusCodes.Status200OK)]

        public async Task<ActionResult<IEnumerable<ActivoDeclinado>>> GetPage([FromQuery]Consulta query = null)
        {
            ///Añade las propiedaes del contexto para el filtro de ACL vía ACL Controller
            query.Filtros.AddRange(ObtieneFiltrosIdentidad());
            var data = await servicioActivoDeclinado.ObtenerPaginadoAsync(query).ConfigureAwait(false);
            return Ok(data.Elementos.ToList<ActivoDeclinado>());
        }
 
        /// <summary>
        /// Obtiene un Estado Transferencia en base al Id único
        /// </summary>
        /// <param name="id">Id único del Estado Transferencia</param>
        /// <returns></returns>

        [HttpGet("{ActivoId}")]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        [ProducesResponseType(StatusCodes.Status200OK)]

        public async Task<ActionResult<ActivoDeclinado>> Get(string ActivoId)
        {
            var o = await servicioActivoDeclinado.UnicoAsync(x => x.ActivoId == ActivoId).ConfigureAwait(false);
            if (o != null) return Ok(o);
            return NotFound(ActivoId);
        }


        /// <summary>
        /// Elimina de manera permanente un estado Transferencia en base al arreglo de identificadores recibidos
        /// </summary>
        /// <param name="ids">Arreglo de identificadores string</param>
        /// <returns></returns>
        [HttpDelete]
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
            return Ok(await servicioActivoDeclinado.Eliminar(lids).ConfigureAwait(false));
        }
    }
}