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
using PIKA.Servicio.Organizacion.Interfaces;
using RepositorioEntidades;
using RepositorioEntidades.DatatablesPlugin;

namespace PIKA.GD.API.Controllers.Organizacion
{



    [Authorize]
    [ApiVersion("1.0")]
    [ApiController]
    [Route("api/v{version:apiVersion}/org/[controller]")]
    public class DireccionPostalController : ACLController
    {

        private ILogger<DireccionPostalController> logger;
        private IServicioDireccionPostal servicioDirPost;
        private IMetadataProvider<DireccionPostal> metadataProvider;
        public DireccionPostalController(ILogger<DireccionPostalController> logger,
            IMetadataProvider<DireccionPostal> metadataProvider,
            IServicioDireccionPostal servicioDirPost)
        {
            this.logger = logger;
            this.servicioDirPost = servicioDirPost;
            this.metadataProvider = metadataProvider;
        }

        [HttpGet("metadata", Name = "MetadataDirPostal")]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        public async Task<ActionResult<MetadataInfo>> GetMetadata([FromQuery]Consulta query = null)
        {
            return Ok(await metadataProvider.Obtener().ConfigureAwait(false));
        }



        [HttpPost]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        public async Task<ActionResult<DireccionPostal>> Post([FromBody]DireccionPostal entidad)
        {
            entidad = await servicioDirPost.CrearAsync(entidad).ConfigureAwait(false);
            return Ok(CreatedAtAction("GetDomain", new { id = entidad.Id }, entidad).Value);
        }


        [HttpPut("{id}")]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        public async Task<IActionResult> Put(string id, [FromBody]DireccionPostal entidad)
        {
            var x = ObtieneFiltrosIdentidad();


            if (id != entidad.Id)
            {
                return BadRequest();
            }

            await servicioDirPost.ActualizarAsync(entidad).ConfigureAwait(false);
            return NoContent();

        }


        [HttpGet("page", Name = "GetPageDireccionPostal")]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        public async Task<ActionResult<IEnumerable<DireccionPostal>>> GetPage([FromQuery]Consulta query = null)
        {
            Console.WriteLine("GETPAGING DirPost");
            ///Añade las propiedaes del contexto para el filtro de ACL vía ACL Controller
            query.Filtros.AddRange(ObtieneFiltrosIdentidad());
            var data = await servicioDirPost.ObtenerPaginadoAsync(query).ConfigureAwait(false);
            return Ok(data.Elementos.ToList<DireccionPostal>());
        }


        //----------------------------------------------------------------------------------------------------------
        //----------------------------------------------------------------------------------------------------------
        //----------------------------------------------------------------------------------------------------------
        //----------------------------------------------------------------------------------------------------------




        [HttpGet("datatables", Name = "GetDatatablesPluginDirPostal")]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        public async Task<ActionResult<RespuestaDatatables<DireccionPostal>>> GetDatatablesPlugin([ModelBinder(typeof(DatatablesModelBinder))]SolicitudDatatables query = null)
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

            var data = await servicioDirPost.ObtenerPaginadoAsync(newq).ConfigureAwait(false);

            RespuestaDatatables<DireccionPostal> r = new RespuestaDatatables<DireccionPostal>()
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
        public async Task<ActionResult<DireccionPostal>> Get(string id)
        {
            var o = await servicioDirPost.UnicoAsync(x => x.Id == id).ConfigureAwait(false);
            if (o != null) return Ok(o);
            return NotFound(id);
        }




        [HttpDelete("{id}")]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        public async Task<ActionResult> Delete([FromBody]string[] id)
        {
            await servicioDirPost.Eliminar(id).ConfigureAwait(false);
            return NoContent();
        }

    }
}