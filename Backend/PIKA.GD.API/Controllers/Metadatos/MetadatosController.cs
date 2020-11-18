using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PIKA.GD.API.Servicios;
using PIKA.Modelo.Metadatos;
using System;
using PIKA.Servicio.Metadatos.ElasticSearch.Excepciones;
using PIKA.GD.API.Filters;
using PIKA.Modelo.Metadatos.Extractores;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;
using PIKA.Servicio.Metadatos.Interfaces;
using LazyCache;
using PIKA.GD.API.Servicios.Caches;
using RepositorioEntidades;
using PIKA.Modelo.Metadatos.Instancias;

namespace PIKA.GD.API.Controllers.Metadatos
{
    [Authorize]
    [ApiVersion("1.0")]
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class MetadatosController : ACLController
    {
        private ILogger<MetadatosController> logger;
        private readonly IRepositorioMetadatos repositorio;
        private readonly IAppCache appCache;
        private readonly IServicioPlantilla plantillas;
        public MetadatosController(
            IRepositorioMetadatos repositorio,
            IServicioPlantilla plantillas,
            ILogger<MetadatosController> logger,
            IAppCache cache
            )
        {
            this.plantillas = plantillas;
            this.repositorio = repositorio;
            this.logger = logger;
            this.appCache = cache;
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
            try
            {
                Plantilla plantilla = await CacheMetadatos.ObtienePlantillaPorId(id,appCache, plantillas)
                    .ConfigureAwait(false);
                
                if (plantilla == null) return NotFound(id);

                PlantillaMetadataExtractor extractor = new PlantillaMetadataExtractor();
                return Ok(extractor.Obtener(plantilla));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }


        /// <summary>
        /// Inserta un registro de metadatos para un objeto
        /// </summary>
        /// <param name="plantillaid">Indetificador único de la plantilla</param>
        /// <param name="tipo">Tipo de objeto asociado por ejemplo Documento</param>
        /// <param name="tipoid">Identificador único del objeto asociado por ejemplo Id Documento </param>
        /// <param name="valores">Lista de valores para el registro de metadatos</param>
        /// <returns></returns>
        [HttpPost("{plantillaid}/{tipo}/{tipoid}")]
        [TypeFilter(typeof(AsyncIdentityFilter))]
        public async Task<ActionResult> Inserta(string plantillaid, string tipo, string tipoid, 
            [FromBody] RequestValoresPlantilla valores)
        {
            try
            {

                Plantilla plantilla = await CacheMetadatos.ObtienePlantillaPorId(plantillaid, appCache, plantillas)
                        .ConfigureAwait(false);

                if (plantilla == null) return NotFound(plantillaid);

                bool existe = await PLantillaGenerada(plantilla).ConfigureAwait(false);
                if (existe)
                {
                    string id = await repositorio.Inserta(tipo, tipoid, "dominio", this.TenatId, 
                        false, null, plantilla, valores).ConfigureAwait(false);
                    if(id !=null) return Ok(id);
                }

                return BadRequest(valores);
            }
            catch (ExMetadatosNoValidos em)
            {
                return BadRequest(em.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }

        }

        [HttpPost("lista/{plantillaid}/{tipo}/{tipoid}/{listaid}")]
        [TypeFilter(typeof(AsyncIdentityFilter))]
        public async Task<ActionResult> InsertaEnLista(string plantillaid, string tipo, string tipoid,
            string listaid, [FromBody] RequestValoresPlantilla valores)
        {
            try
            {

                Plantilla plantilla = await CacheMetadatos.ObtienePlantillaPorId(plantillaid, appCache, plantillas)
                        .ConfigureAwait(false);

                if (plantilla == null) return NotFound(plantillaid);

                bool existe = await PLantillaGenerada(plantilla).ConfigureAwait(false);
                if (existe)
                {
                    string id = await repositorio.Inserta(tipo, tipoid, "dominio", this.TenatId,
                        true, listaid, plantilla, valores).ConfigureAwait(false);
                    if (id != null) return Ok(id);
                }

                return BadRequest(valores);
            }
            catch (ExMetadatosNoValidos em)
            {
                return BadRequest(em.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }



        /// <summary>
        /// Actualiza un registro correspondiente a los datos de una plantilla
        /// </summary>
        /// <param name="id"></param>
        /// <param name="plantillaid"></param>
        /// <param name="valores"></param>
        /// <returns></returns>
        [HttpPut("{plantillaid}/{id}")]
        [TypeFilter(typeof(AsyncIdentityFilter))]
        public async Task<ActionResult> Actualiza(string id, string plantillaid,
            [FromBody] RequestValoresPlantilla valores)
        {

            try
            {

                Plantilla plantilla = await CacheMetadatos.ObtienePlantillaPorId(plantillaid, appCache, plantillas)
                         .ConfigureAwait(false);

                if (plantilla == null) return NotFound(plantillaid);

                bool existe = await PLantillaGenerada(plantilla).ConfigureAwait(false);
                if (existe)
                {
                    bool r = await repositorio.Actualiza(id, plantilla, valores).ConfigureAwait(false);
                    if (r) return Ok();
                    return BadRequest(valores);
                }

                return NotFound($"{plantillaid}/{id}");
            }
            catch (ExMetadatosNoValidos em)
            {
                return BadRequest(em.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }

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
            try
            {
                DocumentoPlantilla p = new DocumentoPlantilla();
                Plantilla plantilla = await CacheMetadatos.ObtienePlantillaPorId(plantillaid, appCache, plantillas)
                         .ConfigureAwait(false);

                if (plantilla == null) return NotFound(plantillaid);

                bool existe = await PLantillaGenerada(plantilla).ConfigureAwait(false);
                if (existe)
                {
                    var r = await repositorio.Unico(plantilla, id).ConfigureAwait(false);
                    if (r!=null) return Ok(r);
                    
                }

                return NotFound($"{plantillaid}/{id}");
                
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }

        }

        /// <summary>
        /// Obntiene la lista de metadatos asociados a una lista de plantillas
        /// </summary>
        /// <param name="id"></param>
        /// <param name="plantillaid"></param>
        /// <returns></returns>
        [HttpGet("lista/{plantillaid}/{id}")]
        [TypeFilter(typeof(AsyncIdentityFilter))]
        public async Task<ActionResult<DocumentoPlantilla>> Lista(string id, string plantillaid)
        {
            try
            {
                DocumentoPlantilla p = new DocumentoPlantilla();
                Plantilla plantilla = await CacheMetadatos.ObtienePlantillaPorId(plantillaid, appCache, plantillas)
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
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }

        }


        /// <summary>
        /// Elimina el documento asociado a un identificador de la plantilla
        /// </summary>
        /// <param name="id">Identificador único del documento</param>
        /// <param name="plantillaid">Identificador único de la plantilla a la que pertenece el documento</param>
        /// <returns></returns>
        [HttpDelete("{plantillaid}/{id}")]
        [TypeFilter(typeof(AsyncIdentityFilter))]
        public async Task<ActionResult<string>> Eliminar(string id, string plantillaid)
        {
            try
            {
                DocumentoPlantilla p = new DocumentoPlantilla();
                Plantilla plantilla = await CacheMetadatos.ObtienePlantillaPorId(plantillaid, appCache, plantillas)
                         .ConfigureAwait(false);
                
                if (plantilla == null) return NotFound(plantillaid);

                bool respuesta = await repositorio.EliminaDocumento(id, plantillaid).ConfigureAwait(false);
                if (respuesta) return Ok(id);
                
                return NotFound(id);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }

        }

        /// <summary>
        /// Elimina todos los documentos de una lista documenos en  una plantilla
        /// </summary>
        /// <param name="id">Identificador único de la lista de documentos</param>
        /// <param name="plantillaid">identificador único de la plantilla</param>
        /// <returns></returns>
        [HttpDelete("lista/{plantillaid}/{id}")]
        [TypeFilter(typeof(AsyncIdentityFilter))]
        public async Task<ActionResult<string>> EliminarLista(string id, string plantillaid)
        {
            try
            {
                DocumentoPlantilla p = new DocumentoPlantilla();
                Plantilla plantilla = await CacheMetadatos.ObtienePlantillaPorId(plantillaid, appCache, plantillas)
                         .ConfigureAwait(false);

                if (plantilla == null) return NotFound(plantillaid);

                long respuesta = await repositorio.EliminaListaDocumentos(id, plantillaid).ConfigureAwait(false);
                if (respuesta>=0) return Ok(respuesta);

                return NotFound(id);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }

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
            var data = await plantillas.ObtenerAsync(x=>x.Eliminada==false).ConfigureAwait(false);
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
        /// Ontiene los vinculos a plantillas par un tipo de objeto e identificador único del mismo
        /// </summary>
        /// <param name="tipo">Identificador único del tipo de objeto al que se asocian los metadatos</param>
        /// <param name="id">Identificaodor único del objeto</param>
        /// <returns></returns>
        [HttpGet("{tipo}/{id}", Name = "ObtieneVinculosPlantillas")]
        [TypeFilter(typeof(AsyncIdentityFilter))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<VinculosObjetoPlantilla>> ObtieneVinculosPlantillas(string tipo, string id)
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