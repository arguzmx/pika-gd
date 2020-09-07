using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PIKA.GD.API.Filters;
using PIKA.GD.API.Model;
using PIKA.Modelo.Metadatos;
using PIKA.Modelo.Organizacion;
using PIKA.Servicio.Organizacion.Interfaces;
using RepositorioEntidades;


namespace PIKA.GD.API.Controllers.Organizacion
{

    [Authorize]
    [ApiVersion("1.0")]
    [ApiController]
    [Route("api/v{version:apiVersion}/org/[controller]")]
    public class RolController : ACLController
    {
        private ILogger<RolController> logger;
        private IServicioRol servicioRol;
        private IProveedorMetadatos<Rol> metadataProvider;

        public RolController(ILogger<RolController> logger,
          IProveedorMetadatos<Rol> metadataProvider,
          IServicioRol servicioRol)
        {
            this.logger = logger;
            this.servicioRol = servicioRol;
            this.metadataProvider = metadataProvider;
        }

        /// <summary>
        /// Obtiene los metadatos relacionados con la entidad Rol
        /// </summary>
        /// <returns></returns>
        [HttpGet("metadata", Name = "MetadataRol")]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<MetadataInfo>> GetMetadata([FromQuery]Consulta query = null)
        {
            return Ok(await metadataProvider.Obtener().ConfigureAwait(false));
        }


        /// <summary>
        /// Añade una nueva entidad del tipo rol
        /// </summary>
        /// <param name="entidad"></param>
        /// <returns></returns>
        [HttpPost]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<Rol>> Post([FromBody]Rol entidad)
        {
            entidad = await servicioRol.CrearAsync(entidad).ConfigureAwait(false);
            return Ok(CreatedAtAction("GetRol", new { id = entidad.Id }, entidad).Value);
        }


        /// <summary>
        /// Actualoza uan entidad Rol, el Id debe incluirse en el querystring así como en 
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
        public async Task<IActionResult> Put(string id, [FromBody]Rol entidad)
        {
            var x = ObtieneFiltrosIdentidad();

            if (id != entidad.Id)
            {
                return BadRequest();
            }

            await servicioRol.ActualizarAsync(entidad).ConfigureAwait(false);
            return NoContent();
        }

        /// <summary>
        /// Obtiene una página de datos del dominio 
        /// </summary>
        /// <param name="query">Consulta para el paginado</param>
        /// <returns></returns>
        [HttpGet("page", Name = "GetPageRoles")]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<Paginado<Rol>>> GetPage([ModelBinder(typeof(GenericDataPageModelBinder))][FromQuery]Consulta query = null)
        {
            ///Añade las propiedaes del contexto para el filtro de ACL vía ACL Controller
            query.Filtros.AddRange(ObtieneFiltrosIdentidad());
            var filtroOrigen = query.Filtros.Where(x => x.Propiedad == "DominioId").SingleOrDefault();
            if(filtroOrigen != null)
            {
                filtroOrigen.Propiedad = "OrigenId";
                query.Filtros.Add(filtroOrigen);

                query.Filtros.Add(new FiltroConsulta() { Operador = FiltroConsulta.OP_EQ, Propiedad = "TipoOrigenId", Valor = "dominio" });
            }
            var data = await servicioRol.ObtenerPaginadoAsync(query).ConfigureAwait(false);
            return Ok(data);
        }


        [HttpGet("todos")]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<Rol>> GetRoles()
        {
            logger.LogError(this.DominioId);
            var o = await servicioRol.ObtieneRoles(this.DominioId).ConfigureAwait(false);
            return Ok(o);
        }

        // <summary>
        /// Obtiene un Rol en base al Id único
        /// </summary>
        /// <param name="id">Id único del dominio</param>
        /// <returns></returns>
        [HttpGet("{id}")]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<Rol>> Get(string id)
        {
            var o = await servicioRol.UnicoAsync(x => x.Id == id).ConfigureAwait(false);
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
        public async Task<ActionResult> Delete([FromBody]string[] id)
        {
            return Ok(await servicioRol.Eliminar(id).ConfigureAwait(false));
        }


        /// <summary>
        /// Añade una nueva entidad del tipo rol
        /// </summary>
        /// <param name="entidad"></param>
        /// <returns></returns>
        [HttpPost("vincular/{rolid}", Name = "vincularUsuerios")]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult> PostVincular(string rolid, [FromBody] string[] ids)
        {
            var resultado = await servicioRol.Vincular(rolid, ids).ConfigureAwait(false);
            return Ok(resultado.ToList());
        }

        /// <summary>
        /// Añade una nueva entidad del tipo rol
        /// </summary>
        /// <param name="entidad"></param>
        /// <returns></returns>
        [HttpPost("desvincular/{rolid}", Name = "desvincularUsuerios")]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult> PostDesvincular(string rolid, [FromBody] string[] ids)
        {
            var resultado = await servicioRol.Desvincular(rolid, ids).ConfigureAwait(false);
            return Ok(resultado.ToList());
        }

    }
}
