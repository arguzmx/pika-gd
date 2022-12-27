using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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

    public class AtributoTablaController : ACLController

    {
        private ILogger<AtributoTablaController> logger;
        private IServicioAtributoTabla servicioAtributoTabla;
        private IProveedorMetadatos<AtributoTabla> metadataProvider;
        public AtributoTablaController(ILogger<AtributoTablaController> logger,
            IProveedorMetadatos<AtributoTabla> metadataProvider,
            IServicioAtributoTabla servicioAtributoTabla)
        {
            this.logger = logger;
            this.servicioAtributoTabla = servicioAtributoTabla;
            this.metadataProvider = metadataProvider;
        }


        public override void EmiteConfiguracionSeguridad(UsuarioAPI usuario, ContextoRegistroActividad RegistroActividad, List<EventoAuditoriaActivo> Eventos)
        {
            servicioAtributoTabla.EstableceContextoSeguridad(usuario, RegistroActividad, Eventos);
        }


        [HttpGet("metadata", Name = "MetadataAtributoTabla")]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        public async Task<ActionResult<MetadataInfo>> GetMetadata([FromQuery]Consulta query = null)
        {
            return Ok(await metadataProvider.Obtener().ConfigureAwait(false));
        }



        [HttpPost]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        public async Task<ActionResult<AtributoTabla>> Post([FromBody]AtributoTabla entidad)
        {

            entidad = await servicioAtributoTabla.CrearAsync(entidad).ConfigureAwait(false);
            return Ok(CreatedAtAction("GetAtributoTabla", new { id = entidad.PropiedadId }, entidad).Value);
        }


        [HttpPut("{id}")]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        public async Task<IActionResult> Put(string id, [FromBody]AtributoTabla entidad)
        {
            var x = ObtieneFiltrosIdentidad();


            if (id != entidad.Id)
            {
                return BadRequest();
            }

            await servicioAtributoTabla.ActualizarAsync(entidad).ConfigureAwait(false);
            return NoContent();

        }


        [HttpGet("page", Name = "GetPageAtributoTabla")]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        public async Task<ActionResult<Paginado<AtributoTabla>>> GetPage([ModelBinder(typeof(GenericDataPageModelBinder))][FromQuery]Consulta query = null)
        {
            ///Añade las propiedaes del contexto para el filtro de ACL vía ACL Controller
            var data = await servicioAtributoTabla.ObtenerPaginadoAsync(
                  Query: query,
                  include: null)
                  .ConfigureAwait(false);

            return Ok(data);
        }



        [HttpGet("{id}")]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        public async Task<ActionResult<AtributoTabla>> Get(string id)
        {
            var o = await servicioAtributoTabla.UnicoAsync(x => x.Id == id).ConfigureAwait(false);
            if (o != null) return Ok(o);
            return NotFound(id);
        }




        [HttpDelete("{id}")]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        public async Task<ActionResult> Delete([FromBody]string id)
        {
            string IdsTrim = "";
            foreach (string item in id.Split(',').ToList().Where(x => !string.IsNullOrEmpty(x)).ToArray())
            {
                IdsTrim += item.Trim() + ",";
            }
            string[] lids = IdsTrim.Split(',').ToList()
           .Where(x => !string.IsNullOrEmpty(x)).ToArray();
            return Ok(await servicioAtributoTabla.Eliminar(lids).ConfigureAwait(false));

        }
    }
}