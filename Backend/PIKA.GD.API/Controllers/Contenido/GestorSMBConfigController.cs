﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PIKA.GD.API.Filters;
using PIKA.GD.API.Model;
using PIKA.Modelo.Contenido;
using PIKA.Modelo.Metadatos;
using PIKA.Servicio.Contenido.Interfaces;
using RepositorioEntidades;

namespace PIKA.GD.API.Controllers.Contenido
{
    [Authorize]
    [ApiVersion("1.0")]
    [ApiController]
    [Route("api/v{version:apiVersion}/contenido/[controller]")]
    public class GestorSMBConfigController : ACLController
    {

        private ILogger<GestorSMBConfigController> logger;
        private IServicioGestorSMBConfig servicioEntidad;
        private IProveedorMetadatos<GestorSMBConfig> metadataProvider;
        public GestorSMBConfigController(ILogger<GestorSMBConfigController> logger,
            IProveedorMetadatos<GestorSMBConfig> metadataProvider,
            IServicioGestorSMBConfig servicioEntidad)
        {
            this.logger = logger;
            this.servicioEntidad = servicioEntidad;
            this.metadataProvider = metadataProvider;
        }


        /// <summary>
        /// Otiene los metadatos asociados al la configuración de gestores SMB de almacenamiento
        /// </summary>
        /// <returns></returns>
        [HttpGet("metadata", Name = "MetadataGestorSMBConfigContenido")]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<MetadataInfo>> GetMetadata()
        {
            return Ok(await metadataProvider.Obtener().ConfigureAwait(false));
        }


        /// <summary>
        ///  Añade una nueva entidad gestor SMB de almacenamiento
        /// </summary>
        /// <param name="entidad"></param>
        /// <returns></returns>
        [HttpPost]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<GestorSMBConfig>> Post([FromBody]GestorSMBConfig entidad)
        {
            entidad = await servicioEntidad.CrearAsync(entidad).ConfigureAwait(false);
            return Ok(CreatedAtAction("GetGestorSMBConfig", new { id = entidad.VolumenId }, entidad).Value);
        }

        /// <summary>
        /// Actualiza una entidad  gestor SMB de almacenamiento, el Id debe incluirse en el Querystring así como en 
        /// el serializado para la petición PUT
        /// </summary>
        /// <param name="id">Identificador único del dominio</param>
        /// <param name="entidad">Entidad serializada JSON</param>
        /// <returns></returns>
        [HttpPut("{id}")]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> Put(string id, [FromBody] GestorSMBConfig entidad)
        {

            if (id != entidad.VolumenId)
            {
                return BadRequest();
            }

            await servicioEntidad.ActualizarAsync(entidad).ConfigureAwait(false);
            return NoContent();

        }

        /// <summary>
        /// Otiene una página de resultados de gestores SMB de almacenamiento en base a la configuración de paginado u al query recibido 
        /// </summary>
        /// <param name="query">Query de filtrado para el paginado</param>
        /// <returns></returns>
        [HttpGet("page", Name = "GetPageGestorSMBConfig")]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<GestorSMBConfig>>> GetPage(
            [ModelBinder(typeof(GenericDataPageModelBinder))][FromQuery]Consulta query = null)
        {
    
            var data = await servicioEntidad.ObtenerPaginadoAsync(query).ConfigureAwait(false);
            return Ok(data);
        }


        /// <summary>
        /// Obtiene un gestor SMB de almacenamiento en base los identificadores clave
        /// </summary>
        /// <param name="volumenid">Identificador del volumnen asociado</param>
        /// <returns></returns>
        [HttpGet("{volumenid}")]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<GestorSMBConfig>> Get(string volumenid)
        {
            var o = await servicioEntidad.UnicoAsync(x => x.VolumenId == volumenid)
                .ConfigureAwait(false);

            if (o != null) return Ok(o);
            return NotFound(volumenid);
        }

        /// <summary>
        /// Elimina de manera permanente un gestor SMB de almacenamiento en base al arreglo de identificadores recibidos
        /// </summary>
        /// <param name="ids">Lista de identificadores de volumen separados por comas</param>
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

    }
}