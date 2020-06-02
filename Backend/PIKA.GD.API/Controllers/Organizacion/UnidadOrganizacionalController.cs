using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PIKA.GD.API.Filters;
using PIKA.GD.API.Model;
using PIKA.Infraestructura.Comun.Constantes;
using PIKA.Modelo.Metadatos;
using PIKA.Modelo.Organizacion;
using PIKA.Servicio.Organizacion;
using RepositorioEntidades;


namespace PIKA.GD.API.Controllers.Organizacion
{

    [Authorize]
    [ApiVersion("1.0")]
    [ApiController]
    [Route("api/v{version:apiVersion}/org/[controller]")]
    public class UnidadOrganizacionalController : ACLController
    {

        private ILogger<UnidadOrganizacionalController> logger;
        private IServicioUnidadOrganizacional servicioUO;
        private IProveedorMetadatos<UnidadOrganizacional> metadataProvider;
        public UnidadOrganizacionalController(ILogger<UnidadOrganizacionalController> logger,
            IProveedorMetadatos<UnidadOrganizacional> metadataProvider,
            IServicioUnidadOrganizacional servicioUO)
        {
            this.logger = logger;
            this.servicioUO = servicioUO;
            this.metadataProvider = metadataProvider;
        }


        /// <summary>
        /// Obtiene los metadatos relacionados con la entidad Unidad Organizacional
        /// </summary>
        /// <returns></returns>
        [HttpGet("metadata", Name = "MetadataOU")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        public async Task<ActionResult<MetadataInfo>> GetMetadata([FromQuery]Consulta query = null)
        {
            return Ok(await metadataProvider.Obtener().ConfigureAwait(false));
        }


        /// <summary>
        /// Añade una nueva entidad del tipo unidad organizacional
        /// </summary>
        /// <param name="entidad"></param>
        /// <returns></returns>
        [HttpPost]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<UnidadOrganizacional>> Post([FromBody]UnidadOrganizacional entidad)
        {

            entidad = await servicioUO.CrearAsync(entidad).ConfigureAwait(false);
            return Ok(CreatedAtAction("GetOU", new { id = entidad.Id }, entidad).Value);
        }


        /// <summary>
        /// Actualoza uan entidad unidad organizacional, el Id debe incluirse en el querystring así como en 
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
        public async Task<IActionResult> Put(string id, [FromBody]UnidadOrganizacional entidad)
        {
            var x = ObtieneFiltrosIdentidad();

            if (id != entidad.Id)
            {
                return BadRequest();
            }

            await servicioUO.ActualizarAsync(entidad).ConfigureAwait(false);
            return NoContent();

        }



        /// <summary>
        /// Obtiene una página de datos del dominio 
        /// </summary>
        /// <param name="query">Consulta para el paginado</param>
        /// <returns></returns>
        [HttpGet("page", Name = "GetPageUO")]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<Paginado<UnidadOrganizacional>>> GetPage([ModelBinder(typeof(GenericDataPageModelBinder))][FromQuery]Consulta query = null)
        {
            ///Añade las propiedaes del contexto para el filtro de ACL vía ACL Controller
            query.Filtros.AddRange(ObtieneFiltrosIdentidad());
            var data = await servicioUO.ObtenerPaginadoAsync(query).ConfigureAwait(false);

            return Ok(data);
        }


        /// <summary>
        /// Obtiene una página de datos de unidades organizacionales para un dominio
        /// </summary>
        /// <param name="query">Consulta para el paginado</param>
        /// <returns></returns>
        [HttpGet("page/dominio/{id}", Name = "GetPageUODominio")]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<Paginado<UnidadOrganizacional>>> GetPageDominio(string id, [ModelBinder(typeof(GenericDataPageModelBinder))][FromQuery] Consulta query = null)
        {
            ///Añade las propiedaes del contexto para el filtro de ACL vía ACL Controller
            query.Filtros.AddRange(ObtieneFiltrosIdentidad());
            query.Filtros.Add(new FiltroConsulta() { Propiedad = UnidadOrganizacional.CampoDominioId, Operador = FiltroConsulta.OP_EQ, Valor = id });
            var data = await servicioUO.ObtenerPaginadoAsync(query).ConfigureAwait(false);

            return Ok(data);
        }



        /// <summary>
        /// Obtiene una unidad organizacional en base al Id único
        /// </summary>
        /// <param name="id">Id único del dominio</param>
        /// <returns></returns>
        [HttpGet("{id}")]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<UnidadOrganizacional>> Get(string id)
        {
            var o = await servicioUO.UnicoAsync(x=>x.Id==id).ConfigureAwait(false);
            if (o != null) return Ok(o);
            return NotFound(id);
        }


        /// <summary>
        /// Marca como eliminados una lista de unidades organizacionales en base al arreglo de identificadores recibidos
        /// </summary>
        /// <param name="ids">Arreglo de identificadores string</param>
        /// <returns></returns>
        [HttpDelete]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult> Delete([FromBody ]string[] id)
        {
            return Ok(await servicioUO.Eliminar(id).ConfigureAwait(false));
        }


        /// <summary>
        /// Restaura una lista de unidades oraganizacionales eliminados en base al arreglo de identificadores recibidos
        /// </summary>
        /// <param name="ids">Arreglo de identificadores string</param>
        /// <returns></returns>
        [HttpPatch("restaurar", Name = "restaurarUO")]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult> Undelete([FromBody] string[] ids)
        {
            return Ok(await servicioUO.Restaurar(ids).ConfigureAwait(false));
        }

    }
}
