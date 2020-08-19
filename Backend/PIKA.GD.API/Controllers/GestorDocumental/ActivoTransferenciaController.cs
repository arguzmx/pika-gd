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
        [ProducesResponseType(StatusCodes.Status200OK)]

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

        [HttpPost("TransferenciaId")]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        [ProducesResponseType(StatusCodes.Status200OK)]

        public async Task<ActionResult<ActivoTransferencia>> Post(string TransferenciaId, [FromBody]ActivoTransferencia entidad)
        {
            if (TransferenciaId.Trim() != entidad.TransferenciaId.Trim())
            {
                return BadRequest();
            }
          
            Console.WriteLine(TransferenciaId.Trim());

            entidad = await servicioActivoTransferencia.CrearAsync(entidad).ConfigureAwait(false);
            return Ok(CreatedAtAction("GetActivoTransferencia", new { ActivoId = entidad.ActivoId }, entidad).Value);
        }


        /// <summary>
        /// Actualiza una entidad Estados Transferencias, el Id debe incluirse en el Querystring así como en 
        /// el serializado para la petición PUT
        /// </summary>
        /// <param name="id">Identificador único del dominio</param>
        /// <param name="entidad">Datos serialziados de la OU</param>
        /// <returns></returns>
        [HttpPut("{activoid}")]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> Put(string activoid, [FromBody]ActivoTransferencia entidad)
        {
            var x = ObtieneFiltrosIdentidad();


            if (activoid != entidad.ActivoId)
            {
                return BadRequest();
            }

            await servicioActivoTransferencia.ActualizarAsync(entidad).ConfigureAwait(false);
            return NoContent();

        }
        /// <summary>
        /// Devulve un alista de Estado Transferencia asociadas al objeto del tipo especificado
        /// </summary>
        /// <param name="query">Consulta para la paginación y búsqueda</param>
        /// <returns></returns>

        [HttpGet("page", Name = "GetPageActivoTransferencia")]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        [ProducesResponseType(StatusCodes.Status200OK)]

        public async Task<ActionResult<IEnumerable<ActivoTransferencia>>> GetPage([FromQuery]Consulta query = null)
        {
            ///Añade las propiedaes del contexto para el filtro de ACL vía ACL Controller
            query.Filtros.AddRange(ObtieneFiltrosIdentidad());
            var data = await servicioActivoTransferencia.ObtenerPaginadoAsync(query).ConfigureAwait(false);
            return Ok(data.Elementos.ToList<ActivoTransferencia>());
        }
        /// <summary>
        /// Obtiene un Estado Transferencia en base al Id único
        /// </summary>
        /// <param name="id">Id único del Estado Transferencia</param>
        /// <returns></returns>

        [HttpGet("{ActivoId}")]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        [ProducesResponseType(StatusCodes.Status200OK)]

        public async Task<ActionResult<ActivoTransferencia>> Get(string ActivoId)
        {
            var o = await servicioActivoTransferencia.UnicoAsync(x => x.ActivoId == ActivoId).ConfigureAwait(false);
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
            return Ok(await servicioActivoTransferencia.Eliminar(lids).ConfigureAwait(false));
        }
    }
}