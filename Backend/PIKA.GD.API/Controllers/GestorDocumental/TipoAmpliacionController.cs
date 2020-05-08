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
    public class TipoAmpliacionController : ACLController
    {
        private readonly ILogger<TipoAmpliacionController> logger;
        private IServicioTipoAmpliacion servicioTipoAmpliacion;
        private IProveedorMetadatos<TipoAmpliacion> metadataProvider;
        public TipoAmpliacionController(ILogger<TipoAmpliacionController> logger,
            IProveedorMetadatos<TipoAmpliacion> metadataProvider,
            IServicioTipoAmpliacion servicioTipoAmpliacion)
        {
            this.logger = logger;
            this.servicioTipoAmpliacion = servicioTipoAmpliacion;
            this.metadataProvider = metadataProvider;
        }

        [HttpGet("metadata", Name = "MetadataTipoAmpliacion")]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        public async Task<ActionResult<MetadataInfo>> GetMetadata([FromQuery]Consulta query = null)
        {
            return Ok(await metadataProvider.Obtener().ConfigureAwait(false));
        }



        [HttpPost]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        public async Task<ActionResult<TipoAmpliacion>> Post([FromBody]TipoAmpliacion entidad)
        {
            entidad = await servicioTipoAmpliacion.CrearAsync(entidad).ConfigureAwait(false);
            return Ok(CreatedAtAction("GetTipoAmpliacion", new { id = entidad.Id }, entidad).Value);
        }


        [HttpPut("{id}")]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        public async Task<IActionResult> Put(string id, [FromBody]TipoAmpliacion entidad)
        {
            var x = ObtieneFiltrosIdentidad();


            if (id != entidad.Id)
            {
                return BadRequest();
            }

            await servicioTipoAmpliacion.ActualizarAsync(entidad).ConfigureAwait(false);
            return NoContent();

        }


        [HttpGet("page", Name = "GetPageTipoAmpliacion")]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        public async Task<ActionResult<IEnumerable<TipoAmpliacion>>> GetPage([ModelBinder(typeof(GenericDataPageModelBinder))][FromQuery]Consulta query = null)
        {
            ///Añade las propiedaes del contexto para el filtro de ACL vía ACL Controller
            query.Filtros.AddRange(ObtieneFiltrosIdentidad());
            var data = await servicioTipoAmpliacion.ObtenerPaginadoAsync(query).ConfigureAwait(false);
            return Ok(data.Elementos.ToList<TipoAmpliacion>());
        }


        //----------------------------------------------------------------------------------------------------------
        //----------------------------------------------------------------------------------------------------------
        //----------------------------------------------------------------------------------------------------------
        //----------------------------------------------------------------------------------------------------------




        [HttpGet("datatables", Name = "GetDatatablesPluginTipoAmpliacion")]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        public async Task<ActionResult<RespuestaDatatables<TipoAmpliacion>>> GetDatatablesPlugin([ModelBinder(typeof(DatatablesModelBinder))]SolicitudDatatables query = null)
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

            var data = await servicioTipoAmpliacion.ObtenerPaginadoAsync(newq).ConfigureAwait(false);

            RespuestaDatatables<TipoAmpliacion> r = new RespuestaDatatables<TipoAmpliacion>()
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
        public async Task<ActionResult<TipoAmpliacion>> Get(string id)
        {
            var o = await servicioTipoAmpliacion.UnicoAsync(x => x.Id == id).ConfigureAwait(false);
            if (o != null) return Ok(o);
            return NotFound(id);
        }




        [HttpDelete]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        public async Task<ActionResult> Delete([FromBody]string[] id)
        {
            return Ok(await servicioTipoAmpliacion.Eliminar(id).ConfigureAwait(false));
        }
    }
}