using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LazyCache;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PIKA.GD.API.Servicios;
using PIKA.Infraestructura.Comun;
using PIKA.Modelo.Metadatos;
using PIKA.Servicio.Metadatos.Data.Migrations;
using PIKA.Servicio.Metadatos.Interfaces;
using RepositorioEntidades;

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
        private readonly IServicioPlantilla servicioPlantilla;
        private readonly IAPICache<Plantilla> cachePlantilla;
        private Plantilla plantilla;
        

        private async Task<Plantilla> ObtenerPlantilla(string PlantillaId)
        {
            plantilla = await cachePlantilla.Obtiene(PlantillaId).ConfigureAwait(true);


            if (plantilla == null) {
                plantilla = await servicioPlantilla.UnicoAsync(
                    predicado: x => x.Id == PlantillaId,
                    incluir: 
                    y=>y.Include(z=>z.Propiedades).ThenInclude(z=>  z.TipoDato )
                    .Include(z=>z.Propiedades).ThenInclude(z=>z.ValidadorNumero)
                    .Include(z => z.Propiedades).ThenInclude(z => z.ValidadorTexto)
                    .Include(z => z.Propiedades).ThenInclude(z => z.AtributoTabla)
                     ).ConfigureAwait(true);
                if (plantilla != null) {
                    Console.WriteLine(" + Cache");
                    await cachePlantilla.Inserta(PlantillaId, plantilla, new TimeSpan(1, 0, 0)).ConfigureAwait(false);
                }
            }
            return plantilla;
        }

        public MetadatosController(
            IRepositorioMetadatos repositorio,
            ILogger<MetadatosController> logger,
            IServicioPlantilla servicioPlantilla,
            IAPICache<Plantilla> cachePlantilla
            )
        {
            this.repositorio = repositorio;
            this.logger = logger;
            this.servicioPlantilla = servicioPlantilla;
            this.cachePlantilla = cachePlantilla;
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
            await Task.Delay(1).ConfigureAwait(true);
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
            await Task.Delay(1).ConfigureAwait(true);
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
            await Task.Delay(1).ConfigureAwait(true);
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
            await Task.Delay(1).ConfigureAwait(true);
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
            Console.WriteLine($"{plantillaid}/{id}");
            await Task.Delay(1).ConfigureAwait(true);
            if (await ObtenerPlantilla(plantillaid) != null)
            {
                return Ok(this.plantilla);
            }
            else
            {

                return NotFound();
            }

            return Ok();
            
        }


     
    }
}