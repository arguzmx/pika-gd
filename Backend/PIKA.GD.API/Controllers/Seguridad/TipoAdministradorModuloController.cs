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
using PIKA.Infraestructura.Comun;
using PIKA.Modelo.Metadatos;
using PIKA.Servicio.Seguridad.Interfaces;
using RepositorioEntidades;


namespace PIKA.GD.API.Controllers.Seguridad
{
  
    [Authorize]
    [ApiVersion("1.0")]
    [ApiController]
    [Route("api/v{version:apiVersion}/seguridad/[controller]")]
    public class TipoAdministradorModuloController : ACLController
    {
        private ILogger<TipoAdministradorModuloController> logger;
        private IServicioTipoAdministradorModulo servicioTipoAdministradorModulo;
        private IProveedorMetadatos<TipoAdministradorModulo> metadataProvider;
        public TipoAdministradorModuloController(ILogger<TipoAdministradorModuloController> logger,
            IProveedorMetadatos<TipoAdministradorModulo> metadataProvider,
            IServicioTipoAdministradorModulo servicioTipoAdministradorModulo)
        {
            this.logger = logger;
            this.servicioTipoAdministradorModulo = servicioTipoAdministradorModulo;
            this.metadataProvider = metadataProvider;
        }

        [HttpGet("metadata", Name = "MetadataTipoAdministradorModulo")]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        [ProducesResponseType(StatusCodes.Status200OK)]

        public async Task<ActionResult<MetadataInfo>> GetMetadata([FromQuery]Consulta query = null)
        {
            return Ok(await metadataProvider.Obtener().ConfigureAwait(false));
        }



        [HttpPost]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        [ProducesResponseType(StatusCodes.Status200OK)]

        public async Task<ActionResult<TipoAdministradorModulo>> Post([FromBody]TipoAdministradorModulo entidad)
        {

            entidad = await servicioTipoAdministradorModulo.CrearAsync(entidad).ConfigureAwait(false);
            return Ok(CreatedAtAction("GetTipoAdministradorModulo", new { id = entidad.Id }, entidad).Value);
        }


        [HttpPut("{id}")]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> Put(string id, [FromBody]TipoAdministradorModulo entidad)
        {
            var x = ObtieneFiltrosIdentidad();

            if (id != entidad.Id)
            {
                return BadRequest();
            }

            await servicioTipoAdministradorModulo.ActualizarAsync(entidad).ConfigureAwait(false);
            return NoContent();

        }


        [HttpGet("page", Name = "GetPageTipoAdministradorModulo")]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        [ProducesResponseType(StatusCodes.Status200OK)]

        public async Task<ActionResult<Paginado<TipoAdministradorModulo>>> GetPage([ModelBinder(typeof(GenericDataPageModelBinder))][FromQuery]Consulta query = null)
        {

            ///Añade las propiedaes del contexto para el filtro de ACL vía ACL Controller
            query.Filtros.AddRange(ObtieneFiltrosIdentidad()); 
            var data = await servicioTipoAdministradorModulo.ObtenerPaginadoAsync(
           Query: query,
           include: null)
           .ConfigureAwait(false);

            return Ok(data);
        }


        [HttpGet("{id}")]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        [ProducesResponseType(StatusCodes.Status200OK)]

        public async Task<ActionResult<TipoAdministradorModulo>> Get(string id)
        {
            var o = await servicioTipoAdministradorModulo.UnicoAsync(x => x.Id == id).ConfigureAwait(false);
            if (o != null) return Ok(o);
            return NotFound(id);
        }




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
            return Ok(await servicioTipoAdministradorModulo.Eliminar(lids).ConfigureAwait(false));

        }
    }
}