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
using RepositorioEntidades;
using RepositorioEntidades.DatatablesPlugin;

namespace PIKA.GD.API.Controllers.Metadatos
{
    [Authorize]
    [ApiVersion("1.0")]
    [ApiController]
    [Route("api/v{version:apiVersion}/metadatos/[controller]")]
    public class TipoDatoPropiedadPlantillaController : ACLController
    {
        private ILogger<TipoDatoPropiedadPlantillaController> logger;
        private IServicioTipoDatoPropiedadPlantilla servicioTipoDatoPropiedadPlantilla;
        private IProveedorMetadatos<TipoDatoPropiedadPlantilla> metadataProvider;
        public TipoDatoPropiedadPlantillaController(ILogger<TipoDatoPropiedadPlantillaController> logger,
            IProveedorMetadatos<TipoDatoPropiedadPlantilla> metadataProvider,
            IServicioTipoDatoPropiedadPlantilla servicioTipoDatoPropiedadPlantilla)
        {
            this.logger = logger;
            this.servicioTipoDatoPropiedadPlantilla = servicioTipoDatoPropiedadPlantilla;
            this.metadataProvider = metadataProvider;
        }

        [HttpGet("metadata", Name = "MetadataTipoDatoPropiedadPlantilla")]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        public async Task<ActionResult<MetadataInfo>> GetMetadata([FromQuery]Consulta query = null)
        {
            return Ok(await metadataProvider.Obtener().ConfigureAwait(false));
        }



        [HttpPost]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        public async Task<ActionResult<TipoDatoPropiedadPlantilla>> Post([FromBody]TipoDatoPropiedadPlantilla entidad)
        {

            
            entidad = await servicioTipoDatoPropiedadPlantilla.CrearAsync(entidad).ConfigureAwait(false);
            return Ok(CreatedAtAction("GetTipoDatoPropiedadPlantilla", new { id = entidad.TipoDatoId }, entidad).Value);
        }


        [HttpPut("{id}")]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        public async Task<IActionResult> Put(string id, [FromBody]TipoDatoPropiedadPlantilla entidad)
        {
            var x = ObtieneFiltrosIdentidad();


            if (id != entidad.TipoDatoId)
            {
                return BadRequest();
            }

            await servicioTipoDatoPropiedadPlantilla.ActualizarAsync(entidad).ConfigureAwait(false);
            return NoContent();

        }


        [HttpGet("page", Name = "GetPageTipoDatoPropiedadPlantilla")]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        public async Task<ActionResult<IEnumerable<TipoDatoPropiedadPlantilla>>> GetPage([FromQuery]Consulta query = null)
        {
            
            ///Añade las propiedaes del contexto para el filtro de ACL vía ACL Controller
            query.Filtros.AddRange(ObtieneFiltrosIdentidad());
            var data = await servicioTipoDatoPropiedadPlantilla.ObtenerPaginadoAsync(query).ConfigureAwait(false);
            return Ok(data.Elementos.ToList<TipoDatoPropiedadPlantilla>());
        }


        //----------------------------------------------------------------------------------------------------------
        //----------------------------------------------------------------------------------------------------------
        //----------------------------------------------------------------------------------------------------------
        //----------------------------------------------------------------------------------------------------------




        [HttpGet("datatables", Name = "GetDatatablesPluginTipoDatoPropiedadPlantilla")]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        public async Task<ActionResult<RespuestaDatatables<TipoDatoPropiedadPlantilla>>> GetDatatablesPlugin([ModelBinder(typeof(DatatablesModelBinder))]SolicitudDatatables query = null)
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

            var data = await servicioTipoDatoPropiedadPlantilla.ObtenerPaginadoAsync(newq).ConfigureAwait(false);

            RespuestaDatatables<TipoDatoPropiedadPlantilla> r = new RespuestaDatatables<TipoDatoPropiedadPlantilla>()
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
        public async Task<ActionResult<TipoDatoPropiedadPlantilla>> Get(string id)
        {
            var o = await servicioTipoDatoPropiedadPlantilla.UnicoAsync(x => x.TipoDatoId == id).ConfigureAwait(false);
            if (o != null) return Ok(o);
            return NotFound(id);
        }




        [HttpDelete("{id}")]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        public async Task<ActionResult> Delete([FromBody]string[] id)
        {
            await servicioTipoDatoPropiedadPlantilla.Eliminar(id).ConfigureAwait(false);
            return NoContent();
        }
    }
}