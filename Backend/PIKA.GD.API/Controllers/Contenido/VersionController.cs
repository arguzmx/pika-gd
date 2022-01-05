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
using PIKA.Servicio.Contenido.ElasticSearch;
using PIKA.Servicio.Contenido.Interfaces;
using RepositorioEntidades;
using Version = PIKA.Modelo.Contenido.Version;

namespace PIKA.GD.API.Controllers.Contenido
{
    [Authorize]
    [ApiVersion("1.0")]
    [ApiController]
    [Route("api/v{version:apiVersion}/contenido/[controller]")]
    public class VersionController : ACLController
    {

        private ILogger<VersionController> logger;
        private IServicioElemento servicioElemento;
        private IProveedorMetadatos<Version> metadataProvider;
        private IRepoContenidoElasticSearch repoContenido;
        private readonly IServicioVolumen servicioVol;
        public VersionController(ILogger<VersionController> logger,
            IProveedorMetadatos<Version> metadataProvider,
            IServicioVersion servicioEntidad,
            IRepoContenidoElasticSearch repoContenido,
            IServicioVolumen servicioVol,
            IServicioElemento servicioElemento)
        {
            this.logger = logger;
            this.metadataProvider = metadataProvider;
            this.repoContenido = repoContenido;
            this.servicioVol = servicioVol;
            this.servicioElemento = servicioElemento;
        }


        /// <summary>
        /// Otiene los metadatos asociados a la Version
        /// </summary>
        /// <returns></returns>
        [HttpGet("metadata", Name = "MetadataVersionContenido")]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<MetadataInfo>> GetMetadata()
        {
            return Ok(await metadataProvider.Obtener().ConfigureAwait(false));
        }


        /// <summary>
        ///  Añade una nueva Version de contenido
        /// </summary>
        /// <param name="entidad"></param>
        /// <returns></returns>
        [HttpPost]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<Version>> Post([FromBody]Modelo.Contenido.Version entidad)
        {
            await repoContenido.CreaRepositorio().ConfigureAwait(false);
            entidad.CreadorId = this.UsuarioId;
            
            var id = await this.repoContenido.CreaVersion(entidad).ConfigureAwait(false);
            await servicioElemento.ActualizaVersion(entidad.ElementoId, id).ConfigureAwait(false);
            
            // entidad = await servicioEntidad.CrearAsync(entidad).ConfigureAwait(false);

            return Ok(CreatedAtAction("GetVersion", new { id }, entidad).Value);
        }

        /// <summary>
        /// Actualiza unq entidad Version de contenido existente, el Id debe incluirse en el Querystring así como en 
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
        public async Task<IActionResult> Put(string id, [FromBody]Version entidad)
        {
            if (id != entidad.Id)
            {
                return BadRequest();
            }
            await this.repoContenido.ActualizaVersion(id, entidad, true).ConfigureAwait(false);

            // await servicioEntidad.ActualizarAsync(entidad).ConfigureAwait(false);
            return NoContent();

        }

        ///// <summary>
        ///// Otiene una página de resultados de Versiones de contenide en base a la configuración de paginado u al query recibido 
        ///// </summary>
        ///// <param name="query">Query de filtrado para el paginado</param>
        ///// <returns></returns>
        //[HttpGet("page", Name = "GetPageVersion")]
        //[TypeFilter(typeof(AsyncACLActionFilter))]
        //[ProducesResponseType(StatusCodes.Status200OK)]
        //public async Task<ActionResult<Paginado<Version>>> GetPage([ModelBinder(typeof(GenericDataPageModelBinder))][FromQuery]Consulta query = null)
        //{
        //    ///Añade las propiedaes del contexto para el filtro de ACL vía ACL Controller
        //    query.Filtros.AddRange(ObtieneFiltrosIdentidad());
        //    var data = await servicioEntidad.ObtenerPaginadoAsync(query).ConfigureAwait(false);
        //    return Ok(data);
        //}


        /// <summary>
        /// Obtiene una Version en base al Id único
        /// </summary>
        /// <param name="id">Id único del país</param>
        /// <returns></returns>
        [HttpGet("{id}")]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<Version>> Get(string id)
        {
            var o = await this.repoContenido.ObtieneVersion(id).ConfigureAwait(false);
            // var o = await servicioEntidad.UnicoAsync(x => x.Id == id).ConfigureAwait(false);
            if (o != null) return Ok(o);
            return NotFound(id);
        }


        /// <summary>
        /// Elimina de manera lógica una Version de contenido en base al arreglo de identificadores recibidos
        /// </summary>
        /// <param name="ids">Lista de identificadores separados por omas</param>
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

            foreach(string id  in lids)
            {
                await this.repoContenido.EliminaVersion(id).ConfigureAwait(false);
            };

            return Ok(lids);
        }

        ///// <summary>
        ///// Restaura una lista de Versioness eliminados en base al arreglo de identificadores recibidos
        ///// </summary>
        ///// <param name="ids">Arreglo de identificadores string</param>
        ///// <returns></returns>
        //[HttpPatch("restaurar/{ids}", Name = "RestaurarVersion")]
        //[TypeFilter(typeof(AsyncACLActionFilter))]
        //[ProducesResponseType(StatusCodes.Status200OK)]
        //public async Task<ActionResult> Undelete(string ids)
        //{
        //    string IdsTrim = "";
        //    foreach (string item in ids.Split(',').ToList().Where(x => !string.IsNullOrEmpty(x)).ToArray())
        //    {
        //        IdsTrim += item.Trim() + ",";
        //    }
        //    string[] lids = IdsTrim.Split(',').ToList()
        //   .Where(x => !string.IsNullOrEmpty(x)).ToArray();
        //    return Ok(await servicioEntidad.Restaurar(lids).ConfigureAwait(false));
        //}

    }
}