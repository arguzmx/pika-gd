using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PIKA.Modelo.Metadatos;
using PIKA.GD.API.Filters;
using PIKA.Modelo.Metadatos.Extractores;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;
using PIKA.Servicio.Metadatos.Interfaces;
using LazyCache;
using PIKA.GD.API.Servicios.Caches;
using RepositorioEntidades;
using PIKA.Modelo.Metadatos.Instancias;
using PIKA.Servicio.Metadatos.ElasticSearch.modelos;
using Microsoft.Extensions.Options;
using PIKA.Infraestructura.Comun;
using PIKA.Infraestructura.Comun.Seguridad;

namespace PIKA.GD.API.Controllers.Metadatos
{
    [Authorize]
    [ApiVersion("1.0")]
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class MetadatosController : ACLController
    {
        private readonly IRepositorioMetadatos repositorio;
        private readonly IAppCache appCache;
        private readonly IServicioPlantilla plantillas;
        private readonly ConfiguracionServidor config;
        public MetadatosController(
            IOptions<ConfiguracionServidor> config,
            IRepositorioMetadatos repositorio,
            IServicioPlantilla plantillas,
            ILogger<MetadatosController> logger,
            IAppCache cache
            )
        {
            this.plantillas = plantillas;
            this.repositorio = repositorio;
            this.appCache = cache;
            this.config = config.Value;
        }


        public override void EmiteConfiguracionSeguridad(UsuarioAPI usuario, ContextoRegistroActividad RegistroActividad, List<EventoAuditoriaActivo> Eventos)
        {
            plantillas.EstableceContextoSeguridad(usuario, RegistroActividad, Eventos);
        }


        /// <summary>
        /// Obtiene una plantilla a partir de su identificador único
        /// </summary>
        /// <param name="id">Identificador único de la plantilla</param>
        /// <returns></returns>
        [HttpGet("{id}", Name = "ObtienePlantilla")]
        [TypeFilter(typeof(AsyncIdentityFilter))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<MetadataInfo>> ObtienePlantilla(string id)
        {
          
                Plantilla plantilla = await CacheMetadatos.ObtienePlantillaPorId(id, appCache, plantillas, config.seguridad_cache_segundos)
                .ConfigureAwait(false);

                if (plantilla == null) return NotFound(id);

                PlantillaMetadataExtractor extractor = new PlantillaMetadataExtractor();

                return Ok(extractor.Obtener(plantilla));
        }


        /// <summary>
        /// Ontiene la lista de plantillas correspondiente a laa unidad organizacional
        /// </summary>
        /// <returns></returns>
        [HttpGet("plantillas", Name = "ObtienePlantillas")]
        [TypeFilter(typeof(AsyncIdentityFilter))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<List<ValorListaOrdenada>>> GetListaPlantillas()
        {
            var data = await plantillas.ObtenerAsync(x => x.Eliminada == false).ConfigureAwait(false);
            List<ValorListaOrdenada> planbtillas = new List<ValorListaOrdenada>();
            data.ForEach(p =>
            {
                planbtillas.Add(new ValorListaOrdenada()
                {
                    Id = p.Id,
                    Indice = 0,
                    Texto = p.Nombre
                });
            });
            return Ok(planbtillas);
        }




        /// <summary>
        /// Inserta un registro de datos para la plantilla del documento
        /// </summary>
        /// <param name="plantillaid">Identificador único de la plantilla</param>
        /// <param name="valores">Valores para el documento de plantilla</param>
        /// <returns></returns>
        [HttpPost("{plantillaid}")]
        [TypeFilter(typeof(AsyncIdentityFilter))]
        public async Task<ActionResult<DocumentoPlantilla>> Inserta(string plantillaid,
            [FromBody] RequestValoresPlantilla valores)
        {

            Plantilla plantilla = await CacheMetadatos.ObtienePlantillaPorId(plantillaid, appCache, plantillas, config.seguridad_cache_segundos)
                    .ConfigureAwait(false);

            if (plantilla == null) return NotFound(plantillaid);

            bool existe = await PLantillaGenerada(plantilla).ConfigureAwait(false);
            if (existe)
            {
                DocumentoPlantilla documento = await repositorio.Inserta("dominio", this.TenantId,
                    false, null, plantilla, valores, "").ConfigureAwait(false);
                if (documento != null) return Ok(documento);
            }

            return BadRequest(valores);

        }



        [HttpPost("{plantillaid}/lista/")]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        public async Task<ActionResult<string>> CreaLista(string plantillaid, [FromBody] RequestCrearLista request)
        {
            string id = await repositorio.CreaLista(plantillaid, request).ConfigureAwait(false);
            return Ok(System.Text.Json.JsonSerializer.Serialize(id));
        }


        /// <summary>
        /// Inserta un registro de datos en la listaa para la plantilla del documento
        /// </summary>
        /// <param name="plantillaid">Identificador único de la plantilla</param>
        /// <param name="listaid">Identificador único de la lista, si no existe es creado</param>
        /// <param name="valores">Valores para el documento de plantilla</param>
        /// <returns></returns>
        [HttpPost("{plantillaid}/lista/{listaid}")]
        [TypeFilter(typeof(AsyncIdentityFilter))]
        public async Task<ActionResult> InsertaEnLista(string plantillaid,
            string listaid, [FromBody] RequestValoresPlantilla valores)
        {
            Plantilla plantilla = await CacheMetadatos.ObtienePlantillaPorId(plantillaid, appCache, plantillas, config.seguridad_cache_segundos)
                    .ConfigureAwait(false);

            if (plantilla == null) return NotFound(plantillaid);

            bool existe = await PLantillaGenerada(plantilla).ConfigureAwait(false);
            if (existe)
            {
                var documento = await repositorio.Inserta("dominio", this.TenantId,
                    true, listaid, plantilla, valores, "").ConfigureAwait(false);
                if (documento != null) return Ok(documento);
            }
            return BadRequest(valores);
        }



        /// <summary>
        /// Actualiza un registro correspondiente a los datos de una plantilla
        /// </summary>
        /// <param name="id">Identificador único del documento de plantilla</param>
        /// <param name="plantillaid">Identificador único de la plantilla</param>
        /// <param name="valores">Valores del documento, sólo se actualizan los valores y el índice filtrado</param>
        /// <returns></returns>
        [HttpPut("{plantillaid}/{id}")]
        [TypeFilter(typeof(AsyncIdentityFilter))]
        public async Task<ActionResult> Actualiza(string id, string plantillaid,
            [FromBody] RequestValoresPlantilla valores)
        {

            Plantilla plantilla = await CacheMetadatos.ObtienePlantillaPorId(plantillaid, appCache, plantillas, config.seguridad_cache_segundos)
                     .ConfigureAwait(false);

            if (plantilla == null) return NotFound(plantillaid);

            bool existe = await PLantillaGenerada(plantilla).ConfigureAwait(false);
            if (existe)
            {
                bool r = await repositorio.Actualiza(id, plantilla, valores).ConfigureAwait(false);
                if (r) return NoContent();
                return BadRequest(valores);
            }

            return NotFound($"{plantillaid}/{id}");

        }

        /// <summary>
        /// OBtiene los valores de un documento de plantilla para un id unio
        /// </summary>
        /// <param name="id">Indetificador único del elemento indexado</param>
        /// <param name="plantillaid">Indetificador único de la plantilla</param>
        /// <returns></returns>
        [HttpGet("{plantillaid}/{id}")]
        [TypeFilter(typeof(AsyncIdentityFilter))]
        public async Task<ActionResult<DocumentoPlantilla>> Unico(string id, string plantillaid)
        {
            DocumentoPlantilla p = new DocumentoPlantilla();
            Plantilla plantilla = await CacheMetadatos.ObtienePlantillaPorId(plantillaid, appCache, plantillas, config.seguridad_cache_segundos)
                     .ConfigureAwait(false);

            if (plantilla == null) return NotFound(plantillaid);

            bool existe = await PLantillaGenerada(plantilla).ConfigureAwait(false);
            if (existe)
            {
                var r = await repositorio.Unico(plantilla, id).ConfigureAwait(false);
                if (r != null) return Ok(r);

            }

            return NotFound($"{plantillaid}/{id}");

        }



        /// <summary>
        /// Obntiene la lista de metadatos asociados a una lista de plantillas
        /// </summary>
        /// <param name="id"></param>
        /// <param name="plantillaid"></param>
        /// <returns></returns>
        [HttpGet("{plantillaid}/lista/{id}")]
        [TypeFilter(typeof(AsyncIdentityFilter))]
        public async Task<ActionResult<DocumentoPlantilla>> Lista(string id, string plantillaid)
        {
            DocumentoPlantilla p = new DocumentoPlantilla();
            Plantilla plantilla = await CacheMetadatos.ObtienePlantillaPorId(plantillaid, appCache, plantillas, config.seguridad_cache_segundos)
                     .ConfigureAwait(false);

            if (plantilla == null) return NotFound(plantillaid);

            bool existe = await PLantillaGenerada(plantilla).ConfigureAwait(false);
            if (existe)
            {
                var r = await repositorio.Lista(plantilla, id).ConfigureAwait(false);
                if (r != null) return Ok(r);

            }

            return NotFound($"{plantillaid}/{id}");

        }

        [HttpPost("{plantillaid}/lista/{tipo}/id")]
        [TypeFilter(typeof(AsyncIdentityFilter))]
        public async Task<ActionResult<DocumentoPlantilla>> ListPorIdTpo(string plantillaid, string tipo, [FromBody]  RequestPlantillaTipo  request)
        {

            Plantilla plantilla = await CacheMetadatos.ObtienePlantillaPorId(plantillaid, appCache, plantillas, config.seguridad_cache_segundos)
                     .ConfigureAwait(false);

            if (plantilla == null) return NotFound(plantillaid);

            bool existe = await PLantillaGenerada(plantilla).ConfigureAwait(false);
            if (existe)
            {
                var r = await repositorio.ListaTipoIds(plantilla, request.Ids , tipo).ConfigureAwait(false);
                if (r != null) return Ok(r);

            }

            return NotFound();

        }



        [HttpPost("{plantillaid}/pagina")]
        [TypeFilter(typeof(AsyncIdentityFilter))]
        public async Task<ActionResult<Paginado<DocumentoPlantilla>>> PaginaPlantilla(string plantillaid, [FromBody] ConsultaAPI q)
        {
            Plantilla plantilla = await CacheMetadatos.ObtienePlantillaPorId(plantillaid, appCache, plantillas, config.seguridad_cache_segundos)
                     .ConfigureAwait(false);

            if (plantilla == null) return NotFound(plantillaid);
            bool existe = await PLantillaGenerada(plantilla).ConfigureAwait(false);

            if (existe)
            {
                var r = await repositorio.PaginadoDocumentoPlantilla(q, plantilla, "","").ConfigureAwait(false);
                if (r != null) return Ok(r);
            }

            return NotFound();
        }


        /// <summary>
        /// Elimina el documento asociado a un identificador de la plantilla
        /// </summary>
        /// <param name="tipo">Tipo del objeto asociado a la plantilla</param>
        /// <param name="id">Identificador único del objeto asociado a la plantilla</param>
        /// <param name="plantillaid">Identificador único de la plantilla a la que pertenece el documento</param>
        /// <param name="docid">Identificador único del documento</param>
        /// <returns></returns>
        [HttpDelete("{plantillaid}/{docid}")]
        [TypeFilter(typeof(AsyncIdentityFilter))]
        public async Task<ActionResult<string>> Eliminar(string docid, string plantillaid)
        {
            await repositorio.EliminaDocumento(docid, plantillaid).ConfigureAwait(false);
            return Ok(System.Text.Json.JsonSerializer.Serialize(docid));
        }

        /// <summary>
        /// Elimina todos los documentos de una lista documenos en  una plantilla
        /// </summary>
        /// <param name="id">Identificador único de la lista de documentos</param>
        /// <param name="plantillaid">identificador único de la plantilla</param>
        /// <returns></returns>
        [HttpDelete("{plantillaid}/lista/{listaid}")]
        [TypeFilter(typeof(AsyncIdentityFilter))]
        public async Task<ActionResult<string>> EliminarLista(string listaid, string plantillaid)
        {
            long respuesta = await repositorio.EliminaListaDocumentos(listaid, plantillaid).ConfigureAwait(false);
            return Ok(System.Text.Json.JsonSerializer.Serialize(respuesta));

        }




        /// <summary>
        /// Ontiene los vinculos a plantillas par un tipo de objeto e identificador único del mismo
        /// </summary>
        /// <param name="tipo">Identificador único del tipo de objeto al que se asocian los metadatos</param>
        /// <param name="id">Identificaodor único del objeto</param>
        /// <returns></returns>
        [HttpGet("vinculos/{tipo}/{id}", Name = "ObtieneVinculosPlantillas")]
        [TypeFilter(typeof(AsyncIdentityFilter))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<VinculosObjetoPlantilla>> ObtieneVinculosPlantillas(string tipo, string id)
        {

            var vinculos = await repositorio.ObtieneVinculos(tipo, id).ConfigureAwait(false);
            return Ok(vinculos);
        }



        /// <summary>
        ///  Ruttina auxiliar para deteminar si el índice correspondiente a un plantilla está creado
        /// </summary>
        /// <param name="plantilla">Identificador único de la plantilla</param>
        /// <returns></returns>

        private async Task<bool> PLantillaGenerada(Plantilla plantilla)
        {
            bool existe = await CacheMetadatos.PlantillaGenerada(plantilla.Id, appCache).ConfigureAwait(false);
            if (!existe)
            {
                // si no esta en cache
                //Verifcica en el repositorio
                existe = await repositorio.ExisteIndice(plantilla.Id).ConfigureAwait(false);
                if (!existe)
                {
                    // Crea la plantilla si no existe
                    await repositorio.CrearIndice(plantilla).ConfigureAwait(false);
                    // y vuelve a consultar
                    existe = await repositorio.ExisteIndice(plantilla.Id).ConfigureAwait(false);
                }

                // Si exite o fue creada lo marca en el cache
                if (existe) CacheMetadatos.EstablecePlantillaGenerada(plantilla.Id, appCache);
            }

            return existe;
        }

    }



}