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
    public class ComentarioTransferenciaController : ACLController
    {
        private readonly ILogger<ComentarioTransferenciaController> logger;
        private IServicioComentarioTransferencia servicioComentarioTransferencia;
        private IProveedorMetadatos<ComentarioTransferencia> metadataProvider;
        public ComentarioTransferenciaController(ILogger<ComentarioTransferenciaController> logger,
            IProveedorMetadatos<ComentarioTransferencia> metadataProvider,
            IServicioComentarioTransferencia servicioComentarioTransferencia)
        {
            this.logger = logger;
            this.servicioComentarioTransferencia = servicioComentarioTransferencia;
            this.metadataProvider = metadataProvider;
        }
        /// <summary>
        /// Obtiene los metadatos relacionados con la entidad Comentario Transferencia
        /// </summary>
        /// <returns></returns>
        [HttpGet("metadata", Name = "MetadataComentarioTransferencia")]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        [ProducesResponseType(StatusCodes.Status200OK)]

        public async Task<ActionResult<MetadataInfo>> GetMetadata([FromQuery]Consulta query = null)
        {
            return Ok(await metadataProvider.Obtener().ConfigureAwait(false));
        }

        /// <summary>
        /// Añade una nueva entidad del tipo Comentario Transferencia
        /// </summary>
        /// <param name="entidad"></param>
        /// <returns></returns>

        [HttpPost]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        [ProducesResponseType(StatusCodes.Status200OK)]

        public async Task<ActionResult<ComentarioTransferencia>> Post(string TransferenciaId,  [FromBody]ComentarioTransferencia entidad)
        {
            if (TransferenciaId.Trim() != entidad.TransferenciaId.Trim() )
            {
                return BadRequest();
            }
            entidad = await servicioComentarioTransferencia.CrearAsync(entidad).ConfigureAwait(false);
            return Ok(CreatedAtAction("GetComentarioTransferencia", new { id = entidad.Id }, entidad).Value);
        }

        /// <summary>
        /// Actualiza unq entidad Comentario Transferencia, el Id debe incluirse en el Querystring así como en 
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
        public async Task<IActionResult> Put(string id, [FromBody]ComentarioTransferencia entidad)
        {
            var x = ObtieneFiltrosIdentidad();


            if (id != entidad.Id)
            {
                return BadRequest();
            }

            await servicioComentarioTransferencia.ActualizarAsync(entidad).ConfigureAwait(false);
            return NoContent();

        }
        /// <summary>
        /// Devulve un alista de Comentario Clasificacion asociadas al objeto del tipo especificado
        /// </summary>
        /// <param name="query">Consulta para la paginación y búsqueda</param>
        /// <returns></returns>

        [HttpGet("page", Name = "GetPageComentarioTransferencia")]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<ComentarioTransferencia>>> GetPage([FromQuery]Consulta query = null)
        {
            ///Añade las propiedaes del contexto para el filtro de ACL vía ACL Controller
            query.Filtros.AddRange(ObtieneFiltrosIdentidad());
            var data = await servicioComentarioTransferencia.ObtenerPaginadoAsync(query).ConfigureAwait(false);
            return Ok(data.Elementos.ToList<ComentarioTransferencia>());
        }
        /// <summary>
        /// Obtiene un Comentario Clasificación en base al Id único
        /// </summary>
        /// <param name="id">Id único del país</param>
        /// <returns></returns>

        [HttpGet("{Id}")]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<ComentarioTransferencia>> Get(string id)
        {
            var o = await servicioComentarioTransferencia.UnicoAsync(x => x.Id == id).ConfigureAwait(false);
            if (o != null) return Ok(o);
            return NotFound(id);
        }



        /// <summary>
        /// Elimina de manera permanente un Comentarios Transferencia en base al arreglo de identificadores recibidos
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
            return Ok(await servicioComentarioTransferencia.Eliminar(lids).ConfigureAwait(false));
        }
    }
}