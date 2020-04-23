using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PIKA.GD.API.Filters;
using PIKA.GD.API.Model;
using PIKA.Infraestructura.Comun;
using PIKA.Modelo.Metadatos;
using PIKA.Servicio.Seguridad.Interfaces;
using RepositorioEntidades;
using RepositorioEntidades.DatatablesPlugin;

namespace PIKA.GD.API.Controllers.Seguridad
{
    [Authorize]
    [ApiVersion("1.0")]
    [ApiController]
    [Route("api/v{version:apiVersion}/seguridad/[controller]")]
    public class AplicacionController : ACLController
    {
        private ILogger<AplicacionController> logger;
        private IServicioAplicacion servicioAplicacion;
        private IProveedorMetadatos<Aplicacion> metadataProvider;
        public AplicacionController(ILogger<AplicacionController> logger,
            IProveedorMetadatos<Aplicacion> metadataProvider,
            IServicioAplicacion servicioAplicacion)
        {
            this.logger = logger;
            this.servicioAplicacion = servicioAplicacion;
            this.metadataProvider = metadataProvider;
        }

        [HttpGet("metadata", Name = "MetadataAplicacion")]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        public async Task<ActionResult<MetadataInfo>> GetMetadata([FromQuery]Consulta query = null)
        {
            return Ok(await metadataProvider.Obtener().ConfigureAwait(false));
        }



        [HttpPost]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        public async Task<ActionResult<Aplicacion>> Post([FromBody]Aplicacion entidad)
        {

            
            entidad = await servicioAplicacion.CrearAsync(entidad).ConfigureAwait(false);
            return Ok(CreatedAtAction("GetAplicacion", new { id = entidad.Id }, entidad).Value);
        }


        [HttpPut("{id}")]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        public async Task<IActionResult> Put(string id, [FromBody]Aplicacion entidad)
        {
            var x = ObtieneFiltrosIdentidad();


            if (id != entidad.Id)
            {
                return BadRequest();
            }

            await servicioAplicacion.ActualizarAsync(entidad).ConfigureAwait(false);
            return NoContent();

        }


        [HttpGet("page", Name = "GetPageAplicacion")]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        public async Task<ActionResult<IEnumerable<Aplicacion>>> GetPage([ModelBinder(typeof(GenericDataPageModelBinder))][FromQuery]Consulta query = null)
        {
            Console.WriteLine($"Prueba {query.Filtros.Count}");
            Console.WriteLine($"--------------------------------{query.Filtros.Count}");
            ///Añade las propiedaes del contexto para el filtro de ACL vía ACL Controller
            query.Filtros.AddRange(ObtieneFiltrosIdentidad());

            var data = await servicioAplicacion.ObtenerPaginadoAsync(query).ConfigureAwait(false);
            return Ok(data.Elementos.ToList<Aplicacion>());
        }


        //----------------------------------------------------------------------------------------------------------
        //----------------------------------------------------------------------------------------------------------
        //----------------------------------------------------------------------------------------------------------
        //----------------------------------------------------------------------------------------------------------




        [HttpGet("datatables", Name = "GetDatatablesPluginAplicacion")]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        public async Task<ActionResult<RespuestaDatatables<Aplicacion>>> GetDatatablesPlugin([ModelBinder(typeof(DatatablesModelBinder))]SolicitudDatatables query = null)
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

            var data = await servicioAplicacion.ObtenerPaginadoAsync(newq).ConfigureAwait(false);

            RespuestaDatatables<Aplicacion> r = new RespuestaDatatables<Aplicacion>()
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
        public async Task<ActionResult<Aplicacion>> Get(string id)
        {
            var o = await servicioAplicacion.UnicoAsync(x => x.Id == id).ConfigureAwait(false);
            if (o != null) return Ok(o);
            return NotFound(id);
        }




        [HttpDelete]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        public async Task<ActionResult> Delete([FromBody]string[] id)
        {
            return Ok(await servicioAplicacion.Eliminar(id).ConfigureAwait(false));
        }
    }
}