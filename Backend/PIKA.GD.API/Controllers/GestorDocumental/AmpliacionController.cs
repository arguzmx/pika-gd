using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PIKA.GD.API.Filters;
using PIKA.GD.API.Model;
using PIKA.Modelo.GestorDocumental;
using PIKA.Modelo.Metadatos;
using PIKA.Servicio.GestionDocumental.Interfaces;
using RepositorioEntidades;
using RepositorioEntidades.DatatablesPlugin;

namespace PIKA.GD.API.Controllers.GestorDocumental
{
    [Authorize]
    [ApiVersion("1.0")]
    [ApiController]
    [Route("api/v{version:apiVersion}/gd/[controller]")]
    public class AmpliacionController : ACLController
    {
        private readonly ILogger<AmpliacionController> logger;
        private IServicioAmpliacion servicioAmpliacion;
        private IProveedorMetadatos<Ampliacion> metadataProvider;
        public AmpliacionController(ILogger<AmpliacionController> logger,
            IProveedorMetadatos<Ampliacion> metadataProvider,
            IServicioAmpliacion servicioAmpliacion)
        {
            this.logger = logger;
            this.servicioAmpliacion = servicioAmpliacion;
            this.metadataProvider = metadataProvider;
        }

        [HttpGet("metadata", Name = "MetadataAmpliacion")]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        public async Task<ActionResult<MetadataInfo>> GetMetadata([FromQuery]Consulta query = null)
        {
            return Ok(await metadataProvider.Obtener().ConfigureAwait(false));
        }



        [HttpPost]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        public async Task<ActionResult<Ampliacion>> Post([FromBody]Ampliacion entidad)
        {
            entidad = await servicioAmpliacion.CrearAsync(entidad).ConfigureAwait(false);
            return Ok(CreatedAtAction("GetAmpliacion", new { id = entidad.ActivoId, vigente = entidad.Vigente }, entidad).Value);
        }


        [HttpPut("{id}")]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        public async Task<IActionResult> Put(string id, [FromBody]Ampliacion entidad)
        {
            var x = ObtieneFiltrosIdentidad();


            if (id != entidad.ActivoId)
            {
                return BadRequest();
            }

            await servicioAmpliacion.ActualizarAsync(entidad).ConfigureAwait(false);
            return NoContent();

        }


        [HttpGet("page", Name = "GetPageAmpliacion")]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        public async Task<ActionResult<IEnumerable<Ampliacion>>> GetPage([ModelBinder(typeof(GenericDataPageModelBinder))][FromQuery]Consulta query = null)
        {
            ///Añade las propiedaes del contexto para el filtro de ACL vía ACL Controller
            query.Filtros.AddRange(ObtieneFiltrosIdentidad());
            var data = await servicioAmpliacion.ObtenerPaginadoAsync(query).ConfigureAwait(false);
            return Ok(data.Elementos.ToList<Ampliacion>());
        }


        //----------------------------------------------------------------------------------------------------------
        //----------------------------------------------------------------------------------------------------------
        //----------------------------------------------------------------------------------------------------------
        //----------------------------------------------------------------------------------------------------------




        [HttpGet("datatables", Name = "GetDatatablesPluginAmpliacion")]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        public async Task<ActionResult<RespuestaDatatables<Ampliacion>>> GetDatatablesPlugin([ModelBinder(typeof(DatatablesModelBinder))]SolicitudDatatables query = null)
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

            var data = await servicioAmpliacion.ObtenerPaginadoAsync(newq).ConfigureAwait(false);

            RespuestaDatatables<Ampliacion> r = new RespuestaDatatables<Ampliacion>()
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
        public async Task<ActionResult<Ampliacion>> Get(string id)
        {
            var o = await servicioAmpliacion.UnicoAsync(x => x.ActivoId == id).ConfigureAwait(false);
            if (o != null) return Ok(o);
            return NotFound(id);
        }




        [HttpDelete]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        public async Task<ActionResult> Delete([FromBody]string[] id)
        {
            return Ok(await servicioAmpliacion.Eliminar(id).ConfigureAwait(false));
        }
    }
}