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

    public class AtributoTablaController : ACLController

    {
        private ILogger<AtributoTablaController> logger;
        private IServicioAtributoTabla servicioAtributoTabla;
        private IProveedorMetadatos<AtributoTabla> metadataProvider;
        public AtributoTablaController(ILogger<AtributoTablaController> logger,
            IProveedorMetadatos<AtributoTabla> metadataProvider,
            IServicioAtributoTabla servicioAtributoTabla)
        {
            this.logger = logger;
            this.servicioAtributoTabla = servicioAtributoTabla;
            this.metadataProvider = metadataProvider;
        }

        [HttpGet("metadata", Name = "MetadataAtributoTabla")]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        public async Task<ActionResult<MetadataInfo>> GetMetadata([FromQuery]Consulta query = null)
        {
            return Ok(await metadataProvider.Obtener().ConfigureAwait(false));
        }



        [HttpPost]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        public async Task<ActionResult<AtributoTabla>> Post([FromBody]AtributoTabla entidad)
        {

            Console.WriteLine(ModelState.IsValid.ToString() + "????");
            entidad = await servicioAtributoTabla.CrearAsync(entidad).ConfigureAwait(false);
            return Ok(CreatedAtAction("GetAtributoTabla", new { id = entidad.PropiedadId }, entidad).Value);
        }


        [HttpPut("{id}")]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        public async Task<IActionResult> Put(string id, [FromBody]AtributoTabla entidad)
        {
            var x = ObtieneFiltrosIdentidad();


            if (id != entidad.PropiedadId)
            {
                return BadRequest();
            }

            await servicioAtributoTabla.ActualizarAsync(entidad).ConfigureAwait(false);
            return NoContent();

        }


        [HttpGet("page", Name = "GetPageAtributoTabla")]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        public async Task<ActionResult<IEnumerable<AtributoTabla>>> GetPage([FromQuery]Consulta query = null)
        {
            Console.WriteLine("------------------------------------------------------");
            ///Añade las propiedaes del contexto para el filtro de ACL vía ACL Controller
            query.Filtros.AddRange(ObtieneFiltrosIdentidad());
            var data = await servicioAtributoTabla.ObtenerPaginadoAsync(query).ConfigureAwait(false);
            return Ok(data.Elementos.ToList<AtributoTabla>());
        }


        //----------------------------------------------------------------------------------------------------------
        //----------------------------------------------------------------------------------------------------------
        //----------------------------------------------------------------------------------------------------------
        //----------------------------------------------------------------------------------------------------------




        [HttpGet("datatables", Name = "GetDatatablesPluginAtributoTabla")]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        public async Task<ActionResult<RespuestaDatatables<AtributoTabla>>> GetDatatablesPlugin([ModelBinder(typeof(DatatablesModelBinder))]SolicitudDatatables query = null)
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

            var data = await servicioAtributoTabla.ObtenerPaginadoAsync(newq).ConfigureAwait(false);

            RespuestaDatatables<AtributoTabla> r = new RespuestaDatatables<AtributoTabla>()
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
        public async Task<ActionResult<AtributoTabla>> Get(string id)
        {
            var o = await servicioAtributoTabla.UnicoAsync(x => x.Id == id).ConfigureAwait(false);
            if (o != null) return Ok(o);
            return NotFound(id);
        }




        [HttpDelete("{id}")]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        public async Task<ActionResult> Delete([FromBody]string[] id)
        {
            await servicioAtributoTabla.Eliminar(id).ConfigureAwait(false);
            return NoContent();
        }
    }
}