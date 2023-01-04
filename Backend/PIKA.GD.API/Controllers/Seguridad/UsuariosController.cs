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
using PIKA.Modelo.Metadatos;
using PIKA.Modelo.Seguridad;
using PIKA.Servicio.Seguridad.Interfaces;
using RepositorioEntidades;

namespace PIKA.GD.API.Controllers.Seguridad
{

    [Authorize]
    [ApiVersion("1.0")]
    [ApiController]
    [Route("api/v{version:apiVersion}/seguridad/[controller]")]
    public class UsuariosController : ACLController
    {

        private ILogger<UsuariosController> logger;
        private IServicioUsuarios servicioEntidad;
        private IProveedorMetadatos<PropiedadesUsuario> metadataProvider;
        public UsuariosController(ILogger<UsuariosController> logger,
            IProveedorMetadatos<PropiedadesUsuario> metadataProvider,
            IServicioUsuarios servicioEntidad)
        {
            this.logger = logger;
            this.servicioEntidad = servicioEntidad;
            this.metadataProvider = metadataProvider;
        }

        public override void EmiteConfiguracionSeguridad(UsuarioAPI usuario, ContextoRegistroActividad RegistroActividad, List<EventoAuditoriaActivo> Eventos)
        {
            servicioEntidad.EstableceContextoSeguridad(usuario, RegistroActividad, Eventos);
        }

        /// <summary>
        /// Obtiene los metadatos relacionados con la entidad Dominio
        /// </summary>
        /// <returns></returns>
        [HttpGet("metadata", Name = "MetadatUsuarios")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        public async Task<ActionResult<MetadataInfo>> GetMetadata()
        {
            return Ok(await metadataProvider.Obtener().ConfigureAwait(false));
        }


        [HttpPost("contrasena", Name = "ActualizaContrasena")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        public async Task<ActionResult<MetadataInfo>> ActualizaContrasena([FromBody] ActualizaContrasenaUsuario data)
        {
            int valor = await servicioEntidad.ActualizarContrasena(data.Id, data.Nueva).ConfigureAwait(false);
            return StatusCode(valor);
        }

        /// <summary>
        /// Crea un nuevo usuario
        /// </summary>
        /// <param name="entidad"></param>
        /// <returns></returns>
        [HttpPost]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<PropiedadesUsuario>> Post([FromBody] PropiedadesUsuario entidad)
        {
            entidad = await servicioEntidad.CrearAsync(this.DominioId, this.TenantId, entidad).ConfigureAwait(false);
            return Ok(CreatedAtAction("Get", new { id = entidad.UsuarioId }, entidad).Value);
        }


        /// <summary>
        /// Actualoza uan entidad usuario, el Id debe incluirse en el querystring así como en 
        /// el serializado para la petición PUT
        /// </summary>
        /// <param name="id">Identificador único del usuario</param>
        /// <param name="entidad">Datos serialziados del usuario</param>
        /// <returns></returns>
        [HttpPut("{id}")]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> Put(string id, [FromBody] PropiedadesUsuario entidad)
        {
            var x = ObtieneFiltrosIdentidad();


            if (id != entidad.UsuarioId)
            {
                return BadRequest();
            }

            await servicioEntidad.ActualizarAsync(entidad).ConfigureAwait(false);
            return NoContent();

        }


        /// <summary>
        /// Obtiene una página de datos de usaurios
        /// </summary>
        /// <param name="query">Consulta para el paginado</param>
        /// <returns></returns>
        [HttpGet("page", Name = "GetPageUsuarios")]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<Paginado<PropiedadesUsuario>>> GetPage([ModelBinder(typeof(GenericDataPageModelBinder))][FromQuery] Consulta query = null)
        {
            /// Añade las propiedaes del contexto para el filtro de ACL vía ACL Controller
            /// query.Filtros.AddRange(ObtieneFiltrosIdentidad());
            var data = await servicioEntidad.ObtenerPaginadoAsync(
           Query: query,
           include: null)
           .ConfigureAwait(false);

            return Ok(data);
        }


        /// <summary>
        /// Obtiene una página de datos de usaurios
        /// </summary>
        /// <param name="query">Consulta para el paginado</param>
        /// <returns></returns>
        [HttpGet("page/{ids}", Name = "GetPageUsuariosPorIds")]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<Paginado<PropiedadesUsuario>>> GetPageUsuariosPorIds(string ids, [ModelBinder(typeof(GenericDataPageModelBinder))][FromQuery] Consulta query = null)
        {
            string IdsTrim = "";
            foreach (string item in ids.Split(',').ToList().Where(x => !string.IsNullOrEmpty(x)).ToArray())
            {
                IdsTrim += item.Trim() + ",";
            }
            List<string> lids = IdsTrim.Split(',').ToList();

            var data = await servicioEntidad.ObtenerPaginadoIdsAsync(lids,
                    Query: query,
                    include: null)
                    .ConfigureAwait(false);

            return Ok(data);
        }

        /// <summary>
        /// Obtiene un usuario en base al Id único
        /// </summary>
        /// <param name="id">Id único del usuario</param>
        /// <returns></returns>
        [HttpGet("{id}")]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<PropiedadesUsuario>> Get(string id)
        {
            var o = await servicioEntidad.UnicoAsync(x => x.UsuarioId == id).ConfigureAwait(false);
            if (o != null) return Ok(o);
            return NotFound(id);
        }


        /// <summary>
        /// Marca como eliminados una lista de usuarios en base al arreglo de identificadores recibidos
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
        /// Restaura una lista dede usuarios eliminados en base al arreglo de identificadores recibidos
        /// </summary>
        /// <param name="ids">Arreglo de identificadores string</param>
        /// <returns></returns>
        [HttpPatch("restaurar/{ids}", Name = "restaurarUsuarios")]
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
        /// Restaura una lista dede usuarios eliminados en base al arreglo de identificadores recibidos
        /// </summary>
        /// <param name="ids">Arreglo de identificadores string</param>
        /// <returns></returns>
        [HttpPatch("inactivar/{ids}", Name = "inactivarUsuarios")]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult> Inactivar(string ids)
        {
            string[] lids = ids.Split(',').ToList()
            .Where(x => !string.IsNullOrEmpty(x)).ToArray();

            return Ok(await servicioEntidad.Inactivar(lids).ConfigureAwait(false));
        }


        /// <summary>
        /// Restaura una lista dede usuarios eliminados en base al arreglo de identificadores recibidos
        /// </summary>
        /// <param name="ids">Arreglo de identificadores string</param>
        /// <returns></returns>
        [HttpPatch("reactivar/{ids}", Name = "reactivarUsuarios")]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult> Reactivar(string ids)
        {
            string[] lids = ids.Split(',').ToList()
            .Where(x => !string.IsNullOrEmpty(x)).ToArray();

            return Ok(await servicioEntidad.Inactivar(lids).ConfigureAwait(false));
        }


        /// <summary>
        /// Obtiene una lista de usuarios  en base a los parámetros de consulta
        /// </summary>
        /// <param name="query">Query de busqueda a la base de datos</param>
        /// <returns></returns>

        [HttpGet("pares", Name = "GetParesPropiedadUsuario")]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<List<ValorListaOrdenada>>> GetPares(
        [ModelBinder(typeof(GenericDataPageModelBinder))][FromQuery] Consulta query = null)
        {

            var data = await servicioEntidad.ObtenerParesAsync(query)
                .ConfigureAwait(false);

            return Ok(data);
        }


        /// <summary>
        /// Obtiene una lista de usuarios en base a con el parámetro ID de consulta
        /// </summary>
        /// <param name="ids">parametro Id para consulta a la base de datos</param>
        /// <returns></returns>

        [HttpGet("pares/{ids}", Name = "GetParesPropiedadUsuriosporId")]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<List<ValorListaOrdenada>>> GetParesporId(
              string ids)
        {

            List<string> lids = ids.Split(',').ToList()
               .Where(x => !string.IsNullOrEmpty(x)).ToList();
            var data = await servicioEntidad.ObtenerParesPorId(lids)
                .ConfigureAwait(false);

            return Ok(data);
        }

    }
}
