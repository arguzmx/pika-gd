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
        /// Añade una nueva entidad 
        /// del Transferencia
        /// </summary>
        /// <param name="entidad"></param>
        /// <returns></returns>

        [HttpPost("{ActivoId}")]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<ActivoDeclinado>> Post(string TransferenciaId, string ActivoId,[FromBody]ActivoDeclinado entidad)
        {
            if (TransferenciaId.Trim() != entidad.TransferenciaId.Trim() && ActivoId.Trim() != entidad.ActivoId.Trim())
            {
                return BadRequest();
            }
            entidad = await servicioActivoDeclinado.CrearAsync(entidad).ConfigureAwait(false);
            return Ok(CreatedAtAction("GetActivoDeclinado", new { ActivoId = entidad.ActivoId }, entidad).Value);
        }


        [HttpPut("{ActivoId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]

        public async Task<IActionResult> Put(string TransferenciaId, string ActivoId, [FromBody]ActivoDeclinado entidad)
        {
            var x = ObtieneFiltrosIdentidad();


            if (TransferenciaId.Trim() != entidad.TransferenciaId.Trim() && ActivoId.Trim() != entidad.ActivoId.Trim())
            {
                return BadRequest();
            }

            await servicioActivoDeclinado.ActualizarAsync(entidad).ConfigureAwait(false);
            return NoContent();

        }

        /// <summary>
        /// Elimina de manera permanente un Activo Declinado en base al arreglo de identificadores recibidos
        /// </summary>
        /// <param name="ids">Arreglo de identificadores string</param>
        /// <returns></returns>
        [HttpDelete("{Ids}",Name ="DeleteActivoDeclinado")]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult> Delete(string TransferenciaId, string ids)
        {
            string IdsTrim = "";
            TransferenciaId = TransferenciaId.Trim();
            foreach (string item in ids.Split(',').ToList().Where(x => !string.IsNullOrEmpty(x)).ToArray())
            {
                IdsTrim += item.Trim() + ",";
            }
            string[] lids = IdsTrim.Split(',').ToList()
           .Where(x => !string.IsNullOrEmpty(x)).ToArray();
            return Ok(await servicioActivoDeclinado.EliminarActivoDeclinado(TransferenciaId,lids).ConfigureAwait(false));
        }
    }
}