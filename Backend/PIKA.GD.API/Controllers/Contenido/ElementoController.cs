using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PIKA.GD.API.Filters;
using PIKA.GD.API.Model;
using PIKA.Infraestructura.Comun.Seguridad;
using PIKA.Modelo.Aplicacion.Tareas;
using PIKA.Modelo.Contenido;
using PIKA.Modelo.Metadatos;
using PIKA.Servicio.AplicacionPlugin.Interfaces;
using PIKA.Servicio.Contenido;
using PIKA.Servicio.Contenido.ElasticSearch;
using PIKA.Servicio.Contenido.Interfaces;
using PIKA.Servicio.Contenido.Servicios.TareasAutomaticas;
using RepositorioEntidades;
using ComunTreas = PIKA.Infraestructura.Comun.Tareas;

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
        private IServicioTareaEnDemanda tareaEnDemanda;


        public ElementoController(
            IRepoContenidoElasticSearch repoContenido,
             IServicioVolumen servicioVol,
        ILogger<ElementoController> logger,
            IProveedorMetadatos<Elemento> metadataProvider,
            IServicioElemento servicioEntidad,
            IServicioTareaEnDemanda tareaEnDemanda)
        {
            this.logger = logger;
            this.servicioEntidad = servicioEntidad;
            this.metadataProvider = metadataProvider;
            this.repoContenido = repoContenido;
            this.servicioVol = servicioVol;
            this.tareaEnDemanda = tareaEnDemanda;
        }


        public override void EmiteConfiguracionSeguridad(UsuarioAPI usuario, ContextoRegistroActividad RegistroActividad, List<EventoAuditoriaActivo> Eventos)
        {
            servicioEntidad.EstableceContextoSeguridad(usuario, RegistroActividad, Eventos);
            servicioVol.EstableceContextoSeguridad(usuario, RegistroActividad, Eventos);
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

            }
            else
            {
                permisos = await servicioEntidad.ACLPuntoMontaje(id).ConfigureAwait(false);
            }


            return Ok(permisos);
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
        public async Task<ActionResult<Elemento>> Post([FromBody] Elemento entidad)
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
                EstadoIndexado = EstadoIndexado.FinalizadoOK
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
        public async Task<IActionResult> Put(string id, [FromBody] Elemento entidad)
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
        public async Task<ActionResult<Paginado<Elemento>>> GetPage([ModelBinder(typeof(GenericDataPageModelBinder))][FromQuery] Consulta query = null)
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

            foreach (string id in lids)
            {
                await repoContenido.EstadoVersion(id, false).ConfigureAwait(false);
            }

            List<string> eliminados = (await servicioEntidad.Eliminar(lids).ConfigureAwait(false)).ToList();
            if (eliminados.Count == 0)
            {
                return Conflict();
            }
            else
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

        [HttpPost("paginas/{id}/ordernar/alfabetico")]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        public async Task<ActionResult> OrdenarContenidoAlfabetico(string id)
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

            if (vElemento != null)
            {
                vElemento.Partes = vElemento.Partes.OrderBy(p => p.NombreOriginal).ToList();
                int idx = 1;
                foreach (var p in vElemento.Partes)
                {
                    p.Indice = idx;
                    idx++;
                }

                await repoContenido.EliminaOCRVersion(vElemento).ConfigureAwait(false);
                await repoContenido.ActualizaVersion(vElemento.Id, vElemento, true).ConfigureAwait(false);

                await servicioEntidad.EventoActualizarVersionElemento(AplicacionContenido.EventosAdicionales.ModificarPaginas.GetHashCode(), id,true, null, id).ConfigureAwait(false);
            }

            return Ok();
        }

        [HttpPost("paginas/{id}/rotar/{angulo}")]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        public async Task<ActionResult> RotarPagina(string id, int angulo, [FromBody] string[] ids)
        {

            if(ids == null || ids.Length ==0)
            {
                return BadRequest("Los ids de las partes son obligatorios");
            }

            var elemento = await servicioEntidad.UnicoAsync(x => x.Id == id).ConfigureAwait(false);
            if (elemento == null)
            {
                return NotFound();
            }

            Modelo.Contenido.Version vElemento = await this.repoContenido.ObtieneVersion(elemento.VersionId).ConfigureAwait(false);
            if (vElemento == null)
            {
                return NotFound();
            }

            foreach(var parteId in  ids.ToList())
            {
                var parte = vElemento.Partes.Where(x => x.Id == parteId).SingleOrDefault();
                if (parte != null)
                {
                    IGestorES gestor = await servicioVol.ObtienInstanciaGestor(elemento.VolumenId)
                   .ConfigureAwait(false);

                    var resultado = await gestor.RotarImagen(elemento.Id, parte.Id, vElemento.Id, elemento.VolumenId, parte.Extension, angulo).ConfigureAwait(false);
                    if (!string.IsNullOrEmpty(resultado))
                    {
                        parte.Indexada = false;
                        vElemento.EstadoIndexado = EstadoIndexado.PorIndexar;
                    }

                }
            }

            await repoContenido.ActualizaVersion(vElemento.Id, vElemento, true).ConfigureAwait(false);
            await servicioEntidad.EventoActualizarVersionElemento(AplicacionContenido.EventosAdicionales.RotarPaginas.GetHashCode(), id, true, null, Newtonsoft.Json.JsonConvert.SerializeObject(ids.ToList())).ConfigureAwait(false);
            return Ok();

        }

        [HttpPost("paginas/{id}/reflejar/{direccion}")]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        public async Task<ActionResult> ReflejarPagina(string id, string direccion, [FromBody] string[] ids)
        {
            if (ids == null || ids.Length == 0)
            {
                return BadRequest("Los ids de las partes son obligatorios");
            }

            if ("VH".IndexOf(direccion.ToUpper()) < 0) {
                return BadRequest("Lo valores válidos para la dirección son V|H");
            }

            var elemento = await servicioEntidad.UnicoAsync(x => x.Id == id).ConfigureAwait(false);
            if (elemento == null)
            {
                return NotFound();
            }

            Modelo.Contenido.Version vElemento = await this.repoContenido.ObtieneVersion(elemento.VersionId).ConfigureAwait(false);
            if (vElemento == null)
            {
                return NotFound();
            }

            foreach (var parteId in ids.ToList())
            {
                var parte = vElemento.Partes.Where(x => x.Id == parteId).SingleOrDefault();
                if (parte != null)
                {
                    IGestorES gestor = await servicioVol.ObtienInstanciaGestor(elemento.VolumenId)
                   .ConfigureAwait(false);

                    var resultado = await gestor.ReflejarImagen(elemento.Id, parte.Id, vElemento.Id, elemento.VolumenId, parte.Extension, direccion).ConfigureAwait(false);
                    if (!string.IsNullOrEmpty(resultado))
                    {
                        parte.Indexada = false;
                        vElemento.EstadoIndexado = EstadoIndexado.PorIndexar;
                    }
                }
            }
            
            await repoContenido.ActualizaVersion(vElemento.Id, vElemento, true).ConfigureAwait(false);
            await servicioEntidad.EventoActualizarVersionElemento(AplicacionContenido.EventosAdicionales.ReflejarPaginas.GetHashCode(), id, true, null, Newtonsoft.Json.JsonConvert.SerializeObject(ids.ToList())).ConfigureAwait(false);
            return Ok();
        }


        [HttpPost("paginas/{id}/mover/{posicion}")]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        public async Task<ActionResult> MoverPaginas(string id, int posicion, [FromBody] int[] paginas)
        {

            string v = "";
            List<int> ps = paginas.ToList();
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

            List<Parte> Fijas = new List<Parte>();
            List<Parte> AMover = new List<Parte>();
            vElemento.Partes = vElemento.Partes.OrderBy(p => p.Indice).ToList();
            foreach (var p in vElemento.Partes)
            {
                if (ps.IndexOf(p.Indice) >= 0)
                {
                    AMover.Add(p);
                }
                else
                {
                    Fijas.Add(p);
                }
            }

            int total = Fijas.Count;
            bool inserted = false;
            if (posicion >= vElemento.Partes.Max(x => x.Indice))
            {
                inserted = true;
                for (int j = 0; j < AMover.Count; j++)
                {
                    Fijas.Insert(Fijas.Count, AMover[j]);
                }
            }
            else
            {
                for (int i = 0; i < total; i++)
                {
                    if (Fijas[i].Indice == posicion)
                    {
                        inserted = true;
                        for (int j = AMover.Count - 1; j >= 0; j--)
                        {
                            Fijas.Insert(i, AMover[j]);
                        }
                        break;
                    }
                }
            }


            if (!inserted)
            {
                int i = Fijas.Count;
                for (int j = AMover.Count - i; j >= 0; j--)
                {
                    Fijas.Insert(i, AMover[j]);
                }
            }

            int newIndex = 1;
            foreach (var f in Fijas)
            {
                f.Indice = newIndex;
                newIndex++;
            }

            vElemento.Partes = Fijas;
            
            await repoContenido.ActualizaVersion(vElemento.Id, vElemento, true).ConfigureAwait(false);
            await servicioEntidad.EventoActualizarVersionElemento(AplicacionContenido.EventosAdicionales.MoverPaginas.GetHashCode(), id, true, null, Newtonsoft.Json.JsonConvert.SerializeObject(paginas.ToList())).ConfigureAwait(false);
            return Ok();
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

            int idx = 1;
            foreach(var p in vElemento.Partes)
            {
                p.Indice = idx;
                idx++;
            }

            await repoContenido.ActualizaVersion(elemento.VersionId, vElemento, false).ConfigureAwait(false);
            await servicioEntidad.EventoActualizarVersionElemento(AplicacionContenido.EventosAdicionales.EliminarPaginas.GetHashCode(), id, true, null, Newtonsoft.Json.JsonConvert.SerializeObject(paginas)).ConfigureAwait(false);
            return Ok();
        }

        [HttpPost("zip/{id}/{v}", Name = "GeneraZip")]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> CrearZIP(string id, string v)
        {
            string controller = "contenido/elemento";
            string version = "";
            if (this.ControllerContext.HttpContext.Request.RouteValues.ContainsKey("version"))
            {
                version = (string)this.ControllerContext.HttpContext.Request.RouteValues.GetValueOrDefault("version");
            }

            string Etiqueta = "";
            var e = await this.servicioEntidad.UnicoAsync(x => x.Id == id).ConfigureAwait(false);
            if (e == null)
            {
                return NotFound("elemento-inexistente");
            }
                    
            Etiqueta = $"ZIP {e.Nombre}";
            Servicio.Contenido.TareasEnDemanda tareas = new Servicio.Contenido.TareasEnDemanda();
            var tzip = tareas.ObtieneTarea(Servicio.Contenido.TareasEnDemanda.TAREA_EXPPORTAR_ZIP);
            if (tzip != null)
            {
                InputPayloadTareaExportarZIP input = new InputPayloadTareaExportarZIP() { ElementoId = id };

                var enEjecucion = (await tareaEnDemanda.TareasPendientesUsuario(this.UsuarioId, this.DominioId, this.TenantId).ConfigureAwait(false))
                    .Where(x => x.TareaProcesoId == tzip.Id).ToList();

                foreach (var tex in enEjecucion)
                {
                    if (tex.TareaProcesoId == tzip.Id)
                    {
                        var inputtex = JsonSerializer.Deserialize<InputPayloadTareaExportarZIP>(tex.InputPayload);
                        if (inputtex.ElementoId == id)
                        {
                            return BadRequest("tarea-existente");
                        }
                    }
                }


                Guid Id = Guid.NewGuid();
                TareaEnDemanda t = new TareaEnDemanda()
                {
                    Completada = false,
                    DominioId = this.DominioId,
                    FechaCreacion = DateTime.UtcNow,
                    Id = Id,
                    InputPayload = JsonSerializer.Serialize(input),
                    Prioridad = tzip.Prioridad,
                    TenantId = this.TenantId,
                    Recogida = false,
                    TipoRespuesta = tzip.TipoRespuesta,
                    UsuarioId = this.UsuarioId,
                    NombreEnsamblado = tzip.NombreEnsamblado,
                    TareaProcesoId = tzip.Id,
                    FechaCaducidad = null,
                    FechaEjecucion = null,
                    Error = null,
                    URLRecoleccion = $"{controller}/zip/{Id}",
                    HorasCaducidad = tzip.HorasCaducidad,
                    OutputPayload = null,
                    Etiqueta = Etiqueta
                };

                await this.tareaEnDemanda.CrearAsync(t).ConfigureAwait(false);

                ComunTreas.PostTareaEnDemanda post = new ComunTreas.PostTareaEnDemanda()
                {
                    Fecha = DateTime.UtcNow,
                    Id = t.Id.ToString(),
                    PickupURL = t.URLRecoleccion,
                    Tipo = Servicio.Contenido.TareasEnDemanda.TAREA_EXPPORTAR_ZIP,
                    TipoRespuesta = t.TipoRespuesta,
                    Etiqueta = Etiqueta
                };

                await servicioEntidad.EventoActualizarVersionElemento(AplicacionContenido.EventosAdicionales.ExportarZIP.GetHashCode(), id).ConfigureAwait(false);
                return Ok(post);
            }

            return BadRequest();

        }

        [HttpGet("zip/{id}", Name = "RecogerZIP")]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetZIP(string id)
        {
            if (!Guid.TryParse(id, out Guid tareaId))
            {
                return BadRequest();
            }

            var t = await tareaEnDemanda.UnicoAsync(x => x.Id == tareaId).ConfigureAwait(false);

            if (t == null || !t.Completada)
            {
                return NotFound();
            }

            OtputPayloadTareaExportarZIP output = JsonSerializer.Deserialize<OtputPayloadTareaExportarZIP>(t.OutputPayload);
            if (!string.IsNullOrEmpty(output.RutaZIP) && System.IO.File.Exists(output.RutaZIP))
            {
                System.IO.FileInfo fi = new System.IO.FileInfo(output.RutaZIP);
                HttpContext.Response.Headers.Add("Content-Length", $"{fi.Length}");
                HttpContext.Response.Headers.Add("Access-Control-Expose-Headers", "Content-Disposition");
                return PhysicalFile(output.RutaZIP, MimeTypes.GetMimeType($"{output.NombreElemento}.zip"), output.NombreElemento + ".zip");

            }
            else
            {
                return NotFound("no-localidazo");
            }

        }


        [HttpGet("pdf/{id}", Name = "RecogerPDF")]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetPDF(string id)
        {
            if (!Guid.TryParse(id, out Guid tareaId))
            {
                return BadRequest();
            }

            var t = await tareaEnDemanda.UnicoAsync(x => x.Id == tareaId).ConfigureAwait(false);

            if (t == null || !t.Completada)
            {
                return NotFound();
            }

            OtputPayloadTareaExportarPDF output = JsonSerializer.Deserialize<OtputPayloadTareaExportarPDF>(t.OutputPayload);
            if (!string.IsNullOrEmpty(output.RutaPDF) && System.IO.File.Exists(output.RutaPDF))
            {
                System.IO.FileInfo fi = new System.IO.FileInfo(output.RutaPDF);
                HttpContext.Response.Headers.Add("Content-Length", $"{fi.Length}");
                HttpContext.Response.Headers.Add("Access-Control-Expose-Headers", "Content-Disposition");
                return PhysicalFile(output.RutaPDF, MimeTypes.GetMimeType($"{output.NombreElemento}.pdf"), output.NombreElemento + ".pdf");

            }
            else
            {
                return NotFound("no-localidazo");
            }

        }



        /// <summary>
        /// Adiciona un trabajo de generacín de PDF
        /// </summary>
        /// <param name="id">Identificador del elemento de contenido</param>
        /// <param name="v">Versión del elemento de contenido</param>
        /// <param name="p">Porcentaje del escalamiento para las imágenes 20-100</param>
        /// <returns></returns>
        [HttpPost("pdf/{id}/{v}/{p}", Name = "GeneraPDF")]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<string>> CrearPDF(string id, string v, int p)
        {

            string controller = "contenido/elemento";
            string version = "";
            if (this.ControllerContext.HttpContext.Request.RouteValues.ContainsKey("version"))
            {
                version = (string)this.ControllerContext.HttpContext.Request.RouteValues.GetValueOrDefault("version");
            }

            string Etiqueta = "";
            var e = await this.servicioEntidad.UnicoAsync(x => x.Id == id).ConfigureAwait(false);
            if (e == null)
            {
                return NotFound("elemento-inexistente");
            }

            if (p < 20)
            {
                p = 20;
            }

            if (p > 100)
            {
                p = 100;
            }

            Etiqueta = $"PDF {e.Nombre}";
            Servicio.Contenido.TareasEnDemanda tareas = new Servicio.Contenido.TareasEnDemanda();
            var tpdf = tareas.ObtieneTarea(Servicio.Contenido.TareasEnDemanda.TAREA_EXPPORTAR_PDF);
            if (tpdf != null)
            {
                InputPayloadTareaExportarPDF input = new InputPayloadTareaExportarPDF() { ElementoId = id, PorcentajePorcientoEscala = p };

                var enEjecucion = (await tareaEnDemanda.TareasPendientesUsuario(this.UsuarioId, this.DominioId, this.TenantId).ConfigureAwait(false))
                    .Where(x => x.TareaProcesoId == tpdf.Id).ToList();

                foreach (var tex in enEjecucion)
                {
                    if (tex.TareaProcesoId == tpdf.Id)
                    {
                        var inputtex = JsonSerializer.Deserialize<InputPayloadTareaExportarPDF>(tex.InputPayload);
                        if (inputtex.ElementoId == id)
                        {
                            return BadRequest("tarea-existente");
                        }
                    }
                }


                Guid Id = Guid.NewGuid();
                TareaEnDemanda t = new TareaEnDemanda()
                {
                    Completada = false,
                    DominioId = this.DominioId,
                    FechaCreacion = DateTime.UtcNow,
                    Id = Id,
                    InputPayload = JsonSerializer.Serialize(input),
                    Prioridad = tpdf.Prioridad,
                    TenantId = this.TenantId,
                    Recogida = false,
                    TipoRespuesta = tpdf.TipoRespuesta,
                    UsuarioId = this.UsuarioId,
                    NombreEnsamblado = tpdf.NombreEnsamblado,
                    TareaProcesoId = tpdf.Id,
                    FechaCaducidad = null,
                    FechaEjecucion = null,
                    Error = null,
                    URLRecoleccion = $"{controller}/pdf/{Id}",
                    HorasCaducidad = tpdf.HorasCaducidad,
                    OutputPayload = null,
                    Etiqueta = Etiqueta
                };

                await this.tareaEnDemanda.CrearAsync(t).ConfigureAwait(false);

                ComunTreas.PostTareaEnDemanda post = new ComunTreas.PostTareaEnDemanda()
                {
                    Fecha = DateTime.UtcNow,
                    Id = t.Id.ToString(),
                    PickupURL = t.URLRecoleccion,
                    Tipo = Servicio.Contenido.TareasEnDemanda.TAREA_EXPPORTAR_PDF,
                    TipoRespuesta = t.TipoRespuesta,
                    Etiqueta = Etiqueta
                };
                await servicioEntidad.EventoActualizarVersionElemento(AplicacionContenido.EventosAdicionales.ExportarPDF.GetHashCode(), id).ConfigureAwait(false);
                return Ok(post);
            }

            return BadRequest();

        }

    }
}