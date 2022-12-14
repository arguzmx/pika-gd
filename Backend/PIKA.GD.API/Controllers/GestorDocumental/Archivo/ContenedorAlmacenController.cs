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
using PIKA.Modelo.GestorDocumental;
using PIKA.Modelo.Metadatos;
using PIKA.Servicio.GestionDocumental.Interfaces;
using PIKA.Servicio.Organizacion;
using RepositorioEntidades;


namespace PIKA.GD.API.Controllers.GestorDocumental
{
    [Authorize]
    [ApiVersion("1.0")]
    [ApiController]
    [Route("api/v{version:apiVersion}/gd/[controller]")]
    public class ContenedorAlmacenController : ACLController
    {
        private readonly ILogger<ContenedorAlmacenController> logger;
        private IServicioContenedorAlmacen servicioEntidad;
        private IServicioAlmacenArchivo servicioAlmacen;
        private IProveedorMetadatos<ContenedorAlmacen> metadataProvider;
        private IServicioUnidadOrganizacional servicioUO;
        private IServicioDominio servicioDominio;
        public ContenedorAlmacenController(ILogger<ContenedorAlmacenController> logger,
            IProveedorMetadatos<ContenedorAlmacen> metadataProvider,
            IServicioContenedorAlmacen servicioEntidad,
            IServicioAlmacenArchivo servicioAlmacen,
            IServicioUnidadOrganizacional servicioUO,
            IServicioDominio servicioDominio)
        {
            this.logger = logger;
            this.servicioEntidad = servicioEntidad;
            this.metadataProvider = metadataProvider;
            this.servicioAlmacen = servicioAlmacen;
            this.servicioDominio = servicioDominio;
            this.servicioUO = servicioUO;
        }

        public override void EmiteConfiguracionSeguridad(UsuarioAPI usuario, ContextoRegistroActividad RegistroActividad, List<EventoAuditoriaActivo> Eventos)
        {
            servicioEntidad.EstableceContextoSeguridad(usuario, RegistroActividad, Eventos);
            servicioAlmacen.EstableceContextoSeguridad(usuario, RegistroActividad, Eventos);
        }

        [HttpGet()]
        [Route("filtrobusqueda/{id}")]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        public async Task<ActionResult<List<FiltroConsultaPropiedad>>> OntieneFiltroBusqueda(string id)
        {

            List<FiltroConsultaPropiedad> propiedades = new List<FiltroConsultaPropiedad>();
            FiltroConsultaPropiedad f = new FiltroConsultaPropiedad()
            {
                PropiedadId = "ZonaAlmacenId"
            };

            List<FiltroConsulta> filtros = new List<FiltroConsulta>();
            var almacen = await this.servicioAlmacen.UnicoAsync(x => x.Id == id);
            filtros.Add(new FiltroConsulta()
            {
                Negacion = false,
                NivelFuzzy = 0,
                Operador = FiltroConsulta.OP_EQ,
                Propiedad = "AlmacenArchivoId",
                Valor = almacen.Id,
                ValorString = almacen.Id
            });

            f.Filtros = filtros;
            propiedades.Add(f);

            return Ok(propiedades);
        }


        /// <summary>
        /// Obtiene los metadatos relacionados con la entidad Almacen Archivo
        /// </summary>
        /// <returns></returns>
        [HttpGet("metadata", Name = "MetadataContenedorAlmacen")]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<MetadataInfo>> GetMetadata([FromQuery] Consulta query = null)
        {
            return Ok(await metadataProvider.Obtener().ConfigureAwait(false));
        }


        /// <summary>
        /// Añade una nueva entidad del tipo Almacen Archivo
        /// </summary>
        /// <param name="entidad"></param>
        /// <returns></returns>
        [HttpPost]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<ContenedorAlmacen>> Post([FromBody] ContenedorAlmacen entidad)
        {
            entidad = await servicioEntidad.CrearAsync(entidad).ConfigureAwait(false);
            return Ok(CreatedAtAction("GetContenedorAlmacen", new { id = entidad.Id }, entidad).Value);
        }
        /// <summary>
        /// Actualiza unq entidad Almacen Archivo, el Id debe incluirse en el Querystring así como en 
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
        public async Task<IActionResult> Put(string id, [FromBody] ContenedorAlmacen entidad)
        {
            var x = ObtieneFiltrosIdentidad();


            if (id.Trim() != entidad.Id.Trim())
            {
                return BadRequest();
            }

            await servicioEntidad.ActualizarAsync(entidad).ConfigureAwait(false);
            return NoContent();

        }
        /// <summary>
        /// Devulve un alista de Almacen Archivo asociadas al objeto del tipo especificado
        /// </summary>
        /// <param name="query">Consulta para la paginación y búsqueda</param>
        /// <returns></returns>

        [HttpGet("page/almacenarchivo/{almacenarchivoid}", Name = "GetPageContenedorAlmacen")]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        [ProducesResponseType(StatusCodes.Status200OK)]

        public async Task<ActionResult<Paginado<ContenedorAlmacen>>> GetPage(string almacenarchivoid,
            [ModelBinder(typeof(GenericDataPageModelBinder))][FromQuery] Consulta query = null)
        {

            query.Filtros.Add(new FiltroConsulta() { Operador = FiltroConsulta.OP_EQ, Propiedad = "AlmacenArchivoId", Valor = almacenarchivoid });
            var data = await servicioEntidad.ObtenerPaginadoAsync(
                Query: query,
                include: null)
                .ConfigureAwait(false);

            return Ok(data);
        }

        /// <summary>
        /// Obtiene un Almacen Archivo en base al Id único
        /// </summary>
        /// <param name="id">Id único del país</param>
        /// <returns></returns>
        [HttpGet("{id}", Name = "GetContenedorAlmacen")]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]

        public async Task<ActionResult<ContenedorAlmacen>> GetContenedorAlmacen(string id)
        {

            var o = await servicioEntidad.UnicoAsync(x => x.Id == id.Trim()).ConfigureAwait(false);
            if (o != null) return Ok(o);
            return NotFound(id);
        }

        /// <summary>
        /// Elimina de manera permanente un Almacen Archivo en base al arreglo de identificadores recibidos
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
        /// Restaura una lista dede Elemento eliminados en base al arreglo de identificadores recibidos
        /// </summary>
        /// <param name="ids">Arreglo de identificadores string</param>
        /// <returns></returns>
        [HttpPatch("restaurar/{ids}", Name = "restaurarContenedorAlmacen")]
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


        [HttpGet("pares", Name = "GetParesContenedorAlmacen")]
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
        /// Obtiene una lista de archivs en base a con el parámetro ID de consulta
        /// </summary>
        /// <param name="ids">parametro Id para consulta a la base de datos</param>
        /// <returns></returns>

        [HttpGet("pares/{ids}", Name = "GetParesContenedorAlmacenporId")]
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

        [HttpGet("reporte/caratula/{id}", Name = "GetReporteCaratulaContenedorAlmacen")]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<FileResult> GetReporteCaratulaContenedorAlmacen(string id)
        {
            string dominio = "";
            string unidad = "";
            var d = await servicioDominio.UnicoAsync(x => x.Id == DominioId).ConfigureAwait(false);
            var ou = await servicioUO.UnicoAsync(x => x.Id == TenantId).ConfigureAwait(false);
            if (d != null) dominio = d.Nombre;
            if (ou != null) unidad = ou.Nombre;

            byte[] bytes = await servicioEntidad.ReporteCaratulaContenedor(dominio, d.Nombre, id).ConfigureAwait(false);

            const string contentType = "application/vnd.openxmlformats-officedocument.wordprocessingml.document";
            HttpContext.Response.ContentType = contentType;
            HttpContext.Response.Headers.Add("Access-Control-Expose-Headers", "Content-Disposition");

            var fileContentResult = new FileContentResult(bytes, contentType)
            {
                FileDownloadName = $"Caratula.docx"
            };
            return fileContentResult;
        }

    }
 }