using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
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
using PIKA.Servicio.GestionDocumental.Servicios.Reporte;
using PIKA.Servicio.Organizacion;
using RepositorioEntidades;


namespace PIKA.GD.API.Controllers.GestorDocumental
{
    [Authorize]
    [ApiVersion("1.0")]
    [ApiController]
    [Route("api/v{version:apiVersion}/gd/[controller]")]
    public class ActivoController : ACLController
    {
        private readonly ILogger<ActivoController> logger;
        private IServicioActivo servicioActivo;
        private IProveedorMetadatos<Activo> metadataProvider;
        IServicioUnidadOrganizacional servicioUO;
        IServicioDominio servicioDominio;
        private IServicioArchivo servicioarchivo;
        private IServicioEntradaClasificacion servicioEntrada;

        public ActivoController(ILogger<ActivoController> logger,
            IProveedorMetadatos<Activo> metadataProvider,
            IServicioActivo servicioActivo,
            IServicioArchivo servicioArchivo,
            IServicioEntradaClasificacion servicioEntrada,
            IServicioUnidadOrganizacional servicioUO,
            IServicioDominio servicioDominio)
        {
            this.logger = logger;
            this.servicioActivo = servicioActivo;
            this.metadataProvider = metadataProvider;
            this.servicioUO = servicioUO;
            this.servicioDominio = servicioDominio;
            this.servicioarchivo = servicioArchivo;
            this.servicioEntrada = servicioEntrada;
        }


        /// <summary>
        /// Obtiene los metadatos relacionados con la entidad Activo
        /// </summary>
        /// <returns></returns>
        [HttpGet("metadata", Name = "MetadataActivo")]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        [ProducesResponseType(StatusCodes.Status200OK)]

        public async Task<ActionResult<MetadataInfo>> GetMetadata([FromQuery] Consulta query = null)
        {
            return Ok(await metadataProvider.Obtener().ConfigureAwait(false));

        }


        /// <summary>
        /// Añade una nueva entidad del tipo Activo
        /// </summary>
        /// <param name="entidad"></param>
        /// <returns></returns>

        [HttpPost]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        [ProducesResponseType(StatusCodes.Status200OK)]

        public async Task<ActionResult<Activo>> Post([FromBody] Activo entidad)
        {
            entidad = await servicioActivo.CrearAsync(entidad).ConfigureAwait(false);
            return Ok(CreatedAtAction("GetActivo", new { id = entidad.Id }, entidad).Value);
        }


        /// <summary>
        /// Actualiza una entidad Activo, el Id debe incluirse en el Querystring así como en 
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
        public async Task<IActionResult> Put(string id, [FromBody] Activo entidad)
        {
            var x = ObtieneFiltrosIdentidad();


            if (id.Trim() != entidad.Id.Trim())
            {
                return BadRequest();
            }

            await servicioActivo.ActualizarAsync(entidad).ConfigureAwait(false);
            return NoContent();

        }

        /// <summary>
        /// Devulve un alista de Activo asociadas al objeto del tipo especificado
        /// </summary>
        /// <param name="query">Consulta para la paginación y búsqueda</param>
        /// <returns></returns>

        [HttpGet("page", Name = "GetPageActivo")]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        public async Task<ActionResult<Paginado<Activo>>> GetPage([ModelBinder(typeof(GenericDataPageModelBinder))][FromQuery] Consulta query = null)
        {
            var data = await servicioActivo.ObtenerPaginadoAsync(
                Query: query,
                include: null)
                .ConfigureAwait(false);

            return Ok(data);
        }


        [HttpGet("page/archivo/{Id}", Name = "GetPageActivoArchivo")]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        public async Task<ActionResult<Paginado<Activo>>> GetPageActivoArchivo(string Id, [ModelBinder(typeof(GenericDataPageModelBinder))][FromQuery] Consulta query = null)
        {
            query.Filtros.Add(new FiltroConsulta()
            {
                Operador = FiltroConsulta.OP_EQ,
                Propiedad = "ArchivoId",
                Valor = Id
            });

            var data = await servicioActivo.ObtenerPaginadoAsync(
                Query: query,
                include: null)
                .ConfigureAwait(false);

            return Ok(data);
        }

        [HttpGet("page/archivo/{Id}/texto/{texto}", Name = "GetPageActivoArchivoPorTexto")]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        public async Task<ActionResult<Paginado<Activo>>> GetPageActivoArchivoPorTexto(string Id, string texto, [ModelBinder(typeof(GenericDataPageModelBinder))][FromQuery] Consulta query = null)
        {
            query.Filtros.Add(new FiltroConsulta()
            {
                Operador = FiltroConsulta.OP_EQ,
                Propiedad = "ArchivoId",
                Valor = Id
            });
            
            var data = await servicioActivo.ObtenerPaginadoAsync(WebUtility.UrlDecode(texto), Query: query, include: null).ConfigureAwait(false);

            return Ok(data);
        }

        [HttpGet("page/unidadadministrativaarchivo/{Id}", Name = "GetPageActivoUAdminArchivo")]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        public async Task<ActionResult<Paginado<Activo>>> GetPageActivoUnidadAdmin(string Id, [ModelBinder(typeof(GenericDataPageModelBinder))][FromQuery] Consulta query = null)
        {
            query.Filtros.Add(new FiltroConsulta()
            {
                Operador = FiltroConsulta.OP_EQ,
                Propiedad = "UnidadAdministrativaArchivoId",
                Valor = Id
            });

            var data = await servicioActivo.ObtenerPaginadoAsync(
                Query: query,
                include: null)
                .ConfigureAwait(false);

            return Ok(data);
        }

        //----------------------------------------------------------------------------------------------------------
        //----------------------------------------------------------------------------------------------------------
        //----------------------------------------------------------------------------------------------------------
        //----------------------------------------------------------------------------------------------------------

        /// <summary>
        /// Obtiene un Activo en base al Id único
        /// </summary>
        /// <param name="id">Id único del Activo</param>
        /// <returns></returns>

        [HttpGet("{id}")]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<Activo>> Get(string id)
        {
            var o = await servicioActivo.UnicoAsync(x => x.Id == id.Trim()).ConfigureAwait(false);
            if (o != null) return Ok(o);
            return NotFound(id);
        }

        [HttpGet("Importar", Name = "GetImportarActivo")]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<FileResult> GetReporteCuadroClasificacion([FromBody] PropiedadesImportadorActivos p)
        {

            byte[] bytes = await servicioActivo.ImportarActivos(p.archivo, p.ArchivoId, p.TipoOrigenId, p.OrigenId, p.FormatoFecha).ConfigureAwait(false);
            var cuadro = await servicioActivo.UnicoAsync(x => x.ArchivoId == p.ArchivoId).ConfigureAwait(false);

            const string contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            HttpContext.Response.ContentType = contentType;
            HttpContext.Response.Headers.Add("Access-Control-Expose-Headers", "Content-Disposition");

            var fileContentResult = new FileContentResult(bytes, contentType)
            {
                FileDownloadName = cuadro.Nombre
            };
            return fileContentResult;
        }


        /// <summary>
        /// Elimina de manera permanente un Activo en base al arreglo de identificadores recibidos
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
            return Ok(await servicioActivo.Eliminar(lids).ConfigureAwait(false));
        }


        /// <summary>
        /// Restaura una lista dede Activos eliminados en base al arreglo de identificadores recibidos
        /// </summary>
        /// <param name="ids">Arreglo de identificadores string</param>
        /// <returns></returns>
        [HttpPatch("restaurar/{ids}", Name = "restaurarActivo")]
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

            return Ok(await servicioActivo.Restaurar(lids).ConfigureAwait(false));
        }



        [HttpGet("reporte/caratula/{id}", Name = "GetReporteCaratuaActivo")]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<FileResult> GetReporteCaratuaActivo(string id)
        {
            string dominio = "";
            string unidad = "";
            var d = await this.servicioDominio.UnicoAsync(x => x.Id == this.DominioId);
            var ou = await this.servicioUO.UnicoAsync(x => x.Id == this.TenantId);
            if (d != null) dominio = d.Nombre;
            if (ou != null) unidad = ou.Nombre;

            byte[] bytes = await servicioActivo.ReporteCaratulaActivo(dominio, d.Nombre, id).ConfigureAwait(false);

            const string contentType = "application/vnd.openxmlformats-officedocument.wordprocessingml.document";
            HttpContext.Response.ContentType = contentType;
            HttpContext.Response.Headers.Add("Access-Control-Expose-Headers", "Content-Disposition");

            var fileContentResult = new FileContentResult(bytes, contentType)
            {
                FileDownloadName = $"Caratula.docx"
            };
            return fileContentResult;
        }


        #region contenido vinculado

        [HttpGet("{id}/contenido")]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<Modelo.Contenido.Elemento>> GetContenidoVinculado(string id)
        {
            List<Task> tareas = new List<Task>();
            var o = await servicioActivo.UnicoAsync(x => x.Id == id.Trim()).ConfigureAwait(false);
            if (o == null) return NotFound(id);

            var a = await servicioarchivo.UnicoAsync(x => x.Id == o.ArchivoId);
            var entrada = await servicioEntrada.UnicoAsync(e => e.Id == o.EntradaClasificacionId);

            ElementoContenidoVinculado el = new ElementoContenidoVinculado()
            {
                Id = o.TieneContenido ? o.ElementoId : null,
                Nombre = o.Nombre,
                Eliminada = false,
                PuntoMontajeId = a.PuntoMontajeId,
                CreadorId = this.UsuarioId,
                FechaCreacion = DateTime.Now,
                VolumenId = a.VolumenDefaultId,
                CarpetaId = null,
                PermisoId = null,
                TipoOrigenId = nameof(Activo),
                OrigenId = id,
                Versionado = true,
                VersionId = null,
                RutaRepositorio = $"/{entrada.Clave} {entrada.Nombre}"
            };
            return Ok(el);
        }

        [HttpPost("{id}/contenido/{contenidoId}")]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<Modelo.Contenido.Elemento>> CreaContenidoVinculado(string id, string contenidoId)
        {
            List<Task> tareas = new List<Task>();
            var o = await servicioActivo.UnicoAsync(x => x.Id == id.Trim()).ConfigureAwait(false);
            if (o == null) return NotFound(id);

            o.ElementoId = contenidoId;
            o.TieneContenido = true;

            await servicioActivo.ActualizarAsync(o);

            return Ok(o);
        }


        #endregion

        #region Seleccion

        [HttpGet("tema", Name = "ObtieneTemasActivosSeleccioandos")]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<List<ValorListaOrdenada>>> ObtieneTemasActivosSeleccioandos()
        {
            var temas = await servicioActivo.ObtienTemas(this.GetUserId());
            return Ok(temas);
        }


        [HttpPost("tema/{nombre}", Name = "CreaTemaActivosSeleccionados")]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<ValorListaOrdenada>> CreaTemaActivosSeleccionados(string  nombre)
        {
           var id =  await servicioActivo.CreaTema(nombre , this.GetUserId());
            return Ok(new ValorListaOrdenada() { Id = id, Texto = nombre, Indice =0 });
        }

        [HttpDelete("tema/{id}", Name = "EliminaTemaActivosSeleccionados")]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<string>> EliminaTemaActivosSeleccionados(string id)
        {
            await servicioActivo.EliminaTema(id, this.GetUserId());
            return Ok();
        }


        [HttpGet("tema/{temaid}/seleccion", Name = "ObtieneActivosSeleccioandos")]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<List<Activo>>> ObtieneActivosSeleccioandos(string temaid)
        {
            var seleccionados = await servicioActivo.ObtieneSeleccionados(temaid, this.GetUserId());
            return Ok(seleccionados);
        }

        [HttpPost("tema/{temaid}/seleccion/{id}", Name = "CreaActivoSeleccionado")]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult> CreaActivoSeleccionado(string temaid, string id)
        {
            await servicioActivo.CrearSeleccion(id, temaid, this.GetUserId());
            return Ok();
        }

        [HttpPost("tema/{temaid}/seleccion", Name = "CreaActivoSeleccionadoLista")]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult> CreaActivoSeleccionadoLista(string temaid, [FromBody] ListaStringIds lista)
        {
            await servicioActivo.CrearSeleccion(lista.Ids, temaid, this.GetUserId());
            return Ok();
        }


        [HttpDelete("tema/{temaid}/seleccion/{id}", Name = "EliminaActivoSeleccionado")]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult> EliminaActivoSeleccionado(string temaid, string id)
        {
            await servicioActivo.EliminaSeleccion(id, temaid, this.GetUserId());
            return Ok();
        }


        [HttpPost("tema/{temaid}seleccion/eliminar", Name = "EliminaActivoSeleccionadoLista")]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult> EliminaActivoSeleccionadoLista(string temaid, [FromBody] ListaStringIds lista)
        {
            await servicioActivo.EliminaSeleccion(lista.Ids, temaid, this.GetUserId());
            return Ok();
        }


        [HttpDelete("tema/{temaid}/seleccion/vaciar", Name = "EliminaSeleccion")]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult> EliminaSeleccion(string temaid)
        {
            await servicioActivo.BorraSeleccion(temaid, this.GetUserId());
            return Ok();
        }

        #endregion


        [HttpGet("pares", Name = "GetParesActivos")]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<List<ValorListaOrdenada>>> GetParesActivos(
[ModelBinder(typeof(GenericDataPageModelBinder))][FromQuery] Consulta query = null)
        {
            var data = await servicioActivo.ObtenerParesAsync(query)
                .ConfigureAwait(false);

            return Ok(data);
        }

        

        [HttpGet("pares/{ids}", Name = "GetParesActivosporId")]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<List<ValorListaOrdenada>>> GetParesActivosporId(
              string ids)
        {

            List<string> lids = ids.Split(',').ToList()
               .Where(x => !string.IsNullOrEmpty(x)).ToList();
            var data = await servicioActivo.ObtenerParesPorId(lids)
                .ConfigureAwait(false);

            return Ok(data);
        }

    }
}