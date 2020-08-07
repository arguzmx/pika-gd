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
using PIKA.Modelo.GestorDocumental;
using PIKA.Modelo.Metadatos;
using PIKA.Servicio.GestionDocumental.Interfaces;
using RepositorioEntidades;


namespace PIKA.GD.API.Controllers.GestorDocumental
{
    [Authorize]
    [ApiVersion("1.0")]
    [ApiController]
    [Route("api/v{version:apiVersion}/gd/[controller]")]
    public class TipoAmpliacionController : ACLController
    {
        private readonly ILogger<TipoAmpliacionController> logger;
        private IServicioTipoAmpliacion servicioTipoAmpliacion;
        private IProveedorMetadatos<TipoAmpliacion> metadataProvider;
        public TipoAmpliacionController(ILogger<TipoAmpliacionController> logger,
            IProveedorMetadatos<TipoAmpliacion> metadataProvider,
            IServicioTipoAmpliacion servicioTipoAmpliacion)
        {
            this.logger = logger;
            this.servicioTipoAmpliacion = servicioTipoAmpliacion;
            this.metadataProvider = metadataProvider;
        }

        /// <summary>
        /// Obtiene los metadatos relacionados con la entidad Tipo Ampliaciòn
        /// </summary>
        /// <returns></returns>
        [HttpGet("metadata", Name = "MetadataTipoAmpliacion")]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        [ProducesResponseType(StatusCodes.Status200OK)]

        public async Task<ActionResult<MetadataInfo>> GetMetadata([FromQuery]Consulta query = null)
        {
            return Ok(await metadataProvider.Obtener().ConfigureAwait(false));
        }
        /// <summary>
        /// Añade una nueva entidad del tipo Ampliaciòn
        /// </summary>
        /// <param name="entidad"></param>
        /// <returns></returns>

        [HttpPost]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        [ProducesResponseType(StatusCodes.Status200OK)]

        public async Task<ActionResult<TipoAmpliacion>> Post([FromBody]TipoAmpliacion entidad)
        {
            entidad = await servicioTipoAmpliacion.CrearAsync(entidad).ConfigureAwait(false);
            return Ok(CreatedAtAction("GetTipoAmpliacion", new { id = entidad.Id.Trim() }, entidad).Value);
        }
        /// <summary>
        /// Actualiza una entidad Tipo Ampliaciòn, el Id debe incluirse en el Querystring así como en 
        /// el serializado para la petición PUT
        /// </summary>
        /// <param name="id">Identificador único del Tipo Ampliaciòn</param>
        /// <param name="entidad">Datos serialziados de la OU</param>
        /// <returns></returns>

        [HttpPut("{id}")]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> Put(string id, [FromBody]TipoAmpliacion entidad)
        {
            var x = ObtieneFiltrosIdentidad();


            if (id.Trim() != entidad.Id.Trim())
            {
                return BadRequest();
            }

            await servicioTipoAmpliacion.ActualizarAsync(entidad).ConfigureAwait(false);
            return NoContent();

        }
        /// <summary>
        /// Devulve un analista de Tipo Ampliaciòn asociadas al objeto del tipo especificado
        /// </summary>
        /// <param name="query">Consulta para la paginación y búsqueda</param>
        /// <returns></returns>


        [HttpGet("page", Name = "GetPageTipoAmpliacion")]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        [ProducesResponseType(StatusCodes.Status200OK)]

        public async Task<ActionResult<IEnumerable<TipoAmpliacion>>> GetPage([ModelBinder(typeof(GenericDataPageModelBinder))][FromQuery]Consulta query = null)
        {
            ///Añade las propiedaes del contexto para el filtro de ACL vía ACL Controller
            query.Filtros.AddRange(ObtieneFiltrosIdentidad());
            var data = await servicioTipoAmpliacion.ObtenerPaginadoAsync(query).ConfigureAwait(false);
            return Ok(data.Elementos.ToList<TipoAmpliacion>());
        }
        /// <summary>
        /// Obtiene un Tipo Ampliaciòn en base al Id único
        /// </summary>
        /// <param name="id">Id único del Tipo Ampliaciòn</param>
        /// <returns></returns>
        [HttpGet("{id}")]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        [ProducesResponseType(StatusCodes.Status200OK)]

        public async Task<ActionResult<TipoAmpliacion>> Get(string id)
        {
            var o = await servicioTipoAmpliacion.UnicoAsync(x => x.Id == id.Trim()).ConfigureAwait(false);
            if (o != null) return Ok(o);
            return NotFound(id);
        }
        /// <summary>
        /// Elimina de manera permanente un Tipo Ampliacion en base al arreglo de identificadores recibidos
        /// </summary>
        /// <param name="ids">Arreglo de identificadores string</param>
        /// <returns></returns>
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
            string[] Ids = IdsTrim.Split(',').ToList().Where(x => !string.IsNullOrEmpty(x)).ToArray();

            return Ok(await servicioTipoAmpliacion.Eliminar(Ids).ConfigureAwait(false));
        }

        /// <summary>
        /// Obtiene una lista de Tipos de Ampliación en base a los parámetros de consulta
        /// </summary>
        /// <param name="query">Query de busqueda a la base de datos</param>
        /// <returns></returns>

        [HttpGet("pares", Name = "GetParesTipoAmpliacion")]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<List<ValorListaOrdenada>>> GetPares(
        [ModelBinder(typeof(GenericDataPageModelBinder))][FromQuery] Consulta query = null)
        {
            var data = await servicioTipoAmpliacion.ObtenerParesAsync(query)
                .ConfigureAwait(false);

            return Ok(data);
        }
        /// <summary>
        /// Obtiene una lista de Tipos de Ampliación en base a con el parámetro ID de consulta
        /// </summary>
        /// <param name="ids">parametro Id para consulta a la base de datos</param>
        /// <returns></returns>

        [HttpGet("pares/{ids}", Name = "GetParesTipoAmpliacionId")]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<List<ValorListaOrdenada>>> GetParesporId(
              string ids)
        {
            string[] ArregloId = ids.Split(',').ToArray();
            List<string> lids = new List<string>();
            foreach (string i in ArregloId)
            {
                lids.Add(i.Trim());
            }
            lids.Where(x => !string.IsNullOrEmpty(x.Trim())).ToList();
            var data = await servicioTipoAmpliacion.ObtenerParesPorId(lids)
                .ConfigureAwait(false);

            return Ok(data);
        }

    }

}