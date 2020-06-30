using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PIKA.GD.API.Filters;
using PIKA.GD.API.Model;
using PIKA.Modelo.Contacto;
using PIKA.Modelo.Metadatos;
using PIKA.Servicio.Contacto;
using RepositorioEntidades;


namespace PIKA.GD.API.Controllers.Contacto
{



    [Authorize]
    [ApiVersion("1.0")]
    [ApiController]
    [Route("api/v{version:apiVersion}/contacto/[controller]")]
    public class DireccionPostalController : ACLController
    {

        private ILogger<DireccionPostalController> logger;
        private IServicioDireccionPostal servicioDirPost;
        private IProveedorMetadatos<DireccionPostal> metadataProvider;
        public DireccionPostalController(ILogger<DireccionPostalController> logger,
            IProveedorMetadatos<DireccionPostal> metadataProvider,
            IServicioDireccionPostal servicioDirPost)
        {
            this.logger = logger;
            this.servicioDirPost = servicioDirPost;
            this.metadataProvider = metadataProvider;
        }

        /// <summary>
        /// Obtiene los metadatos relacionados con la entidad dirección postal
        /// </summary>
        /// <returns></returns>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpGet("metadata", Name = "MetadataDirPostal")]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        public async Task<ActionResult<MetadataInfo>> GetMetadata([FromQuery]Consulta query = null)
        {
            return Ok(await metadataProvider.Obtener().ConfigureAwait(false));
        }



        /// <summary>
        /// Añade una nueva entidad del tipo dirección postal
        /// </summary>
        /// <param name="entidad"></param>
        /// <returns></returns>
        [HttpPost]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<DireccionPostal>> Post([FromBody]DireccionPostal entidad)
        {
            entidad = await servicioDirPost.CrearAsync(entidad).ConfigureAwait(false);
            return Ok(CreatedAtAction("GetDomain", new { id = entidad.Id }, entidad).Value);
        }


        /// <summary>
        /// Actualoza uan entidad dirección postal, el Id debe incluirse en el querystring así como en 
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
        public async Task<IActionResult> Put(string id, [FromBody]DireccionPostal entidad)
        {
            var x = ObtieneFiltrosIdentidad();


            if (id != entidad.Id)
            {
                return BadRequest();
            }

            await servicioDirPost.ActualizarAsync(entidad).ConfigureAwait(false);
            return NoContent();

        }


/// <summary>
/// DEvulve un alista de direcciones postales asociadas al objeto del tipo especificado
/// </summary>
/// <param name="tipo">Tipo de dasto para asociar la dirección</param>
/// <param name="id">Identificador único del objeto asociado a la dirección</param>
/// <param name="query">Consulta para la paginación y búsqueda</param>
/// <returns></returns>
        [HttpGet("page/{tipo}/{id}", Name = "GetPageDirPostal")]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<Paginado<DireccionPostal>>> GetPage(
            string tipo, string id, 
            [ModelBinder(typeof(GenericDataPageModelBinder))][FromQuery]Consulta query = null)
        {

            ///Añade las propiedaes del contexto para el filtro de ACL vía ACL Controller
            //query.Filtros.AddRange(ObtieneFiltrosIdentidad());

            query.Filtros.Add(new FiltroConsulta() { Propiedad = IEntidadRelacionada.CampoTipo, Operador = FiltroConsulta.OP_EQ, Valor = tipo });
            query.Filtros.Add(new FiltroConsulta() { Propiedad = IEntidadRelacionada.CampoId, Operador = FiltroConsulta.OP_EQ, Valor = id });

            var data = await servicioDirPost.ObtenerPaginadoAsync(
                Query: query,
                include: null)
                .ConfigureAwait(false);

            return Ok(data);
        }



        /// <summary>
        /// Obtiene una dirección postal en base al Id único
        /// </summary>
        /// <param name="id">Id único del dominio</param>
        /// <returns></returns>
        [HttpGet("{id}")]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<DireccionPostal>> Get(string id)
        {
            var o = await servicioDirPost.UnicoAsync(
                predicado: x => x.Id == id, 
                incluir: y => y.Include(z => z.Pais).Include(z => z.Estado))
                .ConfigureAwait(false);

            if (o != null) return Ok(o);
            return NotFound(id);
        }




        /// <summary>
        /// Elimina de manera permanente una dirección postal en base al arreglo de identificadores recibidos
        /// </summary>
        /// <param name="ids">Arreglo de identificadores string</param>
        /// <returns></returns>
        [HttpDelete("{ids}")]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult> Delete(string ids)
        {
            string[] lids = ids.Split(',').ToList()
                .Where(x => !string.IsNullOrEmpty(x)).ToArray();
            return Ok(await servicioDirPost.Eliminar(lids).ConfigureAwait(false));
        }


        [HttpPatch("principal/{id}/on", Name = "EstableceDireccionPrincipal" )]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult> PrincipalOn(string id)
        {
            await servicioDirPost.EstablecerPrincipal(id).ConfigureAwait(false);
            return Ok();
        }


        [HttpPatch("principal/{id}/off", Name = "RemoverDireccionPrincipal")]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult> PrincipalOff(string id)
        {
            await servicioDirPost.RemoverPrincipal(id).ConfigureAwait(false);
            return Ok();
        }

    }
}