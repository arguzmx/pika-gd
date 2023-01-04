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
    public class AmpliacionController : ACLController
    {
        private readonly ILogger<AmpliacionController> logger;
        private IServicioAmpliacion servicioAmpliacion;
        private IProveedorMetadatos<Ampliacion> metadataProvider;
        public AmpliacionController(ILogger<AmpliacionController> logger,
            IProveedorMetadatos<Ampliacion> metadataProvider,
            IServicioAmpliacion servicioAmpliacion)
        {
            this.logger = logger;
            this.servicioAmpliacion = servicioAmpliacion;
            this.metadataProvider = metadataProvider;
        }
        public override void EmiteConfiguracionSeguridad(UsuarioAPI usuario, ContextoRegistroActividad RegistroActividad, List<EventoAuditoriaActivo> Eventos)
        {
            servicioAmpliacion.EstableceContextoSeguridad(usuario, RegistroActividad, Eventos);
        }

        /// <summary>
        /// Obtiene los metadatos relacionados con la entidad  Ampliaciòn
        /// </summary>
        /// <returns></returns>
        [HttpGet("metadata", Name = "MetadataAmpliacion")]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        [ProducesResponseType(StatusCodes.Status200OK)]

        public async Task<ActionResult<MetadataInfo>> GetMetadata([FromQuery]Consulta query = null)
        {
            return Ok(await metadataProvider.Obtener().ConfigureAwait(false));
        }
        /// <summary>
        /// Añade una nueva entidad del  Ampliaciòn
        /// </summary>
        /// <param name="entidad"></param>
        /// <returns></returns>
        [HttpPost]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        [ProducesResponseType(StatusCodes.Status200OK)]

        public async Task<ActionResult<Ampliacion>> Post([FromBody]Ampliacion entidad)
        {
            entidad = await servicioAmpliacion.CrearAsync(entidad).ConfigureAwait(false);
            return Ok(CreatedAtAction("GetAmpliacion", new { id = entidad.ActivoId, vigente = entidad.Vigente }, entidad).Value);
        }

        /// <summary>
        /// Actualiza una entidad  Ampliaciòn, el Id debe incluirse en el Querystring así como en 
        /// el serializado para la petición PUT
        /// </summary>
        /// <param name="id">Identificador único del  Ampliaciòn</param>
        /// <param name="entidad">Datos serialziados de la OU</param>
        /// <returns></returns>

        [HttpPut("{id}")]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> Put(string id, [FromBody]Ampliacion entidad)
        {
            var x = ObtieneFiltrosIdentidad();


            if (id.Trim() != entidad.Id.Trim())
            {
                return BadRequest();
            }

            await servicioAmpliacion.ActualizarAsync(entidad).ConfigureAwait(false);
            return NoContent();

        }
        /// <summary>
        /// Devulve un analista de  Ampliaciòn asociadas al objeto del tipo especificado
        /// </summary>
        /// <param name="query">Consulta para la paginación y búsqueda</param>
        /// <returns></returns>
        [HttpGet("page/activo/{id}", Name = "GetPageAmpliacion")]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        [ProducesResponseType(StatusCodes.Status200OK)]

        public async Task<ActionResult<IEnumerable<Ampliacion>>> GetPage(string Id, [ModelBinder(typeof(GenericDataPageModelBinder))][FromQuery]Consulta query = null)
        {

            query.Filtros.Add(new FiltroConsulta()
            {
                Operador = FiltroConsulta.OP_EQ,
                Valor = Id,
                Propiedad = "ActivoId"
            });

            var data = await servicioAmpliacion.ObtenerPaginadoAsync(
                     Query: query,
                     include: null)
                     .ConfigureAwait(false);

            return Ok(data);
        }
        /// <summary>
        /// Obtiene un Ampliaciòn en base al Id único
        /// </summary>
        /// <param name="id">Id único del Ampliaciòn</param>
        /// <returns></returns>

        [HttpGet("{id}")]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        [ProducesResponseType(StatusCodes.Status200OK)]

        public async Task<ActionResult<Ampliacion>> Get(string id)
        {
            var o = await servicioAmpliacion.UnicoAsync(x => x.Id == id.Trim()).ConfigureAwait(false);
            if (o != null) return Ok(o);
            return NotFound(id);
        }

        /// <summary>
        /// Elimina de manera permanente un Ampliacion en base al arreglo de identificadores recibidos
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

            return Ok(await servicioAmpliacion.Eliminar(Ids).ConfigureAwait(false));
        }

    }
}