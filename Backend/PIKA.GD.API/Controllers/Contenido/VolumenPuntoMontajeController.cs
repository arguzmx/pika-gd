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
using PIKA.Modelo.Contenido;
using PIKA.Modelo.Metadatos;
using PIKA.Servicio.Contenido.Interfaces;
using RepositorioEntidades;

namespace PIKA.GD.API.Controllers.Contenido
{
    [Authorize]
    [ApiVersion("1.0")]
    [ApiController]
    [Route("api/v{version:apiVersion}/contenido/[controller]")]
    public class VolumenPuntoMontajeController : ACLController
    {

        private ILogger<VolumenPuntoMontajeController> logger;
        private IServicioVolumenPuntoMontaje servicioEntidad;
        private IProveedorMetadatos<VolumenPuntoMontaje> metadataProvider;
        public VolumenPuntoMontajeController(ILogger<VolumenPuntoMontajeController> logger,
            IProveedorMetadatos<VolumenPuntoMontaje> metadataProvider,
            IServicioVolumenPuntoMontaje servicioEntidad)
        {
            this.logger = logger;
            this.servicioEntidad = servicioEntidad;
            this.metadataProvider = metadataProvider;
        }


        /// <summary>
        /// Otiene los metadatos asociados la relación de volumne y punto de montaje
        /// </summary>
        /// <returns></returns>
        [HttpGet("metadata", Name = "MetadataVolumenPuntoMontajeContenido")]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<MetadataInfo>> GetMetadata()
        {
            return Ok(await metadataProvider.Obtener().ConfigureAwait(false));
        }


        /// <summary>
        ///  Añade una nueva relación de volumne y punto de montaje
        /// </summary>
        /// <param name="entidad"></param>
        /// <returns></returns>
        [HttpPost]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<VolumenPuntoMontaje>> Post([FromBody]VolumenPuntoMontaje entidad)
        {
            entidad = await servicioEntidad.CrearAsync(entidad).ConfigureAwait(false);
            return Ok(CreatedAtAction("GetVolumenPuntoMontaje", new { id = entidad.VolumenId }, entidad).Value);
        }


        /// <summary>
        /// Otiene una página de resultados relaciones de volumne y puntos de montaje en base a la configuración de paginado u al query recibido 
        /// </summary>
        /// <param name="query">Query de filtrado para el paginado</param>
        /// <returns></returns>
        [HttpGet("page/puntomontaje/{id}", Name = "GetPageVolumenPuntoMontaje")]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<VolumenPuntoMontaje>>> GetPage(string id,
            [ModelBinder(typeof(GenericDataPageModelBinder))][FromQuery]Consulta query = null)
        {
            ///Añade las propiedaes del contexto para el filtro de ACL vía ACL Controller
            query.Filtros.AddRange(ObtieneFiltrosIdentidad());
            query.Filtros.Add(new FiltroConsulta() { Propiedad = "PuntoMontajeId", Operador = FiltroConsulta.OP_EQ, Valor = id });

            var data = await servicioEntidad.ObtenerPaginadoAsync(query).ConfigureAwait(false);
            return Ok(data);
        }


        /// <summary>
        /// Obtiene una relacion volumen punto de montaje en base los identificadores clave
        /// </summary>
        /// <param name="puntomontajeid">Identificador del punto de montaje</param>
        /// <param name="volumenid">Identificador del volumnen asociado</param>
        /// <returns></returns>
        [HttpGet("{puntomontajeid}/{volumenid}")]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<VolumenPuntoMontaje>> Get(string puntomontajeid, string volumenid)
        {
            var o = await servicioEntidad.UnicoAsync(x => x.PuntoMontajeId == puntomontajeid &&
            x.VolumenId == volumenid).ConfigureAwait(false);
            if (o != null) return Ok(o);
            return NotFound(new { puntomontajeid, volumenid });
        }




        /// <summary>
        /// Elimina de manera permanente una relacion de volumen a punto de pontaje en base al arreglo de identificadores recibidos
        /// </summary>
        /// <param name="ids">Lista de identificadores de volumen separados por comas</param>
        /// <returns></returns>
        [HttpDelete("{puntomontajeid}/{ids}")]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult> Delete(string puntomontajeid, string ids)
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

    }
}