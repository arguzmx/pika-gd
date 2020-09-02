using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PIKA.GD.API.Filters;
using PIKA.GD.API.Model;
using PIKA.Modelo.Metadatos;
using PIKA.Modelo.Organizacion;
using PIKA.Servicio.Organizacion.Interfaces;
using RepositorioEntidades;


namespace PIKA.GD.API.Controllers.Organizacion
{

    [Authorize]
    [ApiVersion("1.0")]
    [ApiController]
    [Route("api/v{version:apiVersion}/org/[controller]")]
    public class UsuariosRolController : ACLController
    {
        private ILogger<UsuariosRolController> logger;
        private IServicioUsuariosRol servicioUsuariosRol;
        private IProveedorMetadatos<UsuariosRol> metadataProvider;

        public UsuariosRolController(ILogger<UsuariosRolController> logger,
          IProveedorMetadatos<UsuariosRol> metadataProvider,
          IServicioUsuariosRol servicioUsuariosRol)
        {
            this.logger = logger;
            this.servicioUsuariosRol = servicioUsuariosRol;
            this.metadataProvider = metadataProvider;
        }

        /// <summary>
        /// Obtiene los metadatos relacionados con la entidad UsuariosRol
        /// </summary>
        /// <returns></returns>
        [HttpGet("metadata", Name = "MetadataUsuariosRol")]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<MetadataInfo>> GetMetadata([FromQuery] Consulta query = null)
        {
            return Ok(await metadataProvider.Obtener().ConfigureAwait(false));
        }


        /// <summary>
        /// Añade una nueva entidad del tipo UsuariosRol
        /// </summary>
        /// <param name="entidad"></param>
        /// <returns></returns>
        [HttpPost]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<UsuariosRol>> Post([FromBody] UsuariosRol entidad)
        {
            entidad = await servicioUsuariosRol.CrearAsync(entidad).ConfigureAwait(false);
            return Ok(CreatedAtAction("GetUsuariosRol", new { id = entidad.ApplicationUserId }, entidad).Value);
        }


        /// <summary>
        /// Vincula una lista de IDs al objeto UsuariosRol
        /// </summary>
        /// <param name="entidad"></param>
        /// <returns></returns>
        [HttpPost("{rolid}", Name = "PostMultipleUsuariosRol")]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<int>> PostIds(string rolid, [FromBody] string[] ids)
        {
            int cantidad = await servicioUsuariosRol.PostIds(rolid, ids).ConfigureAwait(false);
            return Ok(cantidad);
        }



        /// <summary>
        /// Obtiene una página de datos del dominio 
        /// </summary>
        /// <param name="query">Consulta para el paginado</param>
        /// <returns></returns>
        [HttpGet("page/{tipo}/{id}", Name = "GetPageUsuariosRoles")]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<Paginado<UsuariosRol>>> GetPage(
              string tipo, string id, 
              [ModelBinder(typeof(GenericDataPageModelBinder))][FromQuery]Consulta query = null)
        {
            ///Añade las propiedaes del contexto para el filtro de ACL vía ACL Controller
            query.Filtros.Add(new FiltroConsulta() { Propiedad = "RolId", Operador = FiltroConsulta.OP_EQ, Valor = id });

            var data = await servicioUsuariosRol.ObtenerPaginadoAsync(query).ConfigureAwait(false);
            return Ok(data);
        }



        /// <summary>
        /// Marca como eliminados una lista de dominiios en base al arreglo de identificadores recibidos
        /// </summary>
        /// <param name="ids">Arreglo de identificadores string</param>
        /// <returns></returns>
        [HttpDelete("{rolid}/{ids}")]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult> Delete(string rolid, string ids)
        {
            string IdsTrim = "";
            foreach (string item in ids.Split(',').ToList().Where(x => !string.IsNullOrEmpty(x)).ToArray())
            {
                IdsTrim += item.Trim() + ",";
            }
            string[] lids = IdsTrim.Split(',').ToList()
           .Where(x => !string.IsNullOrEmpty(x)).ToArray();
            return Ok(await servicioUsuariosRol.DeleteIds(rolid, lids).ConfigureAwait(false));
        }

    }
}
