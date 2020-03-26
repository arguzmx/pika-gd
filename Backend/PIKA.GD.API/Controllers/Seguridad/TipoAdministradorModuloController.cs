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
    public class TipoAdministradorModuloController : ACLController
    {
        private ILogger<TipoAdministradorModuloController> logger;
        private IServicioTipoAdministradorModulo servicioTipoAdministradorModulo;
        private IProveedorMetadatos<TipoAdministradorModulo> metadataProvider;
        public TipoAdministradorModuloController(ILogger<TipoAdministradorModuloController> logger,
            IProveedorMetadatos<TipoAdministradorModulo> metadataProvider,
            IServicioTipoAdministradorModulo servicioTipoAdministradorModulo)
        {
            this.logger = logger;
            this.servicioTipoAdministradorModulo = servicioTipoAdministradorModulo;
            this.metadataProvider = metadataProvider;
        }

        [HttpGet("metadata", Name = "MetadataTipoAdministracionmodulo")]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        public async Task<ActionResult<MetadataInfo>> GetMetadata([FromQuery]Consulta query = null)
        {
            return Ok(await metadataProvider.Obtener().ConfigureAwait(false));
        }



        [HttpPost]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        public async Task<ActionResult<TipoAdministradorModulo>> Post([FromBody]TipoAdministradorModulo entidad)
        {
            entidad = await servicioTipoAdministradorModulo.CrearAsync(entidad).ConfigureAwait(false);
            return Ok(CreatedAtAction("GetTipoAdministradorModulo", new { id = entidad.ModuloId }, entidad).Value);
        }


        [HttpPut("{TipoAdministradorModuloId}")]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        public async Task<IActionResult> Put(string id, [FromBody]TipoAdministradorModulo entidad)
        {
            var x = ObtieneFiltrosIdentidad();


            if (id != entidad.ModuloId)
            {
                return BadRequest();
            }

            await servicioTipoAdministradorModulo.ActualizarAsync(entidad).ConfigureAwait(false);
            return NoContent();

        }


        [HttpGet("page", Name = "GetPageTipoAdministracionmodulo")]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        public async Task<ActionResult<IEnumerable<TipoAdministradorModulo>>> GetPage([FromQuery]Consulta query = null)
        {
            Console.WriteLine("------------------------------------------------------");
            ///Añade las propiedaes del contexto para el filtro de ACL vía ACL Controller
            query.Filtros.AddRange(ObtieneFiltrosIdentidad());
            var data = await servicioTipoAdministradorModulo.ObtenerPaginadoAsync(query).ConfigureAwait(false);
            return Ok(data.Elementos.ToList<TipoAdministradorModulo>());
        }


        //----------------------------------------------------------------------------------------------------------
        //----------------------------------------------------------------------------------------------------------
        //----------------------------------------------------------------------------------------------------------
        //----------------------------------------------------------------------------------------------------------




        [HttpGet("datatables", Name = "GetDatatablesPluginTipoAdministradorModulo")]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        public async Task<ActionResult<RespuestaDatatables<TipoAdministradorModulo>>> GetDatatablesPlugin([ModelBinder(typeof(DatatablesModelBinder))]SolicitudDatatables query = null)
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

            var data = await servicioTipoAdministradorModulo.ObtenerPaginadoAsync(newq).ConfigureAwait(false);

            RespuestaDatatables<TipoAdministradorModulo> r = new RespuestaDatatables<TipoAdministradorModulo>()
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
        public async Task<ActionResult<TipoAdministradorModulo>> Get(string id)
        {
            var o = await servicioTipoAdministradorModulo.UnicoAsync(x => x.ModuloId == id).ConfigureAwait(false);
            if (o != null) return Ok(o);
            return NotFound(id);
        }




        [HttpDelete("{id}")]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        public async Task<ActionResult> Delete([FromBody]string[] id)
        {
            await servicioTipoAdministradorModulo.Eliminar(id).ConfigureAwait(false);
            return NoContent();
        }
    }
}