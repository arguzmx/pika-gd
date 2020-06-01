using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PIKA.GD.API.Servicios;
using PIKA.Modelo.Metadatos;
using RepositorioEntidades;
using PIKA.Servicio.Metadatos.ElasticSearch;
using System;
using System.Text.Json;
using System.Linq;

namespace PIKA.GD.API.Controllers.Metadatos
{
    //[Authorize]
    [ApiVersion("1.0")]
    [ApiController]
    [Route("api/v{version:apiVersion}/metadatos")]
    public class MetadatosController : ControllerBase
    {
        private ILogger<MetadatosController> logger;
        private readonly IRepositorioMetadatos repositorio;
        private readonly ICacheAplicacion appCache;
        
        public MetadatosController(
            IRepositorioMetadatos repositorio,
            ILogger<MetadatosController> logger,
            ICacheAplicacion appCache
            )
        {
            this.repositorio = repositorio;
            this.logger = logger;
            this.appCache= appCache;
        }


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

        /// <summary>
        /// Obtiene una lista de Valores para los metadatos en base a una plantilla
        /// </summary>
        /// <param name="plantillaId"></param>
        /// <param name="query"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ValoresPlantilla>>> Consulta(string plantillaId, Consulta query)
        {
            await Task.Delay(1).ConfigureAwait(false);
            var lista = new List<ValoresPlantilla>();
            return Ok(lista);
        }


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


        /// <summary>
        /// Crea una nueva entrada en la plantilla 
        /// </summary>
        /// <param name="TipoOrigenId"></param>
        /// <param name="OrigenId"></param>
        /// <param name="valores"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult<string>> Inserta(string TipoOrigenId, string OrigenId, ValoresPlantilla valores)
        {
            await Task.Delay(1).ConfigureAwait(false);
            return Ok("");
        }



        /// <summary>
        /// Obtiene un elemento de metatdaos para el Id recibido
        /// </summary>
        /// <param name="plantillaId"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("unico/{plantillaid}/{id}", Name = "GetMetadatosUnico")]
        public async Task<ActionResult<ValoresPlantilla>> Unico(string plantillaid, string id)
        {
            Plantilla plantilla = await appCache.Metadatos.ObtenerPlantilla(plantillaid, ContantesCache.CONTROLADORMETADATOS).ConfigureAwait(false);
            if (plantilla != null)
            {
                
                bool existe = await PLantillaGenerada(plantilla).ConfigureAwait(false);
                if (existe)
                {
                    Consulta q = new Consulta()
                    {
                        consecutivo = 0,
                        indice = 0,
                        tamano = 5,
                        ord_columna = "pin64",
                        ord_direccion = "asc",
                        recalcular_totales = false
                    };

                    List<FiltroConsulta> tmp = new List<FiltroConsulta>();

                    q.Filtros .Add(new FiltroConsulta() { Propiedad = "TipOrigenId", Operador = "eq", Valor="demo" });
                    q.Filtros.Add(new FiltroConsulta() { Propiedad = "pbooleano", Operador = "eq", Valor = "true" });

                    string j= JsonSerializer.Serialize(q);

                    //var data = plantilla.ObtieneValoresDemo();
                    //string resp = await repositorio.Inserta(plantilla, data).ConfigureAwait(false);
                    //if (resp != null)
                    //{
                    //    return Ok($"{data}");
                    //}
                    //return UnprocessableEntity($"{plantillaid}");
                    return Ok(j);
                }
                else {
                    return UnprocessableEntity($"{plantillaid}");
                }
                
            }
            else
            {
                return NotFound($"{plantillaid}");
            }

        }



        [HttpPost("buscar/{id}", Name = "Buscar")]
        public async Task<ActionResult<string>> Buscar([FromBody] ConsultaArray query, string id)
        {
            try
            {
                logger.LogInformation("Fuiltros {0}", id);
                logger.LogInformation("Fuiltros {0}", query.ord_direccion);
                logger.LogInformation("Fuiltros {0}", query.Filtros.Length);
            }
            catch (Exception ex)
            {

                logger.LogError("Fuiltros {0}", ex.ToString());
            }
            

            await Task.Delay(1).ConfigureAwait(false);
            return Ok("");
        }



        private async Task<bool> PLantillaGenerada(Plantilla plantilla)
        {
            bool existe = await appCache.Metadatos.EsPlantillaGenerada(plantilla.Id).ConfigureAwait(false);
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
                if (existe) appCache.Metadatos.AdicionaPlantillaGenerada(plantilla.Id); 
            }
            
            return existe;
        }
     
    }



}