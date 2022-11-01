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
using PIKA.Modelo.Contenido.Request;
using PIKA.Modelo.Metadatos;
using PIKA.Servicio.Contenido.Interfaces;
using RepositorioEntidades;

namespace PIKA.GD.API.Controllers.Contenido
{
    [Authorize]
    [ApiVersion("1.0")]
    [ApiController]
    [Route("api/v{version:apiVersion}/contenido/[controller]")]
    public class CarpetaController : ACLController
    {

        private ILogger<CarpetaController> logger;
        private IServicioCarpeta servicioEntidad;
        private IProveedorMetadatos<Carpeta> metadataProvider;
        public CarpetaController(ILogger<CarpetaController> logger,
            IProveedorMetadatos<Carpeta> metadataProvider,
            IServicioCarpeta servicioEntidad)
        {
            this.logger = logger;
            this.servicioEntidad = servicioEntidad;
            this.metadataProvider = metadataProvider;
        }


    
        /// <summary>
        /// Otiene los metadatos asociados a la carpeta
        /// </summary>
        /// <returns></returns>
        [HttpGet("metadata", Name = "MetadataCarpetaContenid")]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<MetadataInfo>> GetMetadata()
        {
            return Ok(await metadataProvider.Obtener().ConfigureAwait(false));
        }


        /// <summary>
        ///  Añade una nueva carpeta de contenido
        /// </summary>
        /// <param name="entidad"></param>
        /// <returns></returns>
        [HttpPost]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<Carpeta>> Post([FromBody]Carpeta entidad)
        {
            entidad.CreadorId = this.UsuarioId;
            entidad = await servicioEntidad.CrearAsync(entidad).ConfigureAwait(false);
            return Ok(CreatedAtAction("GetCarpeta", new { id = entidad.Id }, entidad).Value);
        }



        [HttpPost("ruta")]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<Carpeta>> PostRuta([FromBody] CarpetaDeRuta entidad)
        {
            entidad.UsuarioId = this.UsuarioId;
            Carpeta c = await servicioEntidad.ObtenerCrearPorRuta(entidad).ConfigureAwait(false);
            return Ok(c);
        }


        /// <summary>
        /// Actualiza una entidad Carpeta existente, el Id debe incluirse en el Querystring así como en 
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
        public async Task<IActionResult> Put(string id, [FromBody]Carpeta entidad)
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
        /// Otiene una página de resultados de Carpetas en base a la configuración de paginado u al query recibido 
        /// </summary>
        /// <param name="query">Query de filtrado para el paginado</param>
        /// <returns></returns>
        [HttpGet("page", Name = "GetPageCarpeta")]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<Paginado<Carpeta>>> GetPage([ModelBinder(typeof(GenericDataPageModelBinder))][FromQuery]Consulta query = null)
        {
            ///Añade las propiedaes del contexto para el filtro de ACL vía ACL Controller
            query.Filtros.AddRange(ObtieneFiltrosIdentidad());
            var data = await servicioEntidad.ObtenerPaginadoAsync(query).ConfigureAwait(false);
            return Ok(data);
        }


        /// <summary>
        /// Obtiene una carpeta en base al Id único
        /// </summary>
        /// <param name="id">Id único del país</param>
        /// <returns></returns>
        [HttpGet("{id}")]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<Carpeta>> Get(string id)
        {
            var o = await servicioEntidad.UnicoAsync(x => x.Id == id).ConfigureAwait(false);
            if (o != null) return Ok(o);
            return NotFound(id);
        }


        /// <summary>
        /// Elimina de manera lógica una carpeta en base al arreglo de identificadores recibidos
        /// </summary>
        /// <param name="ids">Lista de identificadores separados por omas</param>
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
        /// Este metodo  puerga todos los elementos
        /// </summary>
        /// <returns></returns>
        [HttpDelete("purgar", Name = "DeletePurgarCarpeta")]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<Carpeta>> DeletePurgar()
        {
            return Ok(await servicioEntidad.Purgar().ConfigureAwait(false));
        }

        /// <summary>
        /// Restaura una lista de carpetaes eliminadas en base al arreglo de identificadores recibidos
        /// </summary>
        /// <param name="ids">Arreglo de identificadores string</param>
        /// <returns></returns>
        [HttpPatch("restaurar/{ids}", Name = "RestaurarCarpeta")]
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


        /// <summary>
        /// Obtiene los nodos raíz de un punto de montaje
        /// </summary>
        /// <param name="puntoontaje">identificadro único del punto de montaje</param>
        /// <param name="n">numero de niveles recursivos</param>
        /// <returns></returns>
        [HttpGet("jerarquia/raices/{puntoontaje}/{n}", Name = "CarpetaRaices")]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<List<NodoJerarquico>>> Descendientes(string puntoontaje, int n = 0)
        {

            return Ok(await servicioEntidad.ObtenerRaices(puntoontaje, n).ConfigureAwait(false));
        }


        /// <summary>
        /// Obtiene los descendientes de una carpeta
        /// </summary>
        /// <param name="puntoontaje">identificadro único del punto de montaje</param>
        /// <param name="id">identificador único del elemento padre</param>
        /// <param name="n">numero de niveles recursivos</param>
        /// <returns></returns>
        [HttpGet("jerarquia/hijos/{puntoontaje}/{id}/{n}", Name = "CarpetaDescendientes")]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<List<NodoJerarquico>>> Descendientes(string puntoontaje, string id, int n = 0)
        {

            return Ok(await  servicioEntidad.ObtenerDescendientes(puntoontaje, id, n).ConfigureAwait(false));
        }


        [HttpGet("jerarquia/{jerarquiaid}/{padreid}", Name = "ObtenerHijosAsyncCarpeta")]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<List<Carpeta>>> ObtenerHijosAsync(string padreid, string jerarquiaid)
        {
            var l = await servicioEntidad.ObtenerHijosAsync(padreid, jerarquiaid).ConfigureAwait(false);
            return Ok(l.ToList());
        }


        [HttpGet("jerarquia/{jerarquiaid}", Name = "ObtenerRaicesAsyncCarpeta")]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<List<Carpeta>>> ObtenerRaicesAsync(string jerarquiaid)
        {
            var l = await servicioEntidad.ObtenerRaicesAsync(jerarquiaid).ConfigureAwait(false);
            return Ok(l.ToList());
        }

       

    }
}