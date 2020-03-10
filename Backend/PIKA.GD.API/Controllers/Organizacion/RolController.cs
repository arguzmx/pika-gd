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
using PIKA.Modelo.Metadatos;
using PIKA.Modelo.Organizacion;
using PIKA.Servicio.Organizacion.Interfaces;
using RepositorioEntidades;
using RepositorioEntidades.DatatablesPlugin;

namespace PIKA.GD.API.Controllers.Organizacion
{

    [Authorize]
    [ApiVersion("1.0")]
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class RolController : ACLController
    {
        private ILogger<RolController> logger;
        private IServicioRol servicioRol;
        private IMetadataProvider<Rol> metadataProvider;

        public RolController(ILogger<RolController> logger,
          IMetadataProvider<Rol> metadataProvider,
          IServicioRol servicioRol)
        {
            this.logger = logger;
            this.servicioRol = servicioRol;
            this.metadataProvider = metadataProvider;
        }

        [HttpGet("metadata", Name = "MetadataRol")]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        public async Task<ActionResult<MetadataInfo>> GetMetadata([FromQuery]Consulta query = null)
        {
            return Ok(await metadataProvider.Obtener().ConfigureAwait(false));
        }


        [HttpPost]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        public async Task<ActionResult<Rol>> Post([FromBody]Rol entidad)
        {
            entidad = await servicioRol.CrearAsync(entidad).ConfigureAwait(false);
            return Ok(CreatedAtAction("GetRol", new { id = entidad.Id }, entidad).Value);
        }

        [HttpPut("{id}")]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        public async Task<IActionResult> Put(string id, [FromBody]Rol entidad)
        {
            var x = ObtieneFiltrosIdentidad();

            if (id != entidad.Id)
            {
                return BadRequest();
            }

            await servicioRol.ActualizarAsync(entidad).ConfigureAwait(false);
            return NoContent();
        }

        [HttpGet("page", Name = "GetPageRol")]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        public async Task<ActionResult<IEnumerable<Dominio>>> GetPage([FromQuery]Consulta query = null)
        {
            Console.WriteLine("GETPAGING rol");
            ///Añade las propiedaes del contexto para el filtro de ACL vía ACL Controller
            query.Filtros.AddRange(ObtieneFiltrosIdentidad());
            var data = await servicioRol.ObtenerPaginadoAsync(query).ConfigureAwait(false);
            return Ok(data.Elementos.ToList<Rol>());
        }

        //----------------------------------------------------------------------------------------------------------
        //----------------------------------------------------------------------------------------------------------
        //----------------------------------------------------------------------------------------------------------
        //----------------------------------------------------------------------------------------------------------


        [HttpGet("datatables", Name = "GetDatatablesPluginRol")]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        public async Task<ActionResult<RespuestaDatatables<Rol>>> GetDatatablesPlugin([ModelBinder(typeof(DatatablesModelBinder))]SolicitudDatatables query = null)
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

            var data = await servicioRol.ObtenerPaginadoAsync(newq).ConfigureAwait(false);

            RespuestaDatatables<Rol> r = new RespuestaDatatables<Rol>()
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
        public async Task<ActionResult<Rol>> Get(string id)
        {
            var o = await servicioRol.UnicoAsync(x => x.Id == id).ConfigureAwait(false);
            if (o != null) return Ok(o);
            return NotFound(id);
        }


        [HttpDelete("{id}")]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        public async Task<ActionResult> Delete([FromBody]string[] id)
        {
            await servicioRol.Eliminar(id).ConfigureAwait(false);
            return NoContent();
        }
    }
}
