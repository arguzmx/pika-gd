using LazyCache;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PIKA.GD.API.Filters;
using PIKA.GD.API.Servicios.Caches;
using PIKA.Modelo.Metadatos;
using PIKA.Modelo.Metadatos.Extractores;
using PIKA.Servicio.Contenido.ElasticSearch;
using PIKA.Servicio.Metadatos.Interfaces;
using System;
using System.Threading.Tasks;

namespace PIKA.GD.API.Controllers.Contenido
{
    [Authorize]
    [ApiVersion("1.0")]
    [ApiController]
    [Route("api/v{version:apiVersion}/contenido/[controller]")]
    public class BusquedaController : ControllerBase
    {
        IRepoContenidoElasticSearch repoContenido;
        private readonly IAppCache appCache;
        private IServicioPlantilla plantillas;

        //ServicioBiusquedaElemento
        //ServicioBusquedaCarpeta
        public BusquedaController(
            IRepoContenidoElasticSearch repoContenido,
            IAppCache cache,
            IServicioPlantilla plantillas) {
            this.repoContenido = repoContenido;
            this.appCache = cache;
            this.plantillas = plantillas;
        }


        /// <summary>
        /// Obtiene una plantilla a partir de su identificador único
        /// </summary>
        /// <param name="id">Identificador único de la plantilla</param>
        /// <returns></returns>
        [HttpGet("{id}", Name = "ObtienePlantillaBusquedaContenido")]
        [TypeFilter(typeof(AsyncIdentityFilter))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<MetadataInfo>> ObtienePlantilla(string id)
        {
            try
            {
                Plantilla plantilla = await CacheMetadatos.ObtienePlantillaPorId(id, appCache, plantillas)
                .ConfigureAwait(false);

                if (plantilla == null) return NotFound(id);

                PlantillaMetadataExtractor extractor = new PlantillaMetadataExtractor();

                return Ok(extractor.Obtener(plantilla));
            }
            catch (Exception ex)
            {

                Console.WriteLine($"{ex}");
                throw;
            }


        }

    }
}
