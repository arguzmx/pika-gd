using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
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
    public class PluginInstaladoController : ACLController
    {
        private ILogger<PluginInstaladoController> logger;
        private IServicioPluginInstalado servicioPluginInstalado;
        private IProveedorMetadatos<PluginInstalado> metadataProvider;
        public PluginInstaladoController(ILogger<PluginInstaladoController> logger,
            IProveedorMetadatos<PluginInstalado> metadataProvider,
            IServicioPluginInstalado servicioPluginInstalado)
        {
            this.logger = logger;
            this.servicioPluginInstalado = servicioPluginInstalado;
            this.metadataProvider = metadataProvider;
        }

        [HttpGet("metadata", Name = "MetadataPluginInstalado")]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        public async Task<ActionResult<MetadataInfo>> GetMetadata([FromQuery]Consulta query = null)
        {
            return Ok(await metadataProvider.Obtener().ConfigureAwait(false));
        }



        [HttpPost]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        public async Task<ActionResult<PluginInstalado>> Post([FromBody]PluginInstalado entidad)
        {
           

            entidad = await servicioPluginInstalado.CrearAsync(entidad).ConfigureAwait(false);
  
            return Ok(CreatedAtAction("GetPluginInstalado", new { id = entidad.PLuginId }, entidad).Value);
        }



        [HttpPut("{id}")]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        public async Task<IActionResult> Put(string id, [FromBody]PluginInstalado entidad)
        {
                    var x = ObtieneFiltrosIdentidad();
         

            if (id != entidad.VersionPLuginId)
            {
                return BadRequest();
            }

            await servicioPluginInstalado.ActualizarAsync(entidad).ConfigureAwait(false);
            
            return NoContent();

        }


        [HttpGet("page", Name = "GetPagePluginInstalado")]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        public async Task<ActionResult<IEnumerable<PluginInstalado>>> GetPage([ModelBinder(typeof(GenericDataPageModelBinder))][FromQuery]Consulta query = null)
        {
            var data = await servicioPluginInstalado.ObtenerPaginadoAsync(
               Query: query,
               include: null)
               .ConfigureAwait(false);

            return Ok(data);

        }


        [HttpGet("{id}")]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        public async Task<ActionResult<PluginInstalado>> Get(string id,string idVersion)
        {
            var o = await servicioPluginInstalado.UnicoAsync(x => x.VersionPLuginId == id ).ConfigureAwait(false);
            if (o != null) return Ok(o);
            return NotFound(id);
        }

        [HttpDelete]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        public async Task<ActionResult> Delete([FromBody]string[] id)
        {
            return Ok(await servicioPluginInstalado.Eliminar(id).ConfigureAwait(false));
        }

    }
}