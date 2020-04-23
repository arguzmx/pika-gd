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
    public class ModuloAplicacionController : ACLController
    {

        private ILogger<ModuloAplicacionController> logger;
        private IServicioModuloAplicacion servicioModuloAplicacion;
        private IProveedorMetadatos<ModuloAplicacion> metadataProvider;
        public ModuloAplicacionController(ILogger<ModuloAplicacionController> logger,
            IProveedorMetadatos<ModuloAplicacion> metadataProvider,
            IServicioModuloAplicacion servicioModuloAplicacion)
        {
            this.logger = logger;
            this.servicioModuloAplicacion = servicioModuloAplicacion;
            this.metadataProvider = metadataProvider;
        }

        [HttpGet("metadata", Name = "MetadataModuloAplicacion")]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        public async Task<ActionResult<MetadataInfo>> GetMetadata([FromQuery]Consulta query = null)
        {
            return Ok(await metadataProvider.Obtener().ConfigureAwait(false));
        }



        [HttpPost]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        public async Task<ActionResult<ModuloAplicacion>> Post([FromBody]ModuloAplicacion entidad)
        {
          
            entidad = await servicioModuloAplicacion.CrearAsync(entidad).ConfigureAwait(false);
        
            return Ok(CreatedAtAction("GetModuloAplicacion", new { id = entidad.Id }, entidad).Value);
        }


        [HttpPut("{id}")]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        public async Task<IActionResult> Put(string id, [FromBody]ModuloAplicacion entidad)
        {
            var x = ObtieneFiltrosIdentidad();

            Console.WriteLine("\n id Entity ::: " + entidad.Id);
            if (id != entidad.Id)
            {
                return BadRequest();
            }
        
            await servicioModuloAplicacion.ActualizarAsync(entidad).ConfigureAwait(false);
            return NoContent();

        }


        [HttpGet("page", Name = "GetPageModuloAplicacion")]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        public async Task<ActionResult<IEnumerable<ModuloAplicacion>>> GetPage([ModelBinder(typeof(GenericDataPageModelBinder))][FromQuery]Consulta query = null)
        {
            
            ///Añade las propiedaes del contexto para el filtro de ACL vía ACL Controller
            query.Filtros.AddRange(ObtieneFiltrosIdentidad());
            var data = await servicioModuloAplicacion.ObtenerPaginadoAsync(query).ConfigureAwait(false);
            return Ok(data.Elementos.ToList<ModuloAplicacion>());
        }


        


        [HttpGet("datatables", Name = "GetDatatablesPluginModuloAplicacion")]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        public async Task<ActionResult<RespuestaDatatables<ModuloAplicacion>>> GetDatatablesPlugin([ModelBinder(typeof(DatatablesModelBinder))]SolicitudDatatables query = null)
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

            var data = await servicioModuloAplicacion.ObtenerPaginadoAsync(newq).ConfigureAwait(false);

            RespuestaDatatables<ModuloAplicacion> r = new RespuestaDatatables<ModuloAplicacion>()
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
        public async Task<ActionResult<ModuloAplicacion>> Get(string id)
        {
            var o = await servicioModuloAplicacion.UnicoAsync(x => x.Id == id).ConfigureAwait(false);
            if (o != null) return Ok(o);
            return NotFound(id);
        }
                     
        [HttpDelete("{id}")]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        public async Task<ActionResult> Delete([FromBody]string[] id)
        {
            await servicioModuloAplicacion.Eliminar(id).ConfigureAwait(false);
            return NoContent();
        }
    }

}