﻿using System;
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
    public class HistorialArchivoActivoController : ACLController
    {
        private readonly ILogger<HistorialArchivoActivoController> logger;
        private IServicioHistorialArchivoActivo servicioHistorialArchivoActivo;
        private IProveedorMetadatos<HistorialArchivoActivo> metadataProvider;
        public HistorialArchivoActivoController(ILogger<HistorialArchivoActivoController> logger,
            IProveedorMetadatos<HistorialArchivoActivo> metadataProvider,
            IServicioHistorialArchivoActivo servicioHistorialArchivoActivo)
        {
            this.logger = logger;
            this.servicioHistorialArchivoActivo = servicioHistorialArchivoActivo;
            this.metadataProvider = metadataProvider;
        }
        /// <summary>
        /// Obtiene los metadatos relacionados con la entidad Historial Archivo Activo
        /// </summary>
        /// <returns></returns>
        [HttpGet("metadata", Name = "MetadataHistorialArchivoActivo")]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        public async Task<ActionResult<MetadataInfo>> GetMetadata([FromQuery] Consulta query = null)
        {
            return Ok(await metadataProvider.Obtener().ConfigureAwait(false));
        }


        /// <summary>
        /// Añade una nueva entidad del Historial Archivo Activo
        /// </summary>
        /// <param name="entidad"></param>
        /// <returns></returns>
        [HttpPost]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        public async Task<ActionResult<HistorialArchivoActivo>> Post([FromBody] HistorialArchivoActivo entidad)
        {
            entidad = await servicioHistorialArchivoActivo.CrearAsync(entidad).ConfigureAwait(false);
            return Ok(CreatedAtAction("GetHistorialArchivoActivo", new { id = entidad.Id }, entidad).Value);
        }
        /// <summary>
        /// Actualiza unq entidad Historial Archivo Activo, el Id debe incluirse en el Querystring así como en 
        /// el serializado para la petición PUT
        /// </summary>
        /// <param name="id">Identificador único del dominio</param>
        /// <param name="entidad">Datos serialziados de la OU</param>
        /// <returns></returns>

        [HttpPut("{id}")]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> Put(string id, [FromBody] HistorialArchivoActivo entidad)
        {
            var x = ObtieneFiltrosIdentidad();


            if (id.Trim() != entidad.Id.Trim())
            {
                return BadRequest();
            }

            await servicioHistorialArchivoActivo.ActualizarAsync(entidad).ConfigureAwait(false);
            return NoContent();

        }


        /// <summary>
        /// Devulve un alista de paises asociadas al objeto del tipo especificado
        /// </summary>
        /// <param name="query">Consulta para la paginación y búsqueda</param>
        /// <returns></returns>
        [HttpGet("page", Name = "GetPageHistorialArchivoActivo")]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<Paginado<HistorialArchivoActivo>>> GetPage(
            [ModelBinder(typeof(GenericDataPageModelBinder))][FromQuery] Consulta query = null)
        {
            var data = await servicioHistorialArchivoActivo.ObtenerPaginadoAsync(
                Query: query,
                include: null)
                .ConfigureAwait(false);

            return Ok(data);
        }

        /// <summary>
        /// Obtiene un Historial Archivo Activo en base al Id único
        /// </summary>
        /// <param name="id">Id único del país</param>
        /// <returns></returns>

        [HttpGet("{id}")]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<HistorialArchivoActivo>> Get(string id)
        {
            var o = await servicioHistorialArchivoActivo.UnicoAsync(x => x.Id == id.Trim()).ConfigureAwait(false);
            if (o != null) return Ok(o);
            return NotFound(id);
        }

        /// <summary>
        /// Elimina de manera permanente un Historial Archivo Activo en base al arreglo de identificadores recibidos
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
            return Ok(await servicioHistorialArchivoActivo.Eliminar(lids).ConfigureAwait(false));
        }


        

    }
}
