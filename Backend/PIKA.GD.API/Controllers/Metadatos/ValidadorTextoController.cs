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
    public class ValidadorTextoController : ACLController

    {
        private ILogger<ValidadorTextoController> logger;
        private IServicioValidadorTexto servicioValidadorTexto;
        private IProveedorMetadatos<ValidadorTexto> metadataProvider;
        public ValidadorTextoController(ILogger<ValidadorTextoController> logger,
            IProveedorMetadatos<ValidadorTexto> metadataProvider,
            IServicioValidadorTexto servicioValidadorTexto)
        {
            this.logger = logger;
            this.servicioValidadorTexto = servicioValidadorTexto;
            this.metadataProvider = metadataProvider;
        }

        [HttpGet("metadata", Name = "MetadataValidadorTexto")]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        public async Task<ActionResult<MetadataInfo>> GetMetadata([FromQuery]Consulta query = null)
        {
            return Ok(await metadataProvider.Obtener().ConfigureAwait(false));
        }



        [HttpPost]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        public async Task<ActionResult<ValidadorTexto>> Post([FromBody]ValidadorTexto entidad)
        {

            Console.WriteLine(ModelState.IsValid.ToString() + "????");
            entidad = await servicioValidadorTexto.CrearAsync(entidad).ConfigureAwait(false);
            return Ok(CreatedAtAction("GetValidadorTexto", new { id = entidad.PropiedadId }, entidad).Value);
        }


        [HttpPut("{id}")]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        public async Task<IActionResult> Put(string id, [FromBody]ValidadorTexto entidad)
        {
            var x = ObtieneFiltrosIdentidad();


            if (id != entidad.PropiedadId)
            {
                return BadRequest();
            }

            await servicioValidadorTexto.ActualizarAsync(entidad).ConfigureAwait(false);
            return NoContent();

        }


        [HttpGet("page", Name = "GetPageValidadorTexto")]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        public async Task<ActionResult<IEnumerable<ValidadorTexto>>> GetPage([FromQuery]Consulta query = null)
        {
            Console.WriteLine("------------------------------------------------------");
            ///Añade las propiedaes del contexto para el filtro de ACL vía ACL Controller
            query.Filtros.AddRange(ObtieneFiltrosIdentidad());
            var data = await servicioValidadorTexto.ObtenerPaginadoAsync(query).ConfigureAwait(false);
            return Ok(data.Elementos.ToList<ValidadorTexto>());
        }


        //----------------------------------------------------------------------------------------------------------
        //----------------------------------------------------------------------------------------------------------
        //----------------------------------------------------------------------------------------------------------
        //----------------------------------------------------------------------------------------------------------




        [HttpGet("datatables", Name = "GetDatatablesPluginValidadorTexto")]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        public async Task<ActionResult<RespuestaDatatables<ValidadorTexto>>> GetDatatablesPlugin([ModelBinder(typeof(DatatablesModelBinder))]SolicitudDatatables query = null)
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

            var data = await servicioValidadorTexto.ObtenerPaginadoAsync(newq).ConfigureAwait(false);

            RespuestaDatatables<ValidadorTexto> r = new RespuestaDatatables<ValidadorTexto>()
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
        public async Task<ActionResult<ValidadorTexto>> Get(string id)
        {
            var o = await servicioValidadorTexto.UnicoAsync(x => x.Id == id).ConfigureAwait(false);
            if (o != null) return Ok(o);
            return NotFound(id);
        }




        [HttpDelete("{id}")]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        public async Task<ActionResult> Delete([FromBody]string[] id)
        {
            await servicioValidadorTexto.Eliminar(id).ConfigureAwait(false);
            return NoContent();
        }
    }
}