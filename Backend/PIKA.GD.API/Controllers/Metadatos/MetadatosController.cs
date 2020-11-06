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
        public async Task<ActionResult> Inserta(string plantillaid, string tipo, string tipoid, 
            [FromBody] RequestValoresPlantilla valores)
        {
            try
            {
                

                ValoresPlantilla valoresplantilla = new ValoresPlantilla() {
                    DatoId = tipoid,
                    TipoDatoId = tipo,
                    PlantillaId = plantillaid,
                    TipoOrigenId = "dominio",
                    OrigenId = this.DominioId,
                    Valores = valores.Valores, 
                    IndiceFiltrado = valores.Filtro
                };


                Plantilla plantilla = await CacheMetadatos.ObtienePlantillaPorId(plantillaid, appCache, plantillas)
                        .ConfigureAwait(false);

                if (plantilla == null) return NotFound(valoresplantilla.PlantillaId);

                bool existe = await PLantillaGenerada(plantilla).ConfigureAwait(false);
                if (existe)
                {
                    string id = await repositorio.Inserta(plantilla, valoresplantilla).ConfigureAwait(false);
                    return Ok(id);
                }

                return NotFound(valoresplantilla.PlantillaId);
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
        /// Inserta un registro de metadatos para un objeto
        /// </summary>
        /// <param name="id">Indetificador único del elemento indexado</param>
        /// <param name="plantillaid">Indetificador único de la plantilla</param>
        /// <param name="tipo">Tipo de objeto asociado por ejemplo Documento</param>
        /// <param name="tipoid">Identificador único del objeto asociado por ejemplo Id Documento </param>
        /// <param name="valores">Lista de valores para el registro de metadatos</param>
        /// <returns></returns>
        [HttpPut("{plantillaid}/{tipo}/{tipoid}/{id}")]
        public async Task<ActionResult> Actualiza(string id, string plantillaid, string tipo, string tipoid,
            [FromBody] RequestValoresPlantilla valores)
        {
            try
            {
                ValoresPlantilla valoresplantilla = new ValoresPlantilla()
                {
                    Id = id,
                    DatoId = tipoid,
                    TipoDatoId = tipo,
                    PlantillaId = plantillaid,
                    TipoOrigenId = "dominio",
                    OrigenId = this.DominioId,
                    Valores = valores.Valores,
                    IndiceFiltrado = valores.Filtro
                };

                Plantilla plantilla = await CacheMetadatos.ObtienePlantillaPorId(id, appCache, plantillas)
                       .ConfigureAwait(false);

                if (plantilla == null) return NotFound(valoresplantilla.PlantillaId);

                bool existe = await PLantillaGenerada(plantilla).ConfigureAwait(false);
                if (existe)
                {
                    
                    bool r = await repositorio.Actualiza(plantilla, valoresplantilla).ConfigureAwait(false);
                    if (r) return Ok();
                    return BadRequest(valores);
                }

                return NotFound(valoresplantilla.PlantillaId);
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
        public async Task<ActionResult<ValoresPlantilla>> Unico(string id, string plantillaid)
        {
            try
            {
                ValoresPlantilla p = new ValoresPlantilla();
                Plantilla plantilla = await CacheMetadatos.ObtienePlantillaPorId(id, appCache, plantillas)
                         .ConfigureAwait(false);

                if (plantilla == null) return NotFound($"plantilla {plantillaid}");

                bool existe = await PLantillaGenerada(plantilla).ConfigureAwait(false);
                if (existe)
                {
                    var r = await repositorio.Unico(plantilla, id).ConfigureAwait(false);
                    if (r!=null) return Ok(r);
                    
                }

                return BadRequest($"plantilla {plantillaid}");
                
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }

        }


        // -----------------------------------------------------------------------------
        // -----------------------------------------------------------------------------
        // -----------------------------------------------------------------------------
        // -----------------------------------------------------------------------------
        // -----------------------------------------------------------------------------


        /// <summary>
        /// Actualiza el contenido de los datos en la plantilla
        /// </summary>
        /// <param name="id"></param>
        /// <param name="valores"></param>
        /// <returns></returns>
        [HttpPut]
        public async Task<ActionResult> Actualizar(string id, ValoresPlantilla valores)
        {
            await Task.Delay(1).ConfigureAwait(false);
            return NoContent();
        }

        ///// <summary>
        ///// Obtiene una lista de Valores para los metadatos en base a una plantilla
        ///// </summary>
        ///// <param name="plantillaId"></param>
        ///// <param name="query"></param>
        ///// <returns></returns>
        //[HttpGet]
        //public async Task<ActionResult<IEnumerable<ValoresPlantilla>>> Consulta(string plantillaId, Consulta query)
        //{
        //    await Task.Delay(1).ConfigureAwait(false);
        //    var lista = new List<ValoresPlantilla>();
        //    return Ok(lista);
        //}


        /// <summary>
        /// Elimina una lista de objetos de una plantilla en base a la lista de identificadores
        /// </summary>
        /// <param name="plantillaId"></param>
        /// <param name="ids"></param>
        /// <returns></returns>
        [HttpDelete]
        public async Task<ActionResult<bool>>Elimina(string plantillaId, string[] ids )
        {
            await Task.Delay(1).ConfigureAwait(false);
            return NoContent();
        }


       


        [HttpPut("{plantillaid}/{tipo}/{if}")]
        public async Task<ActionResult<ValoresPlantilla>> Unico(string plantillaid,
            string tipo, string id, [FromBody] ValoresPlantilla valores)
        {
            Plantilla plantilla = null; // await appCache.Metadatos.ObtenerPlantilla(plantillaid,
                //ConstantesCache.CONTROLADORMETADATOS, "dominio", this.DominioId).ConfigureAwait(false);

            bool existe = await PLantillaGenerada(plantilla).ConfigureAwait(false);
            if (existe)
            {
                await repositorio.Inserta(plantilla, valores).ConfigureAwait(false);
                return Ok();
            }

            return NotFound();
        }


        ///// <summary>
        ///// Obtiene un elemento de metatdaos para el Id recibido
        ///// </summary>
        ///// <param name="plantillaId"></param>
        ///// <param name="id"></param>
        ///// <returns></returns>
        //[HttpGet("{plantillaid}/{tipo}/{id}", Name = "GetMetadatosUnico")]
        //public async Task<ActionResult<ValoresPlantilla>> Unico(string plantillaid, string id)
        //{
        //    Plantilla plantilla = await appCache.Metadatos.ObtenerPlantilla(plantillaid, 
        //        ConstantesCache.CONTROLADORMETADATOS, "dominio", this.DominioId).ConfigureAwait(false);

        //    bool existe = await PLantillaGenerada(plantilla).ConfigureAwait(false);
        //    logger.LogDebug($"{existe}");

        //    return Ok(plantilla);
        //    //if (plantilla != null)
        //    //{
                
        //    //    bool existe = await PLantillaGenerada(plantilla).ConfigureAwait(false);
        //    //    if (existe)
        //    //    {
        //    //        Consulta q = new Consulta()
        //    //        {
        //    //            consecutivo = 0,
        //    //            indice = 0,
        //    //            tamano = 5,
        //    //            ord_columna = "pin64",
        //    //            ord_direccion = "asc",
        //    //            recalcular_totales = false
        //    //        };

        //    //        List<FiltroConsulta> tmp = new List<FiltroConsulta>();

        //    //        q.Filtros .Add(new FiltroConsulta() { Propiedad = "TipOrigenId", Operador = "eq", Valor="demo" });
        //    //        q.Filtros.Add(new FiltroConsulta() { Propiedad = "pbooleano", Operador = "eq", Valor = "true" });

        //    //        string j= JsonSerializer.Serialize(q);

        //    //        //var data = plantilla.ObtieneValoresDemo();
        //    //        //string resp = await repositorio.Inserta(plantilla, data).ConfigureAwait(false);
        //    //        //if (resp != null)
        //    //        //{
        //    //        //    return Ok($"{data}");
        //    //        //}
        //    //        //return UnprocessableEntity($"{plantillaid}");
        //    //        return Ok(j);
        //    //    }
        //    //    else {
        //    //        return UnprocessableEntity($"{plantillaid}");
        //    //    }
                
        //    //}
        //    //else
        //    //{
        //    //    return NotFound($"{plantillaid}");
        //    //}

        //}



        //[HttpPost("buscar/{id}", Name = "Buscar")]
        //public async Task<ActionResult<string>> Buscar([FromBody] ConsultaArray query, string id)
        //{
        //    try
        //    {
        //        logger.LogInformation("Fuiltros {0}", id);
        //        logger.LogInformation("Fuiltros {0}", query.ord_direccion);
        //        logger.LogInformation("Fuiltros {0}", query.Filtros.Length);
        //    }
        //    catch (Exception ex)
        //    {

        //        logger.LogError("Fuiltros {0}", ex.ToString());
        //    }
            

        //    await Task.Delay(1).ConfigureAwait(false);
        //    return Ok("");
        //}



        private async Task<bool> PLantillaGenerada(Plantilla plantilla)
        {
            bool existe = await CacheMetadatos.PlantillaGenerada(plantilla.Id, appCache).ConfigureAwait(false);
            
            if (!existe)
            {
                // si no esta en cache
                //Verifcica en el repositorio
                existe = await repositorio.ExisteIndice(plantilla.Id).ConfigureAwait(false);

               if(!existe)
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