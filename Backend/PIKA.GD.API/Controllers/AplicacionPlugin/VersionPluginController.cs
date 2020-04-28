﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PIKA.GD.API.Filters;
using PIKA.GD.API.Model;
using PIKA.Modelo.Aplicacion.Plugins;
using PIKA.Modelo.Metadatos;
using PIKA.Servicio.AplicacionPlugin.Interfaces;
using RepositorioEntidades;
using RepositorioEntidades.DatatablesPlugin;

namespace PIKA.GD.API.Controllers.AplicacionPlugin
{
    [Authorize]
    [ApiVersion("1.0")]
    [ApiController]
    [Route("api/v{version:apiVersion}/Apli/[controller]")]
    public class VersionPluginController : ACLController
    {
        private ILogger<VersionPluginController> logger;
        private IServicioVersionPlugin servicioEntidad;
        private IProveedorMetadatos<VersionPlugin> metadataProvider;
        public VersionPluginController(ILogger<VersionPluginController> logger,
            IProveedorMetadatos<VersionPlugin> metadataProvider,
            IServicioVersionPlugin servicioEntidad)
        {
            this.logger = logger;
            this.servicioEntidad = servicioEntidad;
            this.metadataProvider = metadataProvider;
        }

        [HttpGet("metadata", Name = "MetadataVersionPlugin")]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        public async Task<ActionResult<MetadataInfo>> GetMetadata([FromQuery]Consulta query = null)
        {
            return Ok(await metadataProvider.Obtener().ConfigureAwait(false));
        }



        [HttpPost]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        public async Task<ActionResult<VersionPlugin>> Post([FromBody]VersionPlugin entidad)
        {

            entidad = await servicioEntidad.CrearAsync(entidad).ConfigureAwait(false);
            return Ok(CreatedAtAction("GetVersionPlugin", new { id = entidad.Id }, entidad).Value);
        }


        [HttpPut("{id}")]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        public async Task<IActionResult> Put(string id, [FromBody]VersionPlugin entidad)
        {
            var x = ObtieneFiltrosIdentidad();
            if (id != entidad.Id)
            {
              
                return BadRequest();
            }

            await servicioEntidad.ActualizarAsync(entidad).ConfigureAwait(false);
            return NoContent();

        }


        [HttpGet("page", Name = "GetPageVersionPlugin")]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        public async Task<ActionResult<IEnumerable<VersionPlugin>>> GetPage([ModelBinder(typeof(GenericDataPageModelBinder))][FromQuery]Consulta query = null)
        {
            ///Añade las propiedaes del contexto para el filtro de ACL vía ACL Controller
            query.Filtros.AddRange(ObtieneFiltrosIdentidad());
            var data = await servicioEntidad.ObtenerPaginadoAsync(query).ConfigureAwait(false);
            return Ok(data.Elementos.ToList<VersionPlugin>());
        }


        //----------------------------------------------------------------------------------------------------------
        //----------------------------------------------------------------------------------------------------------
        //----------------------------------------------------------------------------------------------------------
        //----------------------------------------------------------------------------------------------------------




        [HttpGet("datatables", Name = "GetDatatablesPluginVersionPlugin")]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        public async Task<ActionResult<RespuestaDatatables<VersionPlugin>>> GetDatatablesPlugin([ModelBinder(typeof(DatatablesModelBinder))]SolicitudDatatables query = null)
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

            var data = await servicioEntidad.ObtenerPaginadoAsync(newq).ConfigureAwait(false);

            RespuestaDatatables<VersionPlugin> r = new RespuestaDatatables<VersionPlugin>()
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
        public async Task<ActionResult<VersionPlugin>> Get(string id)
        {
            var o = await servicioEntidad.UnicoAsync(x => x.Id == id).ConfigureAwait(false);
           
            if (o != null) return Ok(o);
            return NotFound(id);
        }




        //[HttpDelete("{id}")]
        [HttpDelete]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        public async Task<ActionResult> Delete([FromBody]string[] id)
        {
            return Ok(await servicioEntidad.Eliminar(id).ConfigureAwait(false));
        }

    }
}