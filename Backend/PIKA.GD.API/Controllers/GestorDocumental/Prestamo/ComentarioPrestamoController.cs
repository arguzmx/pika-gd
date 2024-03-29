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
using PIKA.Infraestructura.Comun.Seguridad;
using PIKA.Modelo.GestorDocumental;
using PIKA.Modelo.Metadatos;
using PIKA.Servicio.GestionDocumental.Interfaces;
using PIKA.Servicio.GestionDocumental.Servicios;
using RepositorioEntidades;


namespace PIKA.GD.API.Controllers.GestorDocumental
{
    [Authorize]
    [ApiVersion("1.0")]
    [ApiController]
    [Route("api/v{version:apiVersion}/gd/[controller]")]
    public class ComentarioPrestamoController : ACLController
    {
        private readonly ILogger<ComentarioPrestamoController> logger;
        private IServicioComentarioPrestamo servicioComentarioPrestamo;
        private IProveedorMetadatos<ComentarioPrestamo> metadataProvider;
        public ComentarioPrestamoController(ILogger<ComentarioPrestamoController> logger,
            IProveedorMetadatos<ComentarioPrestamo> metadataProvider,
            IServicioComentarioPrestamo servicioComentarioPrestamo)
        {
            this.logger = logger;
            this.servicioComentarioPrestamo = servicioComentarioPrestamo;
            this.metadataProvider = metadataProvider;
        }


        public override void EmiteConfiguracionSeguridad(UsuarioAPI usuario, ContextoRegistroActividad RegistroActividad, List<EventoAuditoriaActivo> Eventos)
        {
            servicioComentarioPrestamo.EstableceContextoSeguridad(usuario, RegistroActividad, Eventos);
        }

        [HttpGet("metadata", Name = "MetadataComentarioPrestamo")]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        public async Task<ActionResult<MetadataInfo>> GetMetadata([FromQuery]Consulta query = null)
        {
            return Ok(await metadataProvider.Obtener().ConfigureAwait(false));
        }



        [HttpPost]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        public async Task<ActionResult<ComentarioPrestamo>> Post([FromBody]ComentarioPrestamo entidad)
        {
            entidad = await servicioComentarioPrestamo.CrearAsync(entidad).ConfigureAwait(false);
            return Ok(CreatedAtAction("GetComentarioPrestamo", new { id = entidad.Id }, entidad).Value);
        }


        [HttpPut("{id}")]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        public async Task<IActionResult> Put(string id, [FromBody]ComentarioPrestamo entidad)
        {
            var x = ObtieneFiltrosIdentidad();


            if (id != entidad.Id)
            {
                return BadRequest();
            }

            await servicioComentarioPrestamo.ActualizarAsync(entidad).ConfigureAwait(false);
            return NoContent();

        }


        [HttpGet("page", Name = "GetPageComentarioPrestamo")]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        public async Task<ActionResult<Paginado<ComentarioPrestamo>>> GetPage([ModelBinder(typeof(GenericDataPageModelBinder))][FromQuery]Consulta query = null)
        {
            ///Añade las propiedaes del contexto para el filtro de ACL vía ACL Controller
            var data = await servicioComentarioPrestamo.ObtenerPaginadoAsync(
                   Query: query,
                   include: null)
                   .ConfigureAwait(false);

            return Ok(data);
        }



        [HttpGet("{id}")]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        public async Task<ActionResult<ComentarioPrestamo>> Get(string id)
        {
            var o = await servicioComentarioPrestamo.UnicoAsync(x => x.Id == id).ConfigureAwait(false);
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
            return Ok(await servicioComentarioPrestamo.Eliminar(lids).ConfigureAwait(false));
        }
    }
}