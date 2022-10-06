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
using PIKA.Modelo.GestorDocumental;
using PIKA.Modelo.Metadatos;
using PIKA.Servicio.GestionDocumental.Interfaces;
using PIKA.Servicio.GestionDocumental.Servicios;
using RepositorioEntidades;


namespace PIKA.GD.API.Controllers.GestorDocumental
{
    [Authorize]
    [ApiVersion("1.0")]
    [ApiController]
    [Route("api/v{version:apiVersion}/gd/ActivoTransferencia")]
    public class ActivoTransferenciaController : ACLController
    {
        private readonly ILogger<ActivoTransferenciaController> logger;
        private IServicioActivoTransferencia servicioActivoTransferencia;
        private IServicioTransferencia ServicioTransferencia;
        private IProveedorMetadatos<ActivoTransferencia> metadataProvider;
        public ActivoTransferenciaController(ILogger<ActivoTransferenciaController> logger,
            IProveedorMetadatos<ActivoTransferencia> metadataProvider,
            IServicioActivoTransferencia servicioActivoTransferencia,
            IServicioTransferencia ServicioTransferencia)
        {
            this.logger = logger;
            this.servicioActivoTransferencia = servicioActivoTransferencia;
            this.metadataProvider = metadataProvider;
            this.ServicioTransferencia = ServicioTransferencia;
        }

        // <summary>
        /// Obtiene los metadatos relacionados con la 
        /// entidad Transferencia
        /// </summary>
        /// <returns></returns>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpGet("metadata", Name = "MetadataActivoTransferencia")]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        public async Task<ActionResult<MetadataInfo>> GetMetadata([FromQuery] Consulta query = null)
        {
            return Ok(await metadataProvider.Obtener().ConfigureAwait(false));
        }

        [HttpGet()]
        [Route("filtrobusqueda/{id}")]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        public async Task<ActionResult<List<FiltroConsultaPropiedad>>> OntieneFiltroBusqueda(string id)
        {

            List<FiltroConsultaPropiedad> propiedades = new List<FiltroConsultaPropiedad>();
            FiltroConsultaPropiedad f = new FiltroConsultaPropiedad()
            {
                PropiedadId = "ActivoId"
            };

            List<FiltroConsulta> filtros = new List<FiltroConsulta>();
            var transferencia = await this.ServicioTransferencia.UnicoAsync(x => x.Id == id);
            filtros.Add(new FiltroConsulta()
            {
                Negacion = false,
                NivelFuzzy = 0,
                Operador = FiltroConsulta.OP_EQ,
                Propiedad = "ArchivoId",
                Valor = transferencia.ArchivoOrigenId,
                ValorString = transferencia.ArchivoOrigenId
            });

            filtros.Add(new FiltroConsulta()
            {
                Negacion = false,
                NivelFuzzy = 0,
                Operador = FiltroConsulta.OP_EQ,
                Propiedad = "Eliminada",
                Valor = "false",
                ValorString = "false"
            });

            filtros.Add(new FiltroConsulta()
            {
                Negacion = false,
                NivelFuzzy = 0,
                Operador = FiltroConsulta.OP_EQ,
                Propiedad = "EnPrestamo",
                Valor = "false",
                ValorString = "false"
            });

            f.Filtros = filtros;
            propiedades.Add(f);

            return Ok(propiedades);
        }

        /// <summary>
        /// Añade una nueva entidad 
        /// del Transferencia
        /// </summary>
        /// <param name="entidad"></param>
        /// <returns></returns>

        [HttpPost("{ActivoId}", Name ="PostActivoTransferencia")]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        [ProducesResponseType(StatusCodes.Status200OK)]

        public async Task<ActionResult<ActivoTransferencia>> Post(string TransferenciaId,string ActivoId, [FromBody]ActivoTransferencia entidad)
        {
            if (TransferenciaId.Trim() != entidad.TransferenciaId.Trim() && ActivoId.Trim() != entidad.ActivoId.Trim())
            {
                return BadRequest();
            }


            entidad = await servicioActivoTransferencia.CrearAsync(entidad).ConfigureAwait(false);
            return Ok(CreatedAtAction("GetActivoTransferencia", new { ActivoId = entidad.ActivoId }, entidad).Value);
        }

        [HttpGet("ActivoTrasnferenciaPage", Name = "GetPageActivoTransferencia")]
        [Route("page/transferencia/{TransferenciaId}")]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        public async Task<ActionResult<IEnumerable<ActivoPrestamo>>> GetPageActivoTransferencia(string TransferenciaId, [ModelBinder(typeof(GenericDataPageModelBinder))][FromQuery] Consulta query = null)
        {
            query.Filtros.Add(new FiltroConsulta()
            {
                Propiedad = "TransferenciaId",
                Operador = "eq",
                Valor = TransferenciaId
            });
            ///Añade las propiedaes del contexto para el filtro de ACL vía ACL Controller
            var data = await servicioActivoTransferencia.ObtenerPaginadoAsync(
                       Query: query,
                       include: null)
                       .ConfigureAwait(false);

            return Ok(data);
        }

        /// <summary>
        /// Elimina de manera permanente un Activo Transferencia en base al arreglo de identificadores recibidos
        /// </summary>
        /// <param name="ids">Arreglo de identificadores string</param>
        /// <returns></returns>
        [HttpDelete("{Ids}", Name = "DeleteActivoTransferencia")]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult> Delete(string TransferenciaId, string ids)
        {
            string IdsTrim = "";
            TransferenciaId = TransferenciaId.Trim();
            foreach (string item in ids.Split(',').ToList().Where(x => !string.IsNullOrEmpty(x)).ToArray())
            {
                IdsTrim += item.Trim() + ",";
            }
            string[] lids = IdsTrim.Split(',').ToList()
           .Where(x => !string.IsNullOrEmpty(x)).ToArray();
            return Ok(await servicioActivoTransferencia.EliminarActivoTransferencia(TransferenciaId, lids).ConfigureAwait(false));
        }
    }
}