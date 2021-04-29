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
using PIKA.Modelo.GestorDocumental;
using PIKA.Modelo.Metadatos;
using PIKA.Servicio.GestionDocumental.Interfaces;
using RepositorioEntidades;

namespace PIKA.GD.API.Controllers.GestorDocumental
{
    [Authorize]
    [ApiVersion("1.0")]
    [ApiController]
    [Route("api/v{version:apiVersion}/gd/[controller]")]
    public class ArchivoController : ACLController
    {
        private readonly ILogger<ArchivoController> logger;
        private IServicioArchivo servicioArchivo;
        private IProveedorMetadatos<Archivo> metadataProvider;
        public ArchivoController(ILogger<ArchivoController> logger,
            IProveedorMetadatos<Archivo> metadataProvider,
            IServicioArchivo servicioArchivo)
        {
            this.logger = logger;
            this.servicioArchivo = servicioArchivo;
            this.metadataProvider = metadataProvider;
        }
        /// <summary>
        /// Obtiene los metadatos relacionados con la entidad Archivo
        /// </summary>
        /// <returns></returns>
        [HttpGet("metadata", Name = "MetadataArchivo")]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        [ProducesResponseType(StatusCodes.Status200OK)]

        public async Task<ActionResult<MetadataInfo>> GetMetadata([FromQuery]Consulta query = null)
        {
            return Ok(await metadataProvider.Obtener().ConfigureAwait(false));
        }


        /// <summary>
        /// Añade una nueva entidad del tipo Archivo
        /// </summary>
        /// <param name="entidad"></param>
        /// <returns></returns>

        [HttpPost]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        [ProducesResponseType(StatusCodes.Status200OK)]

        public async Task<ActionResult<Archivo>> Post([FromBody]Archivo entidad)
        {
            entidad = await servicioArchivo.CrearAsync(entidad).ConfigureAwait(false);
            return Ok(CreatedAtAction("GetArchivo", new { id = entidad.Id }, entidad).Value);
        }

        /// <summary>
        /// Actualiza una entidad archivo, el Id debe incluirse en el Querystring así como en 
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
        public async Task<IActionResult> Put(string id, [FromBody]Archivo entidad)
        {
            var x = ObtieneFiltrosIdentidad();


            if (id.Trim() != entidad.Id.Trim())
            {
                return BadRequest();
            }

            await servicioArchivo.ActualizarAsync(entidad).ConfigureAwait(false);
            return NoContent();

        }
        /// <summary>
        /// Devulve una lista de Archivos asociadas al objeto del tipo especificado
        /// </summary>
        /// <param name="query">Consulta para la paginación y búsqueda</param>
        /// <returns></returns>


        [HttpGet("page", Name = "GetPageArchivo")]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        [ProducesResponseType(StatusCodes.Status200OK)]

        /// <summary>
        /// Obtiene un Archivo en base al Id único
        /// </summary>
        /// <param name="id">Id único del Cuadro Clasificacion</param>
        /// <returns></returns>
        public async Task<ActionResult<IEnumerable<Archivo>>> GetPage([FromQuery]Consulta query = null)
        {
            var data = await servicioArchivo.ObtenerPaginadoAsync(
                  Query: query,
                  include: null)
                  .ConfigureAwait(false);

            return Ok(data);
        }
        /// <summary>
        /// Obtiene un Archivo en base al Id único
        /// </summary>
        /// <param name="id">Id único del Cuadro Clasificacion</param>
        /// <returns></returns>

        [HttpGet("{id}")]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        [ProducesResponseType(StatusCodes.Status200OK)]

        public async Task<ActionResult<Archivo>> Get(string id)
        {
            var o = await servicioArchivo.UnicoAsync(x => x.Id == id.Trim()).ConfigureAwait(false);
            if (o != null) return Ok(o);
            return NotFound(id);
        }

        /// <summary>
        /// Elimina de manera permanente un Archivo en base al arreglo de identificadores recibidos
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
            return Ok(await servicioArchivo.Eliminar(lids).ConfigureAwait(false));
        }


        /// <summary>
        /// Restaura una lista dede dominios eliminados en base al arreglo de identificadores recibidos
        /// </summary>
        /// <param name="ids">Arreglo de identificadores string</param>
        /// <returns></returns>
        [HttpPatch("restaurar/{ids}", Name = "restaurarArchivo")]
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

            return Ok(await servicioArchivo.Restaurar(lids).ConfigureAwait(false));
        }

        /// <summary>
        /// Obtiene una lista de archivos en base a los parámetros de consulta
        /// </summary>
        /// <param name="query">Query de busqueda a la base de datos</param>
        /// <returns></returns>

        [HttpGet("pares", Name = "GetParesArchivo")]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<List<ValorListaOrdenada>>> GetPares(
        [ModelBinder(typeof(GenericDataPageModelBinder))][FromQuery] Consulta query = null)
        {
            var data = await servicioArchivo.ObtenerParesAsync(query)
                .ConfigureAwait(false);

            return Ok(data);
        }
        /// <summary>
        /// Obtiene una lista de archivs en base a con el parámetro ID de consulta
        /// </summary>
        /// <param name="ids">parametro Id para consulta a la base de datos</param>
        /// <returns></returns>

        [HttpGet("pares/{ids}", Name = "GetParesArchivoporId")]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<List<ValorListaOrdenada>>> GetParesporId(
              string ids)
        {

            List<string> lids = ids.Split(',').ToList()
               .Where(x => !string.IsNullOrEmpty(x)).ToList();
            var data = await servicioArchivo.ObtenerParesPorId(lids)
                .ConfigureAwait(false);

            return Ok(data);
        }

        /// <summary>
        /// Este metodo  puerga todos los elementos
        /// </summary>
        /// <returns></returns>
        [HttpDelete("purgar", Name = "DeletePurgarArchivo")]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<Prestamo>> DeletePurgar()
        {
            return Ok(await servicioArchivo.Purgar().ConfigureAwait(false));
        }


        [HttpGet("reporte/guiasimple/{id}", Name = "GetReporteguiasimple")]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<FileResult> GetReporteguiasimple(string id)
        {

            byte[] bytes = await servicioArchivo.ReporteGuiaSimpleArchivo(id).ConfigureAwait(false);
            var archivo = await servicioArchivo.UnicoAsync(x => x.Id == id).ConfigureAwait(false);

            const string contentType = "application/vnd.openxmlformats-officedocument.wordprocessingml.document";
            HttpContext.Response.ContentType = contentType;
            HttpContext.Response.Headers.Add("Access-Control-Expose-Headers", "Content-Disposition");

            var fileContentResult = new FileContentResult(bytes, contentType)
            {
                FileDownloadName = $"Guia simplie archivo {archivo.Nombre}.docx"
            };
            return fileContentResult;
        }



        [HttpGet("reporte/inventario/{id}", Name = "GetReporteIvventario")]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<FileResult> GetReporteIvventario(string id)
        {

            string file =  await servicioArchivo.ReporteGuiaInventario(id).ConfigureAwait(false);
            
            var archivo = await servicioArchivo.UnicoAsync(x => x.Id == id).ConfigureAwait(false);

            const string contentType = "application/vnd.openxmlformats-officedocument.wordprocessingml.document";
            HttpContext.Response.ContentType = contentType;
            HttpContext.Response.Headers.Add("Access-Control-Expose-Headers", "Content-Disposition");

            var bytes = await System.IO.File.ReadAllBytesAsync(file);

            var fileContentResult = new FileContentResult(bytes, contentType)
            {
                FileDownloadName = $"Inventario {DateTime.Now.ToString("dd/MM/yyyy")} {archivo.Nombre}.csv"
            };
            return fileContentResult;
        }

    }
}