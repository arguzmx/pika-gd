using System;
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
    public class DestinatarioPermisoController : ACLController
    {

        private ILogger<DestinatarioPermisoController> logger;
        private IServicioDestinatarioPermiso servicioEntidad;
        private IProveedorMetadatos<DestinatarioPermiso> metadataProvider;
        public DestinatarioPermisoController(ILogger<DestinatarioPermisoController> logger,
            IProveedorMetadatos<DestinatarioPermiso> metadataProvider,
            IServicioDestinatarioPermiso servicioEntidad)
        {
            this.logger = logger;
            this.servicioEntidad = servicioEntidad;
            this.metadataProvider = metadataProvider;
        }


        /// <summary>
        /// Otiene los metadatos asociados al destintario del permiso
        /// </summary>
        /// <returns></returns>
        [HttpGet("metadata", Name = "MetadataDestinatarioPermisoContenid")]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<MetadataInfo>> GetMetadata()
        {
            return Ok(await metadataProvider.Obtener().ConfigureAwait(false));
        }


        /// <summary>
        ///  Añade un nuevo destintario al permiso de contenido
        /// </summary>
        /// <param name="entidad"></param>
        /// <returns></returns>
        [HttpPost]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<DestinatarioPermiso>> Post([FromBody]DestinatarioPermiso entidad)
        {
            entidad = await servicioEntidad.CrearAsync(entidad).ConfigureAwait(false);
            return Ok(CreatedAtAction("GetDestinatarioPermiso", new { id = entidad.DestinatarioId }, entidad).Value);
        }


        /// <summary>
        /// Otiene una página de resultados de destinatarios de los permisos de contenid en base a la configuración de paginado u al query recibido 
        /// </summary>
        /// <param name="query">Query de filtrado para el paginado</param>
        /// <returns></returns>
        [HttpGet("page/permiso/{id}", Name = "GetPageDestinatarioPermiso")]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<DestinatarioPermiso>>> GetPage(string id,
            [ModelBinder(typeof(GenericDataPageModelBinder))][FromQuery]Consulta query = null)
        {
            ///Añade las propiedaes del contexto para el filtro de ACL vía ACL Controller
            query.Filtros.AddRange(ObtieneFiltrosIdentidad());
            query.Filtros.Add(new FiltroConsulta() { Propiedad = "PermisoId", Operador = FiltroConsulta.OP_EQ, Valor = id });

            var data = await servicioEntidad.ObtenerPaginadoAsync(query).ConfigureAwait(false);
            return Ok(data);
        }


        /// <summary>
        /// Obtiene un destinatrio de permiso en base los identificadores clave
        /// </summary>
        /// <param name="permisoid">Identificador del permiso</param>
        /// <param name="destinatarioid">Identificador del destinatario</param>
        /// <returns></returns>
        [HttpGet("{permisoid}/{destinatarioid}")]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<DestinatarioPermiso>> Get(string permisoid, string destinatarioid)
        {
            var o = await servicioEntidad.UnicoAsync(x => x.PermisoId == permisoid &&
            x.DestinatarioId == destinatarioid).ConfigureAwait(false);
            if (o != null) return Ok(o);
            return NotFound(new { permisoid, destinatarioid });
        }




        /// <summary>
        /// Elimina de manera permanente un DestinatarioPermiso de contenido en base al arreglo de identificadores recibidos
        /// </summary>
        /// <param name="ids">Lista de identificadores separados por omas</param>
        /// <returns></returns>
        [HttpDelete("{permisoId}/{ids}")]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult> Delete(string permisoId, string ids)
        {
            string[] lids = ids.Split(',').ToList()
                .Where(x => !string.IsNullOrEmpty(x)).ToArray();
            return Ok(await servicioEntidad.Eliminar(permisoId, lids).ConfigureAwait(false));
        }

    }
}