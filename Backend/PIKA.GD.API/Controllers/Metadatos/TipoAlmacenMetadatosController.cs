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
using PIKA.Modelo.Metadatos;
using PIKA.Servicio.Metadatos.Interfaces;
using RepositorioEntidades;


namespace PIKA.GD.API.Controllers.Metadatos
{
    [Authorize]
    [ApiVersion("1.0")]
    [ApiController]
    [Route("api/v{version:apiVersion}/metadatos/[controller]")]
    public class TipoAlmacenMetadatosController : ACLController
    {
        private readonly ILogger<TipoAlmacenMetadatosController> logger;
        private IServicioTipoAlmacenMetadatos servicioTipoAlmacenMetadatos;
        private IProveedorMetadatos<TipoAlmacenMetadatos> metadataProvider;
        public TipoAlmacenMetadatosController(ILogger<TipoAlmacenMetadatosController> logger,
            IProveedorMetadatos<TipoAlmacenMetadatos> metadataProvider,
            IServicioTipoAlmacenMetadatos servicioTipoAlmacenMetadatos)
        {
            this.logger = logger;
            this.servicioTipoAlmacenMetadatos = servicioTipoAlmacenMetadatos;
            this.metadataProvider = metadataProvider;
        }


        /// <summary>
        /// Obtiene los metadatos relacionados con la entidad Tipo Almacen Metadatos
        /// </summary>
        /// <returns></returns>
        [HttpGet("metadata", Name = "MetadataTipoAlmacenMetadatos")]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        [ProducesResponseType(StatusCodes.Status200OK)]

        public async Task<ActionResult<MetadataInfo>> GetMetadata([FromQuery] Consulta query = null)
        {
            Console.WriteLine("\n Ingreso.... \n");
            return Ok(await metadataProvider.Obtener().ConfigureAwait(false));
        }


        /// <summary>
        /// Añade una nueva entidad del tipo Tipo Almacen Metadatos
        /// </summary>
        /// <param name="entidad"></param>
        /// <returns></returns>

        [HttpPost]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        [ProducesResponseType(StatusCodes.Status200OK)]

        public async Task<ActionResult<TipoAlmacenMetadatos>> Post([FromBody] TipoAlmacenMetadatos entidad)
        {
            entidad = await servicioTipoAlmacenMetadatos.CrearAsync(entidad).ConfigureAwait(false);
            return Ok(CreatedAtAction("GetTipoAlmacenMetadatos", new { id = entidad.Id }, entidad).Value);
        }


        /// <summary>
        /// Actualiza una entidad Tipo Almacen Metadatos, el Id debe incluirse en el Querystring así como en 
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
        public async Task<IActionResult> Put(string id, [FromBody] TipoAlmacenMetadatos entidad)
        {
            var x = ObtieneFiltrosIdentidad();


            if (id.Trim() != entidad.Id.Trim())
            {
                return BadRequest();
            }

            await servicioTipoAlmacenMetadatos.ActualizarAsync(entidad).ConfigureAwait(false);
            return NoContent();

        }

        /// <summary>
        /// Devulve un alista de Tipo Almacen Metadatos asociadas al objeto del tipo especificado
        /// </summary>
        /// <param name="query">Consulta para la paginación y búsqueda</param>
        /// <returns></returns>

        [HttpGet("page", Name = "GetPageTipoAlmacenMetadatos")]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        public async Task<ActionResult<IEnumerable<TipoAlmacenMetadatos>>> GetPage([ModelBinder(typeof(GenericDataPageModelBinder))][FromQuery] Consulta query = null)
        {
            var data = await servicioTipoAlmacenMetadatos.ObtenerPaginadoAsync(
                Query: query,
                include: null)
                .ConfigureAwait(false);

            return Ok(data);
        }


        //----------------------------------------------------------------------------------------------------------
        //----------------------------------------------------------------------------------------------------------
        //----------------------------------------------------------------------------------------------------------
        //----------------------------------------------------------------------------------------------------------

        /// <summary>
        /// Obtiene un Tipo Almacen Metadatos en base al Id único
        /// </summary>
        /// <param name="id">Id único del Tipo Almacen Metadatos</param>
        /// <returns></returns>

        [HttpGet("{id}")]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<TipoAlmacenMetadatos>> Get(string id)
        {
            var o = await servicioTipoAlmacenMetadatos.UnicoAsync(x => x.Id == id.Trim()).ConfigureAwait(false);
            if (o != null) return Ok(o);
            return NotFound(id);
        }



        /// <summary>
        /// Elimina de manera permanente un Tipo Almacen Metadatos en base al arreglo de identificadores recibidos
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
            return Ok(await servicioTipoAlmacenMetadatos.Eliminar(lids).ConfigureAwait(false));
        }


        /// <summary>
        /// Restaura una lista dede Tipo Almacen Metadatoss eliminados en base al arreglo de identificadores recibidos
        /// </summary>
        /// <param name="ids">Arreglo de identificadores string</param>
        /// <returns></returns>
        [HttpPatch("restaurar/{ids}", Name = "restaurarTipoAlmacenMetadatos")]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult> Undelete(string ids)
        {

            string IdsTrim = "";
            foreach (string item in ids.Split(',').ToList().Where(x => !string.IsNullOrEmpty(x)).ToArray())
            {
                IdsTrim += item.Trim() + ",";
            }
            string[] lids = IdsTrim.Split(',').ToList()
           .Where(x => !string.IsNullOrEmpty(x)).ToArray();

            return Ok(await servicioTipoAlmacenMetadatos.Restaurar(lids).ConfigureAwait(false));
        }

    }
}