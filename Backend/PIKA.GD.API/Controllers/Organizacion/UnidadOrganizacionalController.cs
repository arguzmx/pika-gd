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
    [Route("api/v{version:apiVersion}/[controller]")]
    public class UnidadOrganizacionalController : ACLController
    {

        private ILogger<UnidadOrganizacionalController> logger;
        private IServicioUnidadOrganizacional servicioUO;
        private IMetadataProvider<UnidadOrganizacional> metadataProvider;
        public UnidadOrganizacionalController(ILogger<UnidadOrganizacionalController> logger,
            IMetadataProvider<UnidadOrganizacional> metadataProvider,
            IServicioUnidadOrganizacional servicioUO)
        {
            this.logger = logger;
            this.servicioUO = servicioUO;
            this.metadataProvider = metadataProvider;
        }

        [HttpGet("metadata")]
        [TypeFilter(typeof(AsyncACLActionFilter), Arguments = new object[] { ConstantesAplicacion.Id, AplicacionOrganizacion.MODULO_ORGANIZACION_UNIDADES_ORGANIZACIONALES })]
        public async Task<ActionResult<MetadataInfo>> GetMetadate([FromQuery]Consulta query = null)
        {
            return Ok(await metadataProvider.Obtener().ConfigureAwait(false));
        }


        [HttpGet]
        [TypeFilter(typeof(AsyncACLActionFilter), Arguments = new object[] { ConstantesAplicacion.Id, AplicacionOrganizacion.MODULO_ORGANIZACION_UNIDADES_ORGANIZACIONALES })]
        public async Task<ActionResult<IEnumerable<UnidadOrganizacional>>> GetPage([FromQuery]Consulta query = null)
        {
            ///Añade las propiedaes del contexto para el filtro de ACL vía ACL Controller
            query.Filtros.AddRange(ObtieneFiltrosIdentidad()); 
            
            var data = await servicioUO.ObtenerPaginadoAsync(query).ConfigureAwait(false);

            return Ok(data.Elementos.ToList<UnidadOrganizacional>());
        }

        [HttpGet("datatables", Name = "GetDatatablesPluginUnidadesorganizacionales")]
        [TypeFilter(typeof(AsyncACLActionFilter), Arguments = new object[] { ConstantesAplicacion.Id, AplicacionOrganizacion.MODULO_ORGANIZACION_UNIDADES_ORGANIZACIONALES })]
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
                columna_ordenamiento = sortname,
                direccion_ordenamiento = query.order[0].dir
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
        [TypeFilter(typeof(AsyncACLActionFilter), Arguments = new object[] { ConstantesAplicacion.Id, AplicacionOrganizacion.MODULO_ORGANIZACION_UNIDADES_ORGANIZACIONALES })]
        public async Task<ActionResult<UnidadOrganizacional>> Get(string id)
        {
            var o = await servicioUO.UnicoAsync(x=>x.Id==id).ConfigureAwait(false);
            if (o != null) return Ok(o);
            return NotFound(id);
        }



        [HttpPost]
        [TypeFilter(typeof(AsyncACLActionFilter), Arguments = new object[] { ConstantesAplicacion.Id, AplicacionOrganizacion.MODULO_ORGANIZACION_UNIDADES_ORGANIZACIONALES })]
        public async Task<ActionResult<UnidadOrganizacional>> Post([FromBody]UnidadOrganizacional entidad)
        {

            entidad= await servicioUO.CrearAsync(entidad).ConfigureAwait(false);
            return Ok(CreatedAtAction("GetOU", new { id = entidad.Id }, entidad).Value);
        }


        [HttpPut("{id}")]
        [TypeFilter(typeof(AsyncACLActionFilter), Arguments = new object[] { ConstantesAplicacion.Id, AplicacionOrganizacion.MODULO_ORGANIZACION_UNIDADES_ORGANIZACIONALES })]
        public async Task<IActionResult> Put(string id, [FromBody]UnidadOrganizacional entidad)
        {
            if (id != entidad.Id)
            {
                return BadRequest();
            }

            await servicioUO.ActualizarAsync(entidad).ConfigureAwait(false);
            return NoContent();

        }


        [HttpDelete("{id}")]
        [TypeFilter(typeof(AsyncACLActionFilter), Arguments = new object[] { ConstantesAplicacion.Id, AplicacionOrganizacion.MODULO_ORGANIZACION_UNIDADES_ORGANIZACIONALES })]
        public async Task<ActionResult> Delete([FromBody ]string[] id)
        {
            await servicioUO.Eliminar(id).ConfigureAwait(false);
            return NoContent();
        }

    }
}
