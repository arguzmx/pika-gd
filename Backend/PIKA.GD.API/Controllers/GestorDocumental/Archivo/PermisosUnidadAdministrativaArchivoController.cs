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
using PIKA.Infraestructura.Comun.Seguridad;
using PIKA.Modelo.Contenido;
using PIKA.Modelo.GestorDocumental;
using PIKA.Modelo.Metadatos;
using PIKA.Servicio.Contenido.Interfaces;
using PIKA.Servicio.GestionDocumental.Interfaces;
using RepositorioEntidades;

namespace PIKA.GD.API.Controllers.Contenido
{
    [Authorize]
    [ApiVersion("1.0")]
    [ApiController]
    [Route("api/v{version:apiVersion}/contenido/[controller]")]
    public class PermisosUnidadAdministrativaArchivoController : ACLController
    {

        private ILogger<PermisosUnidadAdministrativaArchivoController> logger;
        private IServicioPermisosUnidadAdministrativaArchivo servicioEntidad;
        private IProveedorMetadatos<PermisosUnidadAdministrativaArchivo> metadataProvider;
        public PermisosUnidadAdministrativaArchivoController(ILogger<PermisosUnidadAdministrativaArchivoController> logger,
            IProveedorMetadatos<PermisosUnidadAdministrativaArchivo> metadataProvider,
            IServicioPermisosUnidadAdministrativaArchivo servicioEntidad)
        {
            this.logger = logger;
            this.servicioEntidad = servicioEntidad;
            this.metadataProvider = metadataProvider;
        }


        public override void EmiteConfiguracionSeguridad(UsuarioAPI usuario, ContextoRegistroActividad RegistroActividad, List<EventoAuditoriaActivo> Eventos)
        {
            servicioEntidad.EstableceContextoSeguridad(usuario, RegistroActividad, Eventos);
        }

        /// <summary>
        /// Otiene los metadatos asociados al permiso
        /// </summary>
        /// <returns></returns>
        [HttpGet("metadata", Name = "MetadataPermisoUAdmin")]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<MetadataInfo>> GetMetadata()
        {
            return Ok(await metadataProvider.Obtener().ConfigureAwait(false));
        }


        /// <summary>
        ///  Añade un nuevo permiso de contenido
        /// </summary>
        /// <param name="entidad"></param>
        /// <returns></returns>
        [HttpPost]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<Permiso>> Post([FromBody]PermisosUnidadAdministrativaArchivo entidad)
        {
            entidad = await servicioEntidad.CrearAsync(entidad).ConfigureAwait(false);
            return Ok(CreatedAtAction("GetPermiso", new { id = entidad.Id }, entidad).Value);
        }

        /// <summary>
        /// Actualiza unq entidad Permiso de contenido existente, el Id debe incluirse en el Querystring así como en 
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
        public async Task<IActionResult> Put(string id, [FromBody] PermisosUnidadAdministrativaArchivo entidad)
        {
            var x = ObtieneFiltrosIdentidad();


            if (id != entidad.Id)
            {
                return BadRequest();
            }

            await servicioEntidad.ActualizarAsync(entidad).ConfigureAwait(false);
            return NoContent();

        }

        /// <summary>
        /// Otiene una página de resultados de permisos de contenid en base a la configuración de paginado u al query recibido 
        /// </summary>
        /// <param name="query">Query de filtrado para el paginado</param>
        /// <returns></returns>
        [HttpGet("page/unidadadministrativaarchivo/{id}", Name = "GetPagePermisoUnidadArchivo")]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<PermisosUnidadAdministrativaArchivo>>> GetPage(string id, [ModelBinder(typeof(GenericDataPageModelBinder))][FromQuery]Consulta query = null)
        {
            ///Añade las propiedaes del contexto para el filtro de ACL vía ACL Controller
            query.Filtros.AddRange(ObtieneFiltrosIdentidad());
            query.Filtros.Add(new FiltroConsulta() { Negacion = false, Operador = FiltroConsulta.OP_EQ, 
                Propiedad = "UnidadAdministrativaArchivoId", Valor = id, ValorString = id}); 
            
            var data = await servicioEntidad.ObtenerPaginadoAsync(query).ConfigureAwait(false);
            return Ok(data);
        }


        /// <summary>
        /// Obtiene un permiso en base al Id único
        /// </summary>
        /// <param name="id">Id único del país</param>
        /// <returns></returns>
        [HttpGet("{id}")]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<PermisosUnidadAdministrativaArchivo>> Get(string id)
        {
            var o = await servicioEntidad.UnicoAsync(x => x.Id == id).ConfigureAwait(false);
            if (o != null) return Ok(o);
            return NotFound(id);
        }




        /// <summary>
        /// Elimina de manera permanente un Permiso de contenido en base al arreglo de identificadores recibidos
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
                IdsTrim += item.Trim() + "";
            }

            string[] lids = IdsTrim.Split(',').ToList()
           .Where(x => !string.IsNullOrEmpty(x)).ToArray();
            return Ok(await servicioEntidad.Eliminar(lids).ConfigureAwait(false));
        }

    }
}