using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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
    public class MedioContactoController : ACLController
    {

        private ILogger<MedioContactoController> logger;
        private IServicioMedioContacto servicioEntidad;
        private IProveedorMetadatos<MedioContacto> metadataProvider;
        public MedioContactoController(ILogger<MedioContactoController> logger,
            IProveedorMetadatos<MedioContacto> metadataProvider,
            IServicioMedioContacto servicioEntidad)
        {
            
            this.logger = logger;
            this.servicioEntidad = servicioEntidad;
            this.metadataProvider = metadataProvider;

        }

        /// <summary>
        /// Obtiene los metadatos relacionados con la entidad medio de contacto
        /// </summary>
        /// <returns></returns>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpGet("metadata", Name = "MetadataMedioContacto")]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        public async Task<ActionResult<MetadataInfo>> GetMetadata([FromQuery]Consulta query = null)
        {
            return Ok(await metadataProvider.Obtener().ConfigureAwait(false));
        }



        /// <summary>
        /// Añade una nueva entidad del tipo medio de contacto
        /// </summary>
        /// <param name="entidad"></param>
        /// <returns></returns>
        [HttpPost()]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<MedioContacto>> Post([FromBody] MedioContacto entidad)
        {
            entidad = await servicioEntidad.CrearAsync(entidad).ConfigureAwait(false);
            return Ok(CreatedAtAction("GetMedioContacto", new { id = entidad.Id }, entidad).Value);
        }


        /// <summary>
        /// Actualoza uan entidad medio de contacto, el Id debe incluirse en el querystring así como en 
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
        public async Task<IActionResult> Put(string id, [FromBody]MedioContacto entidad)
        {

            if (id != entidad.Id)
            {
                return BadRequest();
            }

            await servicioEntidad.ActualizarAsync(entidad).ConfigureAwait(false);
            return NoContent();

        }


        /// <summary>
        /// Devulve una lista de medio de contacto asociadas al objeto del tipo especificado
        /// </summary>
        /// <param name="query">Consulta para la paginación y búsqueda</param>
        /// <returns></returns>
        [HttpGet("page/{tipo}/{id}", Name = "GetPageMEdioContacto")]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<Paginado<MedioContacto>>> GetPage(
            string tipo, string id,
            [ModelBinder(typeof(GenericDataPageModelBinder))][FromQuery]Consulta query = null)
        {

            ///Añade las propiedaes del contexto para el filtro de ACL vía ACL Controller
            //query.Filtros.AddRange(ObtieneFiltrosIdentidad());

            query.Filtros.Add(new FiltroConsulta() { Propiedad = IEntidadRelacionada.CampoTipo, Operador = FiltroConsulta.OP_EQ, Valor = tipo });
            query.Filtros.Add(new FiltroConsulta() { Propiedad = IEntidadRelacionada.CampoId, Operador = FiltroConsulta.OP_EQ, Valor = id });


            var data = await servicioEntidad.ObtenerPaginadoAsync(
                Query: query,
                include: null)
                .ConfigureAwait(false);

            return Ok(data);
        }



        /// <summary>
        /// Obtiene un medio de contacto en base al Id único
        /// </summary>
        /// <param name="id">Id único del país</param>
        /// <returns></returns>
        [HttpGet("{id}")]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<MedioContacto>> Get(string id)
        {
            var o = await servicioEntidad.UnicoAsync(
                predicado: x => x.Id == id)
                .ConfigureAwait(false);

            if (o != null) return Ok(o);
            return NotFound(id);
        }




        /// <summary>
        /// Elimina de manera permanente un medio de contacto en base al arreglo de identificadores recibidos
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
            return Ok(await servicioEntidad.Eliminar(lids).ConfigureAwait(false));
        }



    }
}