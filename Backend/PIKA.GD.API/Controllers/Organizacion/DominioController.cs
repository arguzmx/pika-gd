using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NSwag.Annotations;
using PIKA.GD.API.Filters;
using PIKA.GD.API.Model;
using PIKA.Infraestructura.Comun.Constantes;
using PIKA.Modelo.Infraestructura.UI;
using PIKA.Modelo.Metadatos;
using PIKA.Modelo.Organizacion;
using PIKA.Servicio.Organizacion;
using RepositorioEntidades;
using PIKA.Servicio.Organizacion.Servicios;

namespace PIKA.GD.API.Controllers.Organizacion
{



    [Authorize]
    [ApiVersion("1.0")]
    [ApiController]
    [Route("api/v{version:apiVersion}/org/[controller]")]
    public class DominioController : ACLController
    {

        private ILogger<DominioController> logger;
        private IServicioDominio servicioEntidad;
        private IProveedorMetadatos<Dominio> metadataProvider;
        public DominioController(ILogger<DominioController> logger,
            IProveedorMetadatos<Dominio> metadataProvider,
            IServicioDominio servicioEntidad)
        {
            this.logger = logger;
            this.servicioEntidad = servicioEntidad;
            this.metadataProvider = metadataProvider;
        }

        /// <summary>
        /// Obtiene los metadatos relacionados con la entidad Dominio
        /// </summary>
        /// <returns></returns>
        [HttpGet("metadata", Name = "MetadataDominio")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        public async Task<ActionResult<MetadataInfo>> GetMetadata()
        {
            return Ok(await metadataProvider.Obtener().ConfigureAwait(false));
        }


        /// <summary>
        /// Añade una nueva entidad del tipo Dominio
        /// </summary>
        /// <param name="entidad"></param>
        /// <returns></returns>
        [HttpPost]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<Dominio>> Post([FromBody]Dominio entidad)
        {
            entidad = await servicioEntidad.CrearAsync(entidad).ConfigureAwait(false);
            return Ok(CreatedAtAction("GetDomain", new { id = entidad.Id }, entidad).Value);
        }


        /// <summary>
        /// Actualoza uan entidad dominio, el Id debe incluirse en el querystring así como en 
        /// el serializado para la petición PUT
        /// </summary>
        /// <param name="id">Identificador único del dominio</param>
        /// <param name="entidad">Datos serialziados del dominio</param>
        /// <returns></returns>
        [HttpPut("{id}")]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> Put(string id, [FromBody]Dominio entidad)
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
        /// Obtiene una página de datos del dominio 
        /// </summary>
        /// <param name="query">Consulta para el paginado</param>
        /// <returns></returns>
        [HttpGet("page", Name = "GetPageDominio")]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<Paginado<Dominio>>> GetPage([ModelBinder(typeof(GenericDataPageModelBinder))][FromQuery]Consulta query = null)
        {
            ///Añade las propiedaes del contexto para el filtro de ACL vía ACL Controller
            query.Filtros.AddRange(ObtieneFiltrosIdentidad());
            var data = await servicioEntidad.ObtenerPaginadoAsync(query).ConfigureAwait(false);

            return Ok(data);
        }


        /// <summary>
        /// Obtiene un dominio en base al Id único
        /// </summary>
        /// <param name="id">Id único del dominio</param>
        /// <returns></returns>
        [HttpGet("{id}")]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<Dominio>> Get(string id)
        {
            var o = await servicioEntidad.UnicoAsync(x => x.Id == id).ConfigureAwait(false);
            if (o != null) return Ok(o);
            return NotFound(id);
        }



       /// <summary>
       /// Marca como eliminados una lista de dominiios en base al arreglo de identificadores recibidos
       /// </summary>
       /// <param name="ids">Arreglo de identificadores string</param>
       /// <returns></returns>
        [HttpDelete]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult> Delete([FromBody]string[] ids)
        {
            return Ok(await servicioEntidad.Eliminar(ids).ConfigureAwait(false));
        }


        /// <summary>
        /// Restaura una lista dede dominiios eliminados en base al arreglo de identificadores recibidos
        /// </summary>
        /// <param name="ids">Arreglo de identificadores string</param>
        /// <returns></returns>
        [HttpPatch("restaurar", Name = "restaurarDominio")]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult> Undelete([FromBody] string[] ids)
        {
            return Ok(await servicioEntidad.Restaurar(ids).ConfigureAwait(false));
        }

    }
}