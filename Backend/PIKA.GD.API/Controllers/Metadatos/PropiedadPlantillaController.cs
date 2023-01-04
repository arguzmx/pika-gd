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
using PIKA.Modelo.Metadatos;
using PIKA.Servicio.Metadatos.Interfaces;
using RepositorioEntidades;


namespace PIKA.GD.API.Controllers.Metadatos
{
    [Authorize]
    [ApiVersion("1.0")]
    [ApiController]
    [Route("api/v{version:apiVersion}/metadatos/[controller]")]
    public class PropiedadPlantillaController : ACLController
    {

        private ILogger<PropiedadPlantillaController> logger;
        private IServicioPropiedadPlantilla servicioEntidad;
        private IProveedorMetadatos<PropiedadPlantilla> metadataProvider;
        private IServicioPlantilla servicioPlantilla;
        private IRepositorioMetadatos repositorioMetadatos;
        public PropiedadPlantillaController(
            IRepositorioMetadatos repositorioMetadatos,
            IServicioPlantilla servicioPlantilla,
            ILogger<PropiedadPlantillaController> logger,
            IProveedorMetadatos<PropiedadPlantilla> metadataProvider,
            IServicioPropiedadPlantilla servicioentidad
            )
        {
            
            this.logger = logger;
            this.servicioEntidad = servicioentidad;
            this.metadataProvider = metadataProvider;
            this.repositorioMetadatos = repositorioMetadatos;
            this.servicioPlantilla = servicioPlantilla;
        }

        public override void EmiteConfiguracionSeguridad(UsuarioAPI usuario, ContextoRegistroActividad RegistroActividad, List<EventoAuditoriaActivo> Eventos)
        {
            servicioEntidad.EstableceContextoSeguridad(usuario, RegistroActividad, Eventos);
            servicioPlantilla.EstableceContextoSeguridad(usuario, RegistroActividad, Eventos);
        }


        /// <summary>
        /// Obtiene los metadatos relacionados con la entidad  PropiedadPlantilla
        /// </summary>
        /// <returns></returns>
        [HttpGet("metadata", Name = "MetadataPropiedadPlantilla")]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<MetadataInfo>> GetMetadata([FromQuery] Consulta query = null)
        {
            return Ok(await metadataProvider.Obtener().ConfigureAwait(false));
        }
        /// <summary>
        /// Añade una nueva entidad Propiedad Plantilla
        /// </summary>
        /// <param name="entidad"></param>
        /// <returns></returns>


        [HttpPost]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<PropiedadPlantilla>> Post([FromBody] PropiedadPlantilla entidad)
        {
            entidad = await servicioEntidad.CrearAsync(entidad).ConfigureAwait(false);
            Plantilla p = await servicioPlantilla.UnicoAsync(x => x.Id == entidad.PlantillaId, null, x => x.Include(y => y.Propiedades)).ConfigureAwait(false);
            await repositorioMetadatos.ActualizaDesdePlantilla(p).ConfigureAwait(false);
            return Ok(CreatedAtAction("GetPropiedadPlantilla", new { id = entidad.Id.Trim() },  entidad ).Value);
        }


        /// <summary>
        /// Actualiza una entidad Propiedad Plantilla, el Id debe incluirse en el Querystring así como en 
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
        public async Task<IActionResult> Put(string id, [FromBody] PropiedadPlantilla entidad)
        {
            var x = ObtieneFiltrosIdentidad();


            if (id.Trim() != entidad.Id.Trim())
            {
                return BadRequest();
            }

            await servicioEntidad.ActualizarAsync(entidad).ConfigureAwait(false);
            return NoContent();

        }
        /// <summary>
        /// Devulve un alista de Propiedad Plantilla asociadas al objeto del tipo especificado
        /// </summary>
        /// <param name="query">Consulta para la paginación y búsqueda</param>
        /// <returns></returns>

        [HttpGet("page/plantilla/{plantillaid}", Name = "GetPagePropiedadPlantilla")]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<Paginado<PropiedadPlantilla>>> GetPage(string plantillaid,
            [ModelBinder(typeof(GenericDataPageModelBinder))][FromQuery] Consulta query = null)
        {
                query.Filtros.Add(new FiltroConsulta() { Propiedad = "PlantillaId", Operador = FiltroConsulta.OP_EQ, Valor = plantillaid });
                var data = await servicioEntidad.ObtenerPaginadoAsync(
                    Query: query,
                    include: null)
                    .ConfigureAwait(false);

                return Ok(data);
        }
        /// <summary>
        /// Obtiene un Propiedad Plantilla en base al Id único
        /// </summary>
        /// <param name="id">Id único del Propiedad Plantilla</param>
        /// <returns></returns>

        [HttpGet("{id}")]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<PropiedadPlantilla>> Get(string id)
        {
            var o = await servicioEntidad.UnicoAsync(x => x.Id == id.Trim()).ConfigureAwait(false);
            if (o != null) return Ok(o);
            return NotFound(id);
        }


        /// <summary>
        /// Elimina de manera permanente un Propiedad Plantilla en base al arreglo de identificadores recibidos
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
            return Ok(await servicioEntidad.Eliminar(lids).ConfigureAwait(false));
        }


        /// <summary>
        /// Restaura una lista dede Propiedad Plantilla eliminados en base al arreglo de identificadores recibidos
        /// </summary>
        /// <param name="ids">Arreglo de identificadores string</param>
        /// <returns></returns>
        [HttpPatch("restaurar/{ids}", Name = "restaurarPropiedadPlantilla")]
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

            return Ok(await servicioEntidad.Restaurar(lids).ConfigureAwait(false));
        }

    }
}