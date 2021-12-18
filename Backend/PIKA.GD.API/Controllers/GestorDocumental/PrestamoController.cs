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
using PIKA.Infraestructura.Comun;
using PIKA.Modelo.GestorDocumental;
using PIKA.Modelo.GestorDocumental.Reportes.JSON;
using PIKA.Modelo.Metadatos;
using PIKA.Servicio.GestionDocumental.Interfaces;
using PIKA.Servicio.Seguridad.Interfaces;
using RepositorioEntidades;


namespace PIKA.GD.API.Controllers.GestorDocumental
{
    [Authorize]
    [ApiVersion("1.0")]
    [ApiController]
    [Route("api/v{version:apiVersion}/gd/[controller]")]
    public class PrestamoController : ACLController
    {
        private readonly ILogger<PrestamoController> logger;
        private IServicioPrestamo servicioPrestamo;
        private IServicioUsuarios servicioUsuarios;
        private IProveedorMetadatos<Prestamo> metadataProvider;

        public PrestamoController(ILogger<PrestamoController> logger,
            IProveedorMetadatos<Prestamo> metadataProvider,
            IServicioPrestamo servicioPrestamo,
            IServicioUsuarios servicioUsuarios)
        {
            this.logger = logger;
            this.servicioPrestamo = servicioPrestamo;
            this.metadataProvider = metadataProvider;
            this.servicioUsuarios = servicioUsuarios;
        }

        [HttpPost("webcommand/{command}")]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        public async Task<ActionResult<RespuestaComandoWeb>> Post(string command, [FromBody] object payload)
        {
            RespuestaComandoWeb r = await servicioPrestamo.ComandoWeb(command, payload).ConfigureAwait(false);
            return Ok(r);
        }

        [HttpGet("reporte/prestamo/{id}", Name = "GetReportePrestamo")]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult> GetReportePrestamo(string id)
        {
  
            var p = await servicioPrestamo.UnicoAsync(x=>x.Id == id).ConfigureAwait(false);
            if (p == null)
            {
                return NotFound();
            }

            var prestador = await servicioUsuarios.UnicoAsync(x=>x.UsuarioId == p.UsuarioOrigenId).ConfigureAwait(false);
            var prestatario = await servicioUsuarios.UnicoAsync(x => x.UsuarioId == p.UsuarioDestinoId).ConfigureAwait(false);


            byte[] bytes = await servicioPrestamo.ReportePrestamo("", id, prestador.AUsuarioPrestamo(), prestatario.AUsuarioPrestamo()).ConfigureAwait(false);
            
            string contentType = MimeTypes.GetMimeType("x.docx");
            string downloadName = $"Prestamo {p.Folio}.docx";
            HttpContext.Response.ContentType = contentType;
            HttpContext.Response.Headers.Add("Access-Control-Expose-Headers", "Content-Disposition");

            var fileContentResult = new FileContentResult(bytes, contentType)
            {
                FileDownloadName = downloadName
            };
            return fileContentResult;
        }

        #region Prestamos

        [HttpGet("metadata", Name = "MetadataPrestamo")]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        public async Task<ActionResult<MetadataInfo>> GetMetadata([FromQuery]Consulta query = null)
        {
            return Ok(await metadataProvider.Obtener().ConfigureAwait(false));
        }

        [HttpPost]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        public async Task<ActionResult<Prestamo>> Post([FromBody]Prestamo entidad)
        {
            entidad.UsuarioOrigenId = this.UsuarioId;
            if (string.IsNullOrEmpty(entidad.TemaId))
            {
                entidad = await servicioPrestamo.CrearAsync(entidad).ConfigureAwait(false);
            }
            else
            {
                entidad = await servicioPrestamo.CrearDesdeTemaAsync(entidad, entidad.TemaId).ConfigureAwait(false);
            }

            return Ok(CreatedAtAction("GetPrestamo", new { id = entidad.Id }, entidad).Value);
        }


        


        [HttpPut("{id}")]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        public async Task<IActionResult> Put(string id, [FromBody]Prestamo entidad)
        {
            var x = ObtieneFiltrosIdentidad();
            if (id != entidad.Id)
            {
                return BadRequest();
            }
            await servicioPrestamo.ActualizarAsync(entidad).ConfigureAwait(false);
            return NoContent();
        }


        [HttpGet("page", Name = "GetPagePrestamo")]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        public async Task<ActionResult<Paginado<Prestamo>>> GetPage([ModelBinder(typeof(GenericDataPageModelBinder))][FromQuery]Consulta query = null)
        {
            ///Añade las propiedaes del contexto para el filtro de ACL vía ACL Controller
            var data = await servicioPrestamo.ObtenerPaginadoAsync(
                  Query: query,
                  include: null)
                  .ConfigureAwait(false);

            return Ok(data);
        }


        [HttpGet("page/archivo/{Id}", Name = "GetPagePrestamosArchivo")]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        public async Task<ActionResult<IEnumerable<Activo>>> GetPagePrestamosArchivo(string Id, [ModelBinder(typeof(GenericDataPageModelBinder))][FromQuery] Consulta query = null)
        {
            query.Filtros.Add(new FiltroConsulta()
            {
                Operador = FiltroConsulta.OP_EQ,
                Propiedad = "ArchivoId",
                Valor = Id
            });

            var data = await servicioPrestamo.ObtenerPaginadoAsync(
                  Query: query,
                  include: null)
                  .ConfigureAwait(false);

            return Ok(data);
        }


        [HttpGet("{id}")]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        public async Task<ActionResult<Prestamo>> Get(string id)
        {
            var o = await servicioPrestamo.UnicoAsync(x => x.Id == id).ConfigureAwait(false);
            if (o != null) return Ok(o);
            return NotFound(id);
        }

        [HttpDelete("delete",Name ="DeletePrestamo")]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        public async Task<ActionResult> Delete(string ids)
        {
            string IdsTrim = "";
            foreach (string item in ids.Split(',').ToList().Where(x => !string.IsNullOrEmpty(x)).ToArray())
            {
                IdsTrim += item.Trim() + ",";
            }
            string[] Ids = IdsTrim.Split(',').ToList().Where(x => !string.IsNullOrEmpty(x)).ToArray();

            return Ok(await servicioPrestamo.Eliminar(Ids).ConfigureAwait(false));
        }
        /// <summary>
        /// Este metodo  puerga todos los elementos
        /// </summary>
        /// <returns></returns>
        [HttpDelete("purgar", Name = "DeletePurgarPrestamo")]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<Prestamo>> DeletePurgar()
        {
            return Ok(await servicioPrestamo.Purgar().ConfigureAwait(false));
        }
        #endregion

    }
}