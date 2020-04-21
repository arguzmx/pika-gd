using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PIKA.GD.API.Filters;
using PIKA.GD.API.Model;
using PIKA.Modelo.GestorDocumental;
using PIKA.Modelo.Metadatos;
using PIKA.Servicio.GestionDocumental.Interfaces;
using RepositorioEntidades;
using RepositorioEntidades.DatatablesPlugin;

namespace PIKA.GD.API.Controllers.GestorDocumental
{
    [Authorize]
    [ApiVersion("1.0")]
    [ApiController]
    [Route("api/v{version:apiVersion}/gd/[controller]")]
    public class PrestamoController : ACLController
    {
        private readonly ILogger<PrestamoController> logger;
        private IServicioPrestamo servicioPrestamo;
        private IProveedorMetadatos<Prestamo> metadataProvider;

        private IServicioActivoPrestamo servicioActivoPrestamo;
        private IProveedorMetadatos<ActivoPrestamo> metadataProviderActivo;
        public PrestamoController(ILogger<PrestamoController> logger,
            IProveedorMetadatos<Prestamo> metadataProvider,
            IProveedorMetadatos<ActivoPrestamo> metadataProviderActivo,
            IServicioPrestamo servicioPrestamo,
            IServicioActivoPrestamo servicioActivoPrestamo)
        {
            this.logger = logger;
            this.servicioPrestamo = servicioPrestamo;
            this.servicioActivoPrestamo = servicioActivoPrestamo;
            this.metadataProvider = metadataProvider;
            this.metadataProviderActivo = metadataProviderActivo;
        }


        #region Prestamos
        [HttpGet("metadata", Name = "MetadataPrestamo")]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        public async Task<ActionResult<MetadataInfo>> GetMetadata([FromQuery]Consulta query = null)
        {
            return Ok(await metadataProvider.Obtener().ConfigureAwait(false));
        }



        [HttpPost]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        public async Task<ActionResult<Prestamo>> Post([FromBody]Prestamo entidad)
        {
            entidad = await servicioPrestamo.CrearAsync(entidad).ConfigureAwait(false);
            return Ok(CreatedAtAction("GetPrestamo", new { id = entidad.Id }, entidad).Value);
        }


        [HttpPut("{id}")]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        public async Task<IActionResult> Put(string id, [FromBody]Prestamo entidad)
        {
            var x = ObtieneFiltrosIdentidad();


            if (id != entidad.Id)
            {
                return BadRequest();
            }

            await servicioPrestamo.ActualizarAsync(entidad).ConfigureAwait(false);
            return NoContent();

        }


        [HttpGet("page", Name = "GetPagePrestamo")]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        public async Task<ActionResult<IEnumerable<Prestamo>>> GetPage([FromQuery]Consulta query = null)
        {
            ///Añade las propiedaes del contexto para el filtro de ACL vía ACL Controller
            query.Filtros.AddRange(ObtieneFiltrosIdentidad());
            var data = await servicioPrestamo.ObtenerPaginadoAsync(query).ConfigureAwait(false);
            return Ok(data.Elementos.ToList<Prestamo>());
        }


        //----------------------------------------------------------------------------------------------------------
        //----------------------------------------------------------------------------------------------------------
        //----------------------------------------------------------------------------------------------------------
        //----------------------------------------------------------------------------------------------------------




        [HttpGet("datatables", Name = "GetDatatablesPluginPrestamo")]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        public async Task<ActionResult<RespuestaDatatables<Prestamo>>> GetDatatablesPlugin([ModelBinder(typeof(DatatablesModelBinder))]SolicitudDatatables query = null)
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

            var data = await servicioPrestamo.ObtenerPaginadoAsync(newq).ConfigureAwait(false);

            RespuestaDatatables<Prestamo> r = new RespuestaDatatables<Prestamo>()
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
        public async Task<ActionResult<Prestamo>> Get(string id)
        {
            var o = await servicioPrestamo.UnicoAsync(x => x.Id == id).ConfigureAwait(false);
            if (o != null) return Ok(o);
            return NotFound(id);
        }




        [HttpDelete]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        public async Task<ActionResult> Delete([FromBody]string[] id)
        {
            return Ok(await servicioPrestamo.Eliminar(id).ConfigureAwait(false));
        }
        #endregion

        #region Activos de prestamo

        [HttpGet("metadata", Name = "MetadataActivoPrestamo")]
        [Route("{id}/Activo/metadata")]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        public async Task<ActionResult<MetadataInfo>> GetMetadataActivo([FromQuery]Consulta query = null)
        {
            return Ok(await metadataProviderActivo.Obtener().ConfigureAwait(false));
        }



        [HttpPost]
        [Route("{id}/Activo")]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        public async Task<ActionResult<ActivoPrestamo>> Post([FromBody]ActivoPrestamo entidad)
        {
            entidad = await servicioActivoPrestamo.CrearAsync(entidad).ConfigureAwait(false);
            return Ok(CreatedAtAction("GetActivoPrestamo", new { id = entidad.ActivoId, entidad.PrestamoId }, entidad).Value);
        }


        [HttpPut("{ActivoId}")]
        [Route("{id}/Activo/{ActivoId}")]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        public async Task<IActionResult> Put(string id, string ActivoId, [FromBody]ActivoPrestamo entidad)
        {
            var x = ObtieneFiltrosIdentidad();


            if (ActivoId != entidad.ActivoId || id != entidad.PrestamoId)
            {
                return BadRequest();
            }

            await servicioActivoPrestamo.ActualizarAsync(entidad).ConfigureAwait(false);
            return NoContent();

        }


        [HttpGet("page", Name = "GetPageActivoPrestamo")]
        [Route("{id}/Activo/page")]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        public async Task<ActionResult<IEnumerable<ActivoPrestamo>>> GetPageActivo([FromQuery]Consulta query = null)
        {
            ///Añade las propiedaes del contexto para el filtro de ACL vía ACL Controller
            query.Filtros.AddRange(ObtieneFiltrosIdentidad());
            var data = await servicioActivoPrestamo.ObtenerPaginadoAsync(query).ConfigureAwait(false);
            return Ok(data.Elementos.ToList<ActivoPrestamo>());
        }


        //----------------------------------------------------------------------------------------------------------
        //----------------------------------------------------------------------------------------------------------
        //----------------------------------------------------------------------------------------------------------
        //----------------------------------------------------------------------------------------------------------




        [HttpGet("datatables", Name = "GetDatatablesPluginActivoPrestamo")]
        [Route("{id}/Activo/datatables")]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        public async Task<ActionResult<RespuestaDatatables<ActivoPrestamo>>> GetDatatablesPluginActivo([ModelBinder(typeof(DatatablesModelBinder))]SolicitudDatatables query = null)
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

            var data = await servicioActivoPrestamo.ObtenerPaginadoAsync(newq).ConfigureAwait(false);

            RespuestaDatatables<ActivoPrestamo> r = new RespuestaDatatables<ActivoPrestamo>()
            {
                data = data.Elementos.ToArray(),
                draw = query.draw,
                error = "",
                recordsFiltered = data.Conteo,
                recordsTotal = data.Conteo
            };

            return Ok(r);
        }



        [HttpGet("{ActivoId}")]
        [Route("{id}/Activo/get")]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        public async Task<ActionResult<ActivoPrestamo>> Get(string ActivoId, string id)
        {
            var o = await servicioActivoPrestamo.UnicoAsync(x => x.ActivoId == ActivoId && x.PrestamoId == id).ConfigureAwait(false);
            if (o != null) return Ok(o);
            return NotFound(new { ActivoId, id });
        }


        [HttpDelete]
        [Route("{id}/Activo")]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        public async Task<ActionResult> DeleteActivo([FromBody]string[] id)
        {
            return Ok(await servicioActivoPrestamo.Eliminar(id).ConfigureAwait(false));
        }
        #endregion
    }
}