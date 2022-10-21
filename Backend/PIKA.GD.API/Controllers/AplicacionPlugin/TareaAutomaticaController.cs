using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PIKA.Constantes.Aplicaciones.Aplicaciones;
using PIKA.GD.API.Filters;
using PIKA.GD.API.Model;
using PIKA.Infraestructura.Comun;
using PIKA.Infraestructura.Comun.Seguridad;
using PIKA.Modelo.Aplicacion.Tareas;
using PIKA.Modelo.Metadatos;
using PIKA.Servicio.AplicacionPlugin.Interfaces;
using RepositorioEntidades;


namespace PIKA.GD.API.Controllers.AplicacionTareaAutomatica
{
    [Authorize]
    [ApiVersion("1.0")]
    [ApiController]
    [Route("api/v{version:apiVersion}/Apli/[controller]")]
    public class TareaAutomaticaController : ACLController
    {
        private ILogger<TareaAutomaticaController> logger;
        private IServicioTareaAutomatica servicioTareaAutomatica;
        private IProveedorMetadatos<TareaAutomatica> metadataProvider;
        private readonly IServicioTokenSeguridad ServicioTokenSeguridad;
        public TareaAutomaticaController(
            ILogger<TareaAutomaticaController> logger,
            IProveedorMetadatos<TareaAutomatica> metadataProvider,
            IServicioCache servicioCache,
            IServicioTareaAutomatica servicioTareaAutomatica,
            IServicioTokenSeguridad ServicioTokenSeguridad)
        {
            this.logger = logger;
            this.servicioTareaAutomatica = servicioTareaAutomatica;
            this.metadataProvider = metadataProvider;
            this.ServicioTokenSeguridad = ServicioTokenSeguridad;
        }

        [HttpGet("metadata", Name = "MetadataTareaAutomatica")]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        public async Task<ActionResult<MetadataInfo>> GetMetadata([FromQuery]Consulta query = null)
        {
            try
            {
                return Ok(await metadataProvider.Obtener().ConfigureAwait(false));
            }
            catch (Exception ex)
            {
                throw;
            }
            
        }



        [HttpPost]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        public async Task<ActionResult<TareaAutomatica>> Post([FromBody]TareaAutomatica entidad)
        {

            var permiso = await ServicioTokenSeguridad.PermisosModuloId(this.UsuarioId, this.DominioId, ConstantesAppAplicacionPlugin.MODULO_APLICACIONES)
            .ConfigureAwait(false);
            
            servicioTareaAutomatica.permisos = permiso;
            servicioTareaAutomatica.usuario = this.usuario;
         
            entidad = await servicioTareaAutomatica.CrearAsync(entidad).ConfigureAwait(false);
            return Ok(CreatedAtAction("GetTareaAutomatica", new { id = entidad.Id }, entidad).Value);

        }
      
        [HttpPut("{id}")]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        public async Task<IActionResult> Put(string id, [FromBody]TareaAutomatica entidad)
        {
            var permiso = await ServicioTokenSeguridad.PermisosModuloId(this.UsuarioId, this.DominioId, ConstantesAppAplicacionPlugin.MODULO_APLICACIONES)
         .ConfigureAwait(false);

            servicioTareaAutomatica.permisos = permiso;
            servicioTareaAutomatica.usuario = this.usuario;


            var x = ObtieneFiltrosIdentidad();


            if (id != entidad.Id)
            {
                return BadRequest();
            }

            await servicioTareaAutomatica.ActualizarAsync(entidad).ConfigureAwait(false);
            return NoContent();

        }


        [HttpGet("page", Name = "GetPageTareaAutomatica")]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        public async Task<ActionResult<Paginado<TareaAutomatica>>> GetPage([ModelBinder(typeof(GenericDataPageModelBinder))][FromQuery]Consulta query = null)
        {
            var permiso = await ServicioTokenSeguridad.PermisosModuloId(this.UsuarioId, this.DominioId, ConstantesAppAplicacionPlugin.MODULO_APLICACIONES)
         .ConfigureAwait(false);

            servicioTareaAutomatica.permisos = permiso;
            servicioTareaAutomatica.usuario = this.usuario;


            query.Filtros.Add(FiltroDominio(nameof(TareaAutomatica.OrigenId)));

            ///Añade las propiedaes del contexto para el filtro de ACL vía ACL Controller
            var data = await servicioTareaAutomatica.ObtenerPaginadoAsync(
                Query: query,
                include: null)
                .ConfigureAwait(false);

            return Ok(data);
        }


        [HttpGet("{id}")]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        public async Task<ActionResult<TareaAutomatica>> Get(string id)
        {
            var permiso = await ServicioTokenSeguridad.PermisosModuloId(this.UsuarioId, this.DominioId, ConstantesAppAplicacionPlugin.MODULO_APLICACIONES)
         .ConfigureAwait(false);

            servicioTareaAutomatica.permisos = permiso;
            servicioTareaAutomatica.usuario = this.usuario;


            var o = await servicioTareaAutomatica.UnicoAsync(x => x.Id == id).ConfigureAwait(false);
            if (o != null) return Ok(o);
            return NotFound(id);
        }

    }
}