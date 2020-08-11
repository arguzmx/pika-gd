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
    public class ActivoController : ACLController
    {
        private readonly ILogger<ActivoController> logger;
        private IServicioActivo servicioActivo;
        private IProveedorMetadatos<Activo> metadataProvider;
        public ActivoController(ILogger<ActivoController> logger,
            IProveedorMetadatos<Activo> metadataProvider,
            IServicioActivo servicioActivo)
        {
            this.logger = logger;
            this.servicioActivo = servicioActivo;
            this.metadataProvider = metadataProvider;
        }


        /// <summary>
        /// Obtiene los metadatos relacionados con la entidad Activo
        /// </summary>
        /// <returns></returns>
        [HttpGet("metadata", Name = "MetadataActivo")]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        [ProducesResponseType(StatusCodes.Status200OK)]

        public async Task<ActionResult<MetadataInfo>> GetMetadata([FromQuery]Consulta query = null)
        {
            return Ok(await metadataProvider.Obtener().ConfigureAwait(false));
        }


        /// <summary>
        /// Añade una nueva entidad del tipo Activo
        /// </summary>
        /// <param name="entidad"></param>
        /// <returns></returns>

        [HttpPost]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        [ProducesResponseType(StatusCodes.Status200OK)]

        public async Task<ActionResult<Activo>> Post([FromBody]Activo entidad)
        {
            entidad = await servicioActivo.CrearAsync(entidad).ConfigureAwait(false);
            return Ok(CreatedAtAction("GetActivo", new { id = entidad.Id }, entidad).Value);
        }


        /// <summary>
        /// Actualiza una entidad Activo, el Id debe incluirse en el Querystring así como en 
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
        public async Task<IActionResult> Put(string id, [FromBody]Activo entidad)
        {
            var x = ObtieneFiltrosIdentidad();


            if (id.Trim() != entidad.Id.Trim())
            {
                return BadRequest();
            }

            await servicioActivo.ActualizarAsync(entidad).ConfigureAwait(false);
            return NoContent();

        }

        /// <summary>
        /// Devulve un alista de Activo asociadas al objeto del tipo especificado
        /// </summary>
        /// <param name="query">Consulta para la paginación y búsqueda</param>
        /// <returns></returns>

        [HttpGet("page", Name = "GetPageActivo")]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        public async Task<ActionResult<IEnumerable<Activo>>> GetPage([ModelBinder(typeof(GenericDataPageModelBinder))][FromQuery]Consulta query = null)
        {
            ///Añade las propiedaes del contexto para el filtro de ACL vía ACL Controller
            query.Filtros.AddRange(ObtieneFiltrosIdentidad());
            var data = await servicioActivo.ObtenerPaginadoAsync(query).ConfigureAwait(false);
            return Ok(data.Elementos.ToList<Activo>());
        }


        //----------------------------------------------------------------------------------------------------------
        //----------------------------------------------------------------------------------------------------------
        //----------------------------------------------------------------------------------------------------------
        //----------------------------------------------------------------------------------------------------------

        /// <summary>
        /// Obtiene un Activo en base al Id único
        /// </summary>
        /// <param name="id">Id único del Activo</param>
        /// <returns></returns>

        [HttpGet("{id}")]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<Activo>> Get(string id)
        {
            var o = await servicioActivo.UnicoAsync(x => x.Id == id.Trim()).ConfigureAwait(false);
            if (o != null) return Ok(o);
            return NotFound(id);
        }



        /// <summary>
        /// Elimina de manera permanente un Activo en base al arreglo de identificadores recibidos
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
            string[] lids = IdsTrim.Split(',').ToList()
           .Where(x => !string.IsNullOrEmpty(x)).ToArray();
            return Ok(await servicioActivo.Eliminar(lids).ConfigureAwait(false));
        }


        /// <summary>
        /// Restaura una lista dede Activos eliminados en base al arreglo de identificadores recibidos
        /// </summary>
        /// <param name="ids">Arreglo de identificadores string</param>
        /// <returns></returns>
        [HttpPatch("restaurar/{ids}", Name = "restaurarActivo")]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult> Undelete(string ids)
        {

            string IdsTrim = "";
            foreach (string item in ids.Split(',').ToList().Where(x => !string.IsNullOrEmpty(x)).ToArray())
            {
                IdsTrim += item.Trim() + ",";
            }
            string[] lids = IdsTrim.Split(',').ToList()
           .Where(x => !string.IsNullOrEmpty(x)).ToArray();

            return Ok(await servicioActivo.Restaurar(lids).ConfigureAwait(false));
        }

    }
}