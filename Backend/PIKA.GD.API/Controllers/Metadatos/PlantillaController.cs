using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PIKA.GD.API.Filters;
using PIKA.GD.API.Model;
using PIKA.Modelo.Metadatos;
using PIKA.Servicio.Metadatos.Interfaces;
using PIKA.Servicio.Metadatos.Servicios;
using RepositorioEntidades;
using RepositorioEntidades.DatatablesPlugin;

namespace PIKA.GD.API.Controllers.Metadatos
{
    [Authorize]
    [ApiVersion("1.0")]
    [ApiController]
    [Route("api/v{version:apiVersion}/metadatos/[controller]")]
    public class PlantillaController : ACLController

    {
        private ILogger<PlantillaController> logger;
        private IServicioPlantilla servicioPlantilla;
        private IProveedorMetadatos<Plantilla> metadataProvider;
        public PlantillaController(ILogger<PlantillaController> logger,
            IProveedorMetadatos<Plantilla> metadataProvider,
            IServicioPlantilla servicioPlantilla)
        {
            this.logger = logger;
            this.servicioPlantilla = servicioPlantilla;
            this.metadataProvider = metadataProvider;
        }

        [HttpGet("metadata", Name = "MetadataPlantilla")]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        public async Task<ActionResult<MetadataInfo>> GetMetadata([FromQuery]Consulta query = null)
        {
            return Ok(await metadataProvider.Obtener().ConfigureAwait(false));
        }



        [HttpPost]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        public async Task<ActionResult<Plantilla>> Post([FromBody]Plantilla entidad)
        {

            entidad = await servicioPlantilla.CrearAsync(entidad).ConfigureAwait(false);
            return Ok(CreatedAtAction("GetPlantilla", new { id = entidad.Id }, entidad).Value);
        }


        [HttpPut("{id}")]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        public async Task<IActionResult> Put(string id, [FromBody]Plantilla entidad)
        {
            var x = ObtieneFiltrosIdentidad();


            if (id != entidad.Id)
            {
                return BadRequest();
            }

            await servicioPlantilla.ActualizarAsync(entidad).ConfigureAwait(false);
            return NoContent();

        }


        [HttpGet("page", Name = "GetPagePlantilla")]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        public async Task<ActionResult<IEnumerable<Plantilla>>> GetPage([ModelBinder(typeof(GenericDataPageModelBinder))][FromQuery]Consulta query = null)
        {
            ///Añade las propiedaes del contexto para el filtro de ACL vía ACL Controller
            query.Filtros.AddRange(ObtieneFiltrosIdentidad());
            var data = await servicioPlantilla.ObtenerPaginadoAsync(query).ConfigureAwait(false);
            return Ok(data.Elementos.ToList<Plantilla>());
        }


        //----------------------------------------------------------------------------------------------------------
        //----------------------------------------------------------------------------------------------------------
        //----------------------------------------------------------------------------------------------------------
        //----------------------------------------------------------------------------------------------------------




        [HttpGet("datatables", Name = "GetDatatablesPluginPlantilla")]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        public async Task<ActionResult<RespuestaDatatables<Plantilla>>> GetDatatablesPlugin([ModelBinder(typeof(DatatablesModelBinder))]SolicitudDatatables query = null)
        {

            if (query.order.Count == 0)
            {
                query.order.Add(new DatatatablesOrder() { dir = "asc", column = 0 });
            }

            string sortname = query.columns[query.order[0].column].data;

            Consulta newq = new Consulta()
            {
                Filtros = query.Filters,
                indice = query.start,
                tamano = query.length,
                ord_columna = sortname,
                ord_direccion = query.order[0].dir
            };

            var data = await servicioPlantilla.ObtenerPaginadoAsync(newq).ConfigureAwait(false);

            RespuestaDatatables<Plantilla> r = new RespuestaDatatables<Plantilla>()
            {
                data = data.Elementos.ToArray(),
                draw = query.draw,
                error = "",
                recordsFiltered = data.Conteo,
                recordsTotal = data.Conteo
            };

            return Ok(r);
        }



        [HttpGet("{id}")]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        public async Task<ActionResult<Plantilla>> Get(string id)
        {
            var o = await servicioPlantilla.UnicoAsync(x => x.Id == id).ConfigureAwait(false);
            if (o != null) return Ok(o);
            return NotFound(id);
        }

        [HttpDelete("{id}")]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        public async Task<ActionResult> Delete([FromBody]string[] id)
        {
            await servicioPlantilla.Eliminar(id).ConfigureAwait(false);
            return NoContent();
        }
    }
}