using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PIKA.GD.API.Filters;
using PIKA.GD.API.Model;
using PIKA.Infraestructura.Comun.Seguridad;
using PIKA.Modelo.Contenido;
using PIKA.Modelo.Metadatos;
using PIKA.Servicio.Contenido.ElasticSearch;
using PIKA.Servicio.Contenido.Interfaces;
using RepositorioEntidades;

namespace PIKA.GD.API.Controllers.Contenido
{
    [Authorize]
    [ApiVersion("1.0")]
    [ApiController]
    [Route("api/v{version:apiVersion}/contenido/[controller]")]
    public class ElementoController : ACLController
    {

        private ILogger<ElementoController> logger;
        private IServicioElemento servicioEntidad;
        private IProveedorMetadatos<Elemento> metadataProvider;
        private IRepoContenidoElasticSearch repoContenido;
        private IServicioVolumen servicioVol;

        public ElementoController(
            IRepoContenidoElasticSearch repoContenido,
             IServicioVolumen servicioVol,
        ILogger<ElementoController> logger,
            IProveedorMetadatos<Elemento> metadataProvider,
            IServicioElemento servicioEntidad)
        {
            this.logger = logger;
            this.servicioEntidad = servicioEntidad;
            this.metadataProvider = metadataProvider;
            this.repoContenido = repoContenido;
            this.servicioVol = servicioVol;
        }

     
        [HttpGet("acl/{id}", Name = "ACLCarpetaContenido")]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<int>> GetACL(string id)
        {

            int permisos = 0;
            if (this.usuario.AdminGlobal)
            {
                permisos = int.MaxValue - MascaraPermisos.PDenegarAcceso; 

            } else
            {
                servicioEntidad.Usuario = this.usuario;
                permisos = await servicioEntidad.ACLPuntoMontaje(id).ConfigureAwait(false);
            }

            
            return Ok( permisos);
        }

        /// <summary>
        /// Otiene los metadatos asociados al Elemento
        /// </summary>
        /// <returns></returns>
        [HttpGet("metadata", Name = "MetadataElementoContenid")]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<MetadataInfo>> GetMetadata()
        {
            return Ok(await metadataProvider.Obtener().ConfigureAwait(false));
        }


        /// <summary>
        ///  Añade un nuevo Elemento de contenido
        /// </summary>
        /// <param name="entidad"></param>
        /// <returns></returns>
        [HttpPost]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<Elemento>> Post([FromBody]Elemento entidad)
        {
            entidad.CreadorId = this.UsuarioId;
            entidad = await servicioEntidad.CrearAsync(entidad).ConfigureAwait(false);

            // Sustituir esta sección por el almacenamiento en elasticsearc
            Modelo.Contenido.Version v = new Modelo.Contenido.Version()
            {
                Id = entidad.VersionId,
                Activa = true,
                CreadorId = entidad.CreadorId,
                ElementoId = entidad.Id,
                Eliminada = false,
                FechaCreacion = entidad.FechaCreacion,
                VolumenId = entidad.VolumenId,
                ConteoPartes = 0,
                MaxIndicePartes = 0,
                TamanoBytes = 0, 
                EstadoIndexado=  EstadoIndexado.FinalizadoOK
            };

            
            List<Task> tasks = new List<Task>() { 
                this.repoContenido.CreaVersion(v)
             };

            Task.WaitAll(tasks.ToArray());

            return Ok(CreatedAtAction("GetElemento", new { id = entidad.Id }, entidad).Value);

        }

        /// <summary>
        /// Actualiza unq entidad Elemento de contenido existente, el Id debe incluirse en el Querystring así como en 
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
        public async Task<IActionResult> Put(string id, [FromBody]Elemento entidad)
        {
            if (id != entidad.Id)
            {
                return BadRequest();
            }

            await servicioEntidad.ActualizarAsync(entidad).ConfigureAwait(false);
            return NoContent();

        }

        /// <summary>
        /// Otiene una página de resultados de Elementos de contenid en base a la configuración de paginado u al query recibido 
        /// </summary>
        /// <param name="query">Query de filtrado para el paginado</param>
        /// <returns></returns>
        [HttpGet("page", Name = "GetPageElemento")]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<Paginado<Elemento>>> GetPage([ModelBinder(typeof(GenericDataPageModelBinder))][FromQuery]Consulta query = null)
        {

            ///Añade las propiedaes del contexto para el filtro de ACL vía ACL Controller
            var data = await servicioEntidad.ObtenerPaginadoAsync(
                 Query: query)
                 .ConfigureAwait(false);

            return Ok(data);
        }

        [HttpPost("page/ids", Name = "GetPageElementoById")]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<List<Elemento>>> GetPageIds(ConsultaAPI q)
        {
            ///Añade las propiedaes del contexto para el filtro de ACL vía ACL Controller
            var data = await servicioEntidad.ObtenerPaginadoByIdsAsync(q)
                 .ConfigureAwait(false);

            return Ok(data);
        }


        /// <summary>
        /// Obtiene un Elemento en base al Id único
        /// </summary>
        /// <param name="id">Id único del país</param>
        /// <returns></returns>
        [HttpGet("{id}")]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<Elemento>> Get(string id)
        {
            var o = await servicioEntidad.UnicoAsync(x => x.Id == id).ConfigureAwait(false);
            if (o != null) return Ok(o);
            return NotFound(id);
        }


        /// <summary>
        /// Elimina de manera lógica un Elemento de contenido en base al arreglo de identificadores recibidos
        /// </summary>
        /// <param name="ids">Lista de identificadores separados por omas</param>
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

            foreach(string id in  lids)
            {
                await repoContenido.EstadoVersion(id, false).ConfigureAwait(false);
            }

            List<string> eliminados = (await servicioEntidad.Eliminar(lids).ConfigureAwait(false)).ToList();
            if (eliminados.Count==0)
            {
                return Conflict();
            } else
            {
                return Ok(eliminados);
            }
        }


        /// <summary>
        /// Este metodo  puerga todos los elementos
        /// </summary>
        /// <returns></returns>
        [HttpDelete("purgar", Name = "DeletePurgarElemento")]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<Carpeta>> DeletePurgar()
        {
            return Ok(await servicioEntidad.Purgar().ConfigureAwait(false));
        }

        /// <summary>
        /// Restaura una lista de Elementoes eliminados en base al arreglo de identificadores recibidos
        /// </summary>
        /// <param name="ids">Arreglo de identificadores string</param>
        /// <returns></returns>
        [HttpPatch("restaurar/{ids}", Name = "RestaurarElemento")]
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

            foreach (string id in lids)
            {
                await repoContenido.EstadoVersion(id, true).ConfigureAwait(false);
            }

            return Ok(await servicioEntidad.Restaurar(lids).ConfigureAwait(false));
        }

        [HttpDelete("paginas/{id}/eliminar/{csvidpaginas}", Name = "EliminaPaginasElemento")]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> EliminaPaginasElemento(string id, string csvidpaginas)
        {
            // Obtiene el elemento y su contraparte en elasticsearch
            string v = "";
            var elemento = await servicioEntidad.UnicoAsync(x => x.Id == id).ConfigureAwait(false);
            if (elemento == null)
            {
                return NotFound();
            }

            if (string.IsNullOrEmpty(v)) v = elemento.VersionId;

            Modelo.Contenido.Version vElemento = await this.repoContenido.ObtieneVersion(v).ConfigureAwait(false);
            if (vElemento == null)
            {
                return NotFound();
            }

            IGestorES gestor = await servicioVol.ObtienInstanciaGestor(elemento.VolumenId)
                           .ConfigureAwait(false);

            List<string> paginas = csvidpaginas.Split(',').ToList();
            foreach (var p in paginas)
            {
                var parte = vElemento.Partes.Where(x => x.Id == p).SingleOrDefault();
                if (parte != null)
                {
                    await repoContenido.EliminaOCR(p, vElemento).ConfigureAwait(false);
                    await gestor.EliminaBytes(elemento.Id, p, v, elemento.VolumenId, parte.Extension).ConfigureAwait(false);
                }
            }

            vElemento.Partes = vElemento.Partes.Where((x) => !paginas.Contains(x.Id)).ToList();

            await repoContenido.ActualizaVersion(elemento.VersionId, vElemento, false).ConfigureAwait(false);

            return Ok();
        }

        [HttpGet("zip/{id}/{v}", Name = "GeneraZip")]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetZIP(string id, string  v)
        {
            v = "";
            var elemento = await this.servicioEntidad.UnicoAsync(x => x.Id == id);
            if (elemento == null)
            {
                return NotFound();
            }

            if (string.IsNullOrEmpty(v)) v = elemento.VersionId;
            IGestorES gestor = await servicioVol.ObtienInstanciaGestor(elemento.VolumenId)
                           .ConfigureAwait(false);

            Modelo.Contenido.Version vElemento = await this.repoContenido.ObtieneVersion(v).ConfigureAwait(false);
            if(vElemento==null)
            {
                return NotFound();
            }
            var archivo = await gestor.ObtieneZIP(vElemento, null);

            if (string.IsNullOrEmpty(archivo)) return BadRequest();

            this.HttpContext.Response.Headers.Add("Access-Control-Expose-Headers", "Content-Disposition");
            return PhysicalFile(archivo, MimeTypes.GetMimeType($"{elemento.Nombre}.zip"), elemento.Nombre + ".zip");


        }


        [HttpGet("pdf/{id}/{v}", Name = "GeneraPDF")]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetPDF(string id, string v)
        {
            v = "";
            var elemento = await this.servicioEntidad.UnicoAsync(x => x.Id == id);
            if (elemento == null)
            {
                return NotFound();
            }

            if (string.IsNullOrEmpty(v)) v = elemento.VersionId;
            IGestorES gestor = await servicioVol.ObtienInstanciaGestor(elemento.VolumenId)
                           .ConfigureAwait(false);

            Modelo.Contenido.Version vElemento = await this.repoContenido.ObtieneVersion(v).ConfigureAwait(false);
            if (vElemento == null)
            {
                return NotFound();
            }
            var archivo = await gestor.ObtienePDF(vElemento, null);

            if (string.IsNullOrEmpty(archivo)) return BadRequest();

            this.HttpContext.Response.Headers.Add("Access-Control-Expose-Headers", "Content-Disposition");
            return PhysicalFile(archivo, MimeTypes.GetMimeType($"{elemento.Nombre}.pdf"), elemento.Nombre + ".pdf");


        }

    }
}