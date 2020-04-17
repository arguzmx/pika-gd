﻿using System;
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
    public class AsociacionPlantillaController : ACLController
    {
        private ILogger<AsociacionPlantillaController> logger;
        private IServicioAsociacionPlantilla servicioAsociacionPlantilla;
        private IProveedorMetadatos<AsociacionPlantilla> metadataProvider;
        public AsociacionPlantillaController(ILogger<AsociacionPlantillaController> logger,
            IProveedorMetadatos<AsociacionPlantilla> metadataProvider,
            IServicioAsociacionPlantilla servicioAsociacionPlantilla)
        {
            this.logger = logger;
            this.servicioAsociacionPlantilla = servicioAsociacionPlantilla;
            this.metadataProvider = metadataProvider;
        }

        [HttpGet("metadata", Name = "MetadataAsociacionPlantilla")]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        public async Task<ActionResult<MetadataInfo>> GetMetadata([FromQuery]Consulta query = null)
        {
            return Ok(await metadataProvider.Obtener().ConfigureAwait(false));
        }



        [HttpPost]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        public async Task<ActionResult<AsociacionPlantilla>> Post([FromBody]AsociacionPlantilla entidad)
        {

            entidad = await servicioAsociacionPlantilla.CrearAsync(entidad).ConfigureAwait(false);
            return Ok(CreatedAtAction("GetAsociacionPlantilla", new { id = entidad.Id }, entidad).Value);
        }


        [HttpPut("{id}")]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        public async Task<IActionResult> Put(string id, [FromBody]AsociacionPlantilla entidad)
        {
            var x = ObtieneFiltrosIdentidad();


            if (id != entidad.Id)
            {
                return BadRequest();
            }

            await servicioAsociacionPlantilla.ActualizarAsync(entidad).ConfigureAwait(false);
            return NoContent();

        }


        [HttpGet("page", Name = "GetPageAsociacionPlantilla")]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        public async Task<ActionResult<IEnumerable<AsociacionPlantilla>>> GetPage([FromQuery]Consulta query = null)
        {
            ///Añade las propiedaes del contexto para el filtro de ACL vía ACL Controller
            query.Filtros.AddRange(ObtieneFiltrosIdentidad());
            var data = await servicioAsociacionPlantilla.ObtenerPaginadoAsync(query).ConfigureAwait(false);
            return Ok(data.Elementos.ToList<AsociacionPlantilla>());
        }


        //----------------------------------------------------------------------------------------------------------
        //----------------------------------------------------------------------------------------------------------
        //----------------------------------------------------------------------------------------------------------
        //----------------------------------------------------------------------------------------------------------




        [HttpGet("datatables", Name = "GetDatatablesPluginAsociacionPlantilla")]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        public async Task<ActionResult<RespuestaDatatables<AsociacionPlantilla>>> GetDatatablesPlugin([ModelBinder(typeof(DatatablesModelBinder))]SolicitudDatatables query = null)
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

            var data = await servicioAsociacionPlantilla.ObtenerPaginadoAsync(newq).ConfigureAwait(false);

            RespuestaDatatables<AsociacionPlantilla> r = new RespuestaDatatables<AsociacionPlantilla>()
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
        public async Task<ActionResult<AsociacionPlantilla>> Get(string id)
        {
            var o = await servicioAsociacionPlantilla.UnicoAsync(x => x.Id == id).ConfigureAwait(false);
            if (o != null) return Ok(o);
            return NotFound(id);
        }




        [HttpDelete("{id}")]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        public async Task<ActionResult> Delete([FromBody]string[] id)
        {
            await servicioAsociacionPlantilla.Eliminar(id).ConfigureAwait(false);
            return NoContent();
        }
    }
}