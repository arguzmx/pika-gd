using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PIKA.GD.API.Filters;
using PIKA.GD.API.Model;
using PIKA.Infraestructura.Comun.Constantes;
using PIKA.Modelo.Metadatos;
using PIKA.Modelo.Organizacion;
using PIKA.Servicio.Organizacion;
using RepositorioEntidades;
using RepositorioEntidades.DatatablesPlugin;

namespace PIKA.GD.API.Controllers.Organizacion
{

    [Authorize]
    [ApiVersion("1.0")]
    [ApiController]
    [Route("api/v{version:apiVersion}/org/[controller]")]
    public class UnidadOrganizacionalController : ACLController
    {

        private ILogger<UnidadOrganizacionalController> logger;
        private IServicioUnidadOrganizacional servicioUO;
        private IProveedorMetadatos<UnidadOrganizacional> metadataProvider;
        public UnidadOrganizacionalController(ILogger<UnidadOrganizacionalController> logger,
            IProveedorMetadatos<UnidadOrganizacional> metadataProvider,
            IServicioUnidadOrganizacional servicioUO)
        {
            this.logger = logger;
            this.servicioUO = servicioUO;
            this.metadataProvider = metadataProvider;
        }

        [HttpGet("metadata", Name ="MetadataUO")]
        [TypeFilter(typeof(AsyncACLActionFilter) )]
        public async Task<ActionResult<MetadataInfo>> GetMetadata([FromQuery]Consulta query = null)
        {
            return Ok(await metadataProvider.Obtener().ConfigureAwait(false));
        }

        [HttpPost]
        [TypeFilter(typeof(AsyncACLActionFilter) )]
        public async Task<ActionResult<UnidadOrganizacional>> Post([FromBody]UnidadOrganizacional entidad)
        {

            entidad = await servicioUO.CrearAsync(entidad).ConfigureAwait(false);
            return Ok(CreatedAtAction("GetOU", new { id = entidad.Id }, entidad).Value);
        }


        [HttpPut("{id}")]
        [TypeFilter(typeof(AsyncACLActionFilter) )]
        public async Task<IActionResult> Put(string id, [FromBody]UnidadOrganizacional entidad)
        {
            var x = ObtieneFiltrosIdentidad();

            if (id != entidad.Id)
            {
                return BadRequest();
            }

            await servicioUO.ActualizarAsync(entidad).ConfigureAwait(false);
            return NoContent();

        }



        [HttpGet("page", Name = "GetPageUO")]
        [TypeFilter(typeof(AsyncACLActionFilter) )]
        public async Task<ActionResult<IEnumerable<UnidadOrganizacional>>> GetPage([ModelBinder(typeof(GenericDataPageModelBinder))][FromQuery]Consulta query = null)
        {
            ///Añade las propiedaes del contexto para el filtro de ACL vía ACL Controller
            query.Filtros.AddRange(ObtieneFiltrosIdentidad());
            var data = await servicioUO.ObtenerPaginadoAsync(query).ConfigureAwait(false);

            return Ok(data.Elementos.ToList<UnidadOrganizacional>());
        }

        //----------------------------------------------------------------------------------------------------------
        //----------------------------------------------------------------------------------------------------------
        //----------------------------------------------------------------------------------------------------------
        //----------------------------------------------------------------------------------------------------------


        [HttpGet("datatables", Name = "GetDatatablesPluginUnidadesorganizacionales")]
        [TypeFilter(typeof(AsyncACLActionFilter) )]
        public async Task<ActionResult<RespuestaDatatables<UnidadOrganizacional>>> GetDatatablesPlugin([ModelBinder(typeof(DatatablesModelBinder))]SolicitudDatatables query = null)
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

            var data = await servicioUO.ObtenerPaginadoAsync(newq).ConfigureAwait(false);

            RespuestaDatatables<UnidadOrganizacional> r = new RespuestaDatatables<UnidadOrganizacional>()
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
        [TypeFilter(typeof(AsyncACLActionFilter) )]
        public async Task<ActionResult<UnidadOrganizacional>> Get(string id)
        {
            var o = await servicioUO.UnicoAsync(x=>x.Id==id).ConfigureAwait(false);
            if (o != null) return Ok(o);
            return NotFound(id);
        }


        [HttpDelete]
        [TypeFilter(typeof(AsyncACLActionFilter) )]
        public async Task<ActionResult> Delete([FromBody ]string[] id)
        {
            return Ok(await servicioUO.Eliminar(id).ConfigureAwait(false));
        }

    }
}
