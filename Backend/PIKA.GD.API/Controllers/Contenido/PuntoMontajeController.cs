﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PIKA.GD.API.Filters;
using PIKA.GD.API.Model;
using PIKA.Infraestructura.Comun.Seguridad;
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
    public class PuntoMontajeController : ACLController
    {

        private ILogger<PuntoMontajeController> logger;
        private IServicioPuntoMontaje servicioEntidad;
        private IProveedorMetadatos<PuntoMontaje> metadataProvider;
        private readonly IServicioTokenSeguridad ServicioTokenSeguridad;
        public PuntoMontajeController(ILogger<PuntoMontajeController> logger,
            IProveedorMetadatos<PuntoMontaje> metadataProvider,
            IServicioPuntoMontaje servicioEntidad,
            IServicioTokenSeguridad ServicioTokenSeguridad)
        {
            this.logger = logger;
            this.servicioEntidad = servicioEntidad;
            this.metadataProvider = metadataProvider;
            this.ServicioTokenSeguridad = ServicioTokenSeguridad;
        }

        public override void EmiteConfiguracionSeguridad(UsuarioAPI usuario, ContextoRegistroActividad RegistroActividad, List<EventoAuditoriaActivo> Eventos)
        {
            servicioEntidad.EstableceContextoSeguridad(usuario, RegistroActividad, Eventos);
        }

        /// <summary>
        /// Otiene los metadatos asociados al punto de montaje
        /// </summary>
        /// <returns></returns>
        [HttpGet("metadata", Name = "MetadataPuntoMontajeContenido")]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<MetadataInfo>> GetMetadata()
        {
            return Ok(await metadataProvider.Obtener().ConfigureAwait(false));
        }


        [HttpGet("permisos/{id}", Name = "PermisosPuntoMontaje")]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<PermisosPuntoMontaje>> GetPermisos(string id)
        {
            servicioEntidad.usuario = this.usuario;
            var permisos = await this.servicioEntidad.ObtienePerrmisos(id,null, TenantId).ConfigureAwait(false);
            return Ok(permisos);
        }

        /// <summary>
        ///  Añade un nuevo punto montaje 
        /// </summary>
        /// <param name="entidad"></param>
        /// <returns></returns>
        [HttpPost]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<PuntoMontaje>> Post([FromBody]PuntoMontaje entidad)
        {
            servicioEntidad.usuario = this.usuario;
            entidad.CreadorId = this.UsuarioId;
            entidad = await servicioEntidad.CrearAsync(entidad).ConfigureAwait(false);
            return Ok(CreatedAtAction("GetPuntoMontaje", new { id = entidad.Id }, entidad).Value);
        }

        /// <summary>
        /// Actualiza unq entidad punto de montaje  existente, el Id debe incluirse en el Querystring así como en 
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
        public async Task<IActionResult> Put(string id, [FromBody]PuntoMontaje entidad)
        {
            var x = ObtieneFiltrosIdentidad();
            servicioEntidad.usuario = this.usuario;

            if (id != entidad.Id)
            {
                return BadRequest();
            }

            await servicioEntidad.ActualizarAsync(entidad).ConfigureAwait(false);
            return NoContent();

        }

        /// <summary>
        /// Otiene una página de resultados de punto de montajes en base a la configuración de paginado u al query recibido 
        /// </summary>
        /// <param name="query">Query de filtrado para el paginado</param>
        /// <returns></returns>
        [HttpGet("page", Name = "GetPagePuntoMontaje")]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<Paginado<PuntoMontaje>>> GetPage([ModelBinder(typeof(GenericDataPageModelBinder))][FromQuery]Consulta query = null)
        {
            var permiso = await ServicioTokenSeguridad.PermisosModuloId(this.UsuarioId, this.DominioId, "PERMISOS-CONTENIDO")
            .ConfigureAwait(false);

            servicioEntidad.usuario = this.usuario;
            servicioEntidad.permisos = permiso;

            ///Añade las propiedaes del contexto para el filtro de ACL vía ACL Controller
            query.Filtros.AddRange(ObtieneFiltrosIdentidadSinDominio());

            var data = await servicioEntidad.ObtenerPaginadoAsync(query).ConfigureAwait(false);
            return Ok(data);
        }


        /// <summary>
        /// Obtiene un punto de montaje en base al Id único
        /// </summary>
        /// <param name="id">Id único del país</param>
        /// <returns></returns>
        [HttpGet("{id}")]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<PuntoMontaje>> Get(string id)
        {
            servicioEntidad.usuario = this.usuario;
            var o = await servicioEntidad.UnicoAsync(x => x.Id == id).ConfigureAwait(false);
            if (o != null) return Ok(o);
            return NotFound(id);
        }


        /// <summary>
        /// Elimina de manera lógica un punto de montaje en base al arreglo de identificadores recibidos
        /// </summary>
        /// <param name="ids">Lista de identificadores separados por omas</param>
        /// <returns></returns>
        [HttpDelete("{ids}")]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult> Delete(string ids)
        {
            servicioEntidad.usuario = this.usuario;
            string IdsTrim = "";
            foreach (string item in ids.Split(',').ToList().Where(x => !string.IsNullOrEmpty(x)).ToArray())
            {
                IdsTrim += item.Trim() + ",";
            }
            string[] lids = IdsTrim.Split(',').ToList()
           .Where(x => !string.IsNullOrEmpty(x)).ToArray();
            return Ok(await servicioEntidad.Eliminar(lids).ConfigureAwait(false));
        }

        /// <summary>
        /// Restaura una lista dede puntos de montaje eliminados en base al arreglo de identificadores recibidos
        /// </summary>
        /// <param name="ids">Arreglo de identificadores string</param>
        /// <returns></returns>
        [HttpPatch("restaurar/{ids}", Name = "RestaurarPuntoMontaje")]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult> Undelete(string ids)
        {
            servicioEntidad.usuario = this.usuario;
            string IdsTrim = "";
            foreach (string item in ids.Split(',').ToList().Where(x => !string.IsNullOrEmpty(x)).ToArray())
            {
                IdsTrim += item.Trim() + ",";
            }
            string[] lids = IdsTrim.Split(',').ToList()
           .Where(x => !string.IsNullOrEmpty(x)).ToArray();
            return Ok(await servicioEntidad.Restaurar(lids).ConfigureAwait(false));
        }


        /// <summary>
        /// Obtiene una lista de volumnees en base a los parámetros de consulta
        /// </summary>
        /// <param name="query">Query de busqueda a la base de datos</param>
        /// <returns></returns>

        [HttpGet("pares", Name = "GetParesPuntoMontaje")]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<List<ValorListaOrdenada>>> GetPares(
        [ModelBinder(typeof(GenericDataPageModelBinder))][FromQuery] Consulta query = null)
        {
            servicioEntidad.usuario = this.usuario;
            var data = await servicioEntidad.ObtenerParesAsync(query)
                .ConfigureAwait(false);

            return Ok(data);
        }
        /// <summary>
        /// Obtiene una lista de volumenes en base a con el parámetro ID de consulta
        /// </summary>
        /// <param name="ids">parametro Id para consulta a la base de datos</param>
        /// <returns></returns>

        [HttpGet("pares/{ids}", Name = "GetParesPuntoMontajeporId")]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<List<ValorListaOrdenada>>> GetParesporId(
              string ids)
        {
            servicioEntidad.usuario = this.usuario;
            List<string> lids = ids.Split(',').ToList()
               .Where(x => !string.IsNullOrEmpty(x)).ToList();
            var data = await servicioEntidad.ObtenerParesPorId(lids)
                .ConfigureAwait(false);

            return Ok(data);
        }

    }
}