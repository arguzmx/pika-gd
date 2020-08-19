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

        

        /// <summary>
        /// Añade una nueva entidad 
        /// del Transferencia
        /// </summary>
        /// <param name="entidad"></param>
        /// <returns></returns>

        [HttpPost("{ActivoId}", Name ="PostActivoTransferencia")]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        [ProducesResponseType(StatusCodes.Status200OK)]

        public async Task<ActionResult<ActivoTransferencia>> Post(string TransferenciaId,string ActivoId, [FromBody]ActivoTransferencia entidad)
        {
            if (TransferenciaId.Trim() != entidad.TransferenciaId.Trim() && ActivoId.Trim() != entidad.ActivoId.Trim())
            {
                return BadRequest();
            }


            entidad = await servicioActivoTransferencia.CrearAsync(entidad).ConfigureAwait(false);
            return Ok(CreatedAtAction("GetActivoTransferencia", new { ActivoId = entidad.ActivoId }, entidad).Value);
        }


        
        /// <summary>
        /// Elimina de manera permanente un Activo Transferencia en base al arreglo de identificadores recibidos
        /// </summary>
        /// <param name="ids">Arreglo de identificadores string</param>
        /// <returns></returns>
        [HttpDelete("{Ids}", Name = "DeleteActivoTransferencia")]
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
            return Ok(await servicioActivoTransferencia.EliminarActivoTransferencia(TransferenciaId, lids).ConfigureAwait(false));
        }
    }
}