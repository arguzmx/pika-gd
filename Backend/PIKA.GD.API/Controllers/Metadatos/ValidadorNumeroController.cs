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
    public class ValidadorNumeroController : ACLController

    {
        private ILogger<ValidadorNumeroController> logger;
        private IServicioValidadorNumero servicioValidadorNumero;
        private IProveedorMetadatos<ValidadorNumero> metadataProvider;
        public ValidadorNumeroController(ILogger<ValidadorNumeroController> logger,
            IProveedorMetadatos<ValidadorNumero> metadataProvider,
            IServicioValidadorNumero servicioValidadorNumero)
        {
            this.logger = logger;
            this.servicioValidadorNumero = servicioValidadorNumero;
            this.metadataProvider = metadataProvider;
        }

        [HttpGet("metadata", Name = "MetadataValidadorNumero")]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        public async Task<ActionResult<MetadataInfo>> GetMetadata([FromQuery]Consulta query = null)
        {
            return Ok(await metadataProvider.Obtener().ConfigureAwait(false));
        }



        [HttpPost]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        public async Task<ActionResult<ValidadorNumero>> Post([FromBody]ValidadorNumero entidad)
        {

            Console.WriteLine(ModelState.IsValid.ToString() + "????");
            entidad = await servicioValidadorNumero.CrearAsync(entidad).ConfigureAwait(false);
            return Ok(CreatedAtAction("GetValidadorNumero", new { id = entidad.PropiedadId }, entidad).Value);
        }


        [HttpPut("{id}")]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        public async Task<IActionResult> Put(string id, [FromBody]ValidadorNumero entidad)
        {
            var x = ObtieneFiltrosIdentidad();


            if (id != entidad.PropiedadId)
            {
                return BadRequest();
            }

            await servicioValidadorNumero.ActualizarAsync(entidad).ConfigureAwait(false);
            return NoContent();

        }


        [HttpGet("page", Name = "GetPageValidadorNumero")]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        public async Task<ActionResult<IEnumerable<ValidadorNumero>>> GetPage([FromQuery]Consulta query = null)
        {
            Console.WriteLine("------------------------------------------------------");
            ///Añade las propiedaes del contexto para el filtro de ACL vía ACL Controller
            query.Filtros.AddRange(ObtieneFiltrosIdentidad());
            var data = await servicioValidadorNumero.ObtenerPaginadoAsync(query).ConfigureAwait(false);
            return Ok(data.Elementos.ToList<ValidadorNumero>());
        }


        //----------------------------------------------------------------------------------------------------------
        //----------------------------------------------------------------------------------------------------------
        //----------------------------------------------------------------------------------------------------------
        //----------------------------------------------------------------------------------------------------------




        [HttpGet("datatables", Name = "GetDatatablesPluginValidadorNumero")]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        public async Task<ActionResult<RespuestaDatatables<ValidadorNumero>>> GetDatatablesPlugin([ModelBinder(typeof(DatatablesModelBinder))]SolicitudDatatables query = null)
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

            var data = await servicioValidadorNumero.ObtenerPaginadoAsync(newq).ConfigureAwait(false);

            RespuestaDatatables<ValidadorNumero> r = new RespuestaDatatables<ValidadorNumero>()
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
        public async Task<ActionResult<ValidadorNumero>> Get(string id)
        {
            var o = await servicioValidadorNumero.UnicoAsync(x => x.PropiedadId== id).ConfigureAwait(false);
            if (o != null) return Ok(o);
            return NotFound(id);
        }




        [HttpDelete("{id}")]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        public async Task<ActionResult> Delete([FromBody]string[] id)
        {
            await servicioValidadorNumero.Eliminar(id).ConfigureAwait(false);
            return NoContent();
        }
    }
}