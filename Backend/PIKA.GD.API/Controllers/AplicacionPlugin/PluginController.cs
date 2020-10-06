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
using PIKA.Modelo.Aplicacion.Plugins;
using PIKA.Modelo.Metadatos;
using PIKA.Servicio.AplicacionPlugin.Interfaces;
using RepositorioEntidades;


namespace PIKA.GD.API.Controllers.AplicacionPlugin
{
    [Authorize]
    [ApiVersion("1.0")]
    [ApiController]
    [Route("api/v{version:apiVersion}/Apli/[controller]")]
    public class PluginController : ACLController
    {
        private ILogger<PluginController> logger;
        private IServicioPlugin servicioPlugin;
        private IProveedorMetadatos<Plugin> metadataProvider;
        private IServicioVersionPlugin servicioVersionPlugin;
        public PluginController(ILogger<PluginController> logger,
            IProveedorMetadatos<Plugin> metadataProvider,
            IServicioPlugin servicioPlugin)
        {
            this.logger = logger;
            this.servicioPlugin = servicioPlugin;
            this.metadataProvider = metadataProvider;
        }

        [HttpGet("metadata", Name = "MetadataPlugin")]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        public async Task<ActionResult<MetadataInfo>> GetMetadata([FromQuery]Consulta query = null)
        {
            return Ok(await metadataProvider.Obtener().ConfigureAwait(false));
        }



        [HttpPost]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        public async Task<ActionResult<Plugin>> Post([FromBody]Plugin entidad)
        {
            entidad = await servicioPlugin.CrearAsync(entidad).ConfigureAwait(false);
            if (entidad.versionPlugins != null)
            {}

            return Ok(CreatedAtAction("GetPlugin", new { id = entidad.Id }, entidad).Value);

        }
      
        [HttpPut("{id}")]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        public async Task<IActionResult> Put(string id, [FromBody]Plugin entidad)
        {
            var x = ObtieneFiltrosIdentidad();


            if (id != entidad.Id)
            {
                return BadRequest();
            }

            await servicioPlugin.ActualizarAsync(entidad).ConfigureAwait(false);
            return NoContent();

        }


        [HttpGet("page", Name = "GetPagePlugin")]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        public async Task<ActionResult<IEnumerable<Plugin>>> GetPage([ModelBinder(typeof(GenericDataPageModelBinder))][FromQuery]Consulta query = null)
        {
            ///Añade las propiedaes del contexto para el filtro de ACL vía ACL Controller
            var data = await servicioPlugin.ObtenerPaginadoAsync(
                Query: query,
                include: null)
                .ConfigureAwait(false);

            return Ok(data);
        }


        [HttpGet("{id}")]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        public async Task<ActionResult<Plugin>> Get(string id)
        {
            var o = await servicioPlugin.UnicoAsync(x => x.Id == id).ConfigureAwait(false);
            if (o != null) return Ok(o);
            return NotFound(id);
        }

        [HttpDelete("{ids}")]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        public async Task<ActionResult> Delete(string id)
        {
            string IdsTrim = "";
            foreach (string item in id.Split(',').ToList().Where(x => !string.IsNullOrEmpty(x)).ToArray())
            {
                IdsTrim += item.Trim() + ",";
            }
            string[] lids = IdsTrim.Split(',').ToList()
           .Where(x => !string.IsNullOrEmpty(x)).ToArray();
            return Ok(await servicioPlugin.Eliminar(lids).ConfigureAwait(false));

        }

        /// <summary>
        /// Este metodo  puerga todos los elementos
        /// </summary>
        /// <returns></returns>
        [HttpDelete("purgar", Name = "PurgarPlugin")]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<Plugin>> DeletePurgar()
        {
            return Ok(await servicioPlugin.Purgar().ConfigureAwait(false));
        }
    }
}