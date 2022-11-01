using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Json.Net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
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
        private IServicioArchivo ServicioArchivo;
        private IProveedorMetadatos<ActivoTransferencia> metadataProvider;
        public ActivoTransferenciaController(ILogger<ActivoTransferenciaController> logger,
            IProveedorMetadatos<ActivoTransferencia> metadataProvider,
            IServicioActivoTransferencia servicioActivoTransferencia,
            IServicioTransferencia ServicioTransferencia,
            IServicioArchivo servicioArchivo)
        {
            this.logger = logger;
            this.servicioActivoTransferencia = servicioActivoTransferencia;
            this.metadataProvider = metadataProvider;
            this.ServicioTransferencia = ServicioTransferencia;
            this.ServicioArchivo = servicioArchivo;
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

        [HttpPost("webcommand/{command}")]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        public async Task<ActionResult<RespuestaComandoWeb>> Post(string command, [FromBody] object payload)
        {
            servicioActivoTransferencia.usuario = this.usuario;
            RespuestaComandoWeb r = await servicioActivoTransferencia.ComandoWeb(command, payload).ConfigureAwait(false);
            return Ok(r);
        }

        [HttpGet()]
        [Route("filtrobusqueda/{id}")]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        public async Task<ActionResult<List<FiltroConsultaPropiedad>>> OntieneFiltroBusqueda(string id)
        {
            servicioActivoTransferencia.usuario = this.usuario;

            List<FiltroConsultaPropiedad> propiedades = new List<FiltroConsultaPropiedad>();
            FiltroConsultaPropiedad f = new FiltroConsultaPropiedad()
            {
                PropiedadId = "ActivoId"
            };

            List<FiltroConsulta> filtros = new List<FiltroConsulta>();
            var transferencia = await this.ServicioTransferencia.UnicoAsync(x => x.Id == id);
            var archivo = await this.ServicioArchivo.UnicoAsync(x => x.Id == transferencia.ArchivoOrigenId);

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
                Propiedad = "RangoDias",
                Valor = transferencia.RangoDias.ToString(),
                ValorString = transferencia.RangoDias.ToString()
            });

            if (transferencia.CuadroClasificacionId != null)
            {
                filtros.Add(new FiltroConsulta()
                {
                    Negacion = false,
                    NivelFuzzy = 0,
                    Operador = FiltroConsulta.OP_EQ,
                    Propiedad = "CuadroClasificacionId",
                    Valor = transferencia.CuadroClasificacionId,
                    ValorString = transferencia.CuadroClasificacionId
                });
            }


            if (transferencia.EntradaClasificacionId != null)
            {
                filtros.Add(new FiltroConsulta()
                {
                    Negacion = false,
                    NivelFuzzy = 0,
                    Operador = FiltroConsulta.OP_EQ,
                    Propiedad = "EntradaClasificacionId",
                    Valor = transferencia.EntradaClasificacionId,
                    ValorString = transferencia.EntradaClasificacionId
                });
            }

            // Parametro dummy para forzar un tipo de búsqueda
            if (archivo.TipoArchivoId == TipoArchivo.IDARCHIVO_TRAMITE)
            {
                filtros.Add(new FiltroConsulta()
                {
                    Negacion = false,
                    NivelFuzzy = 0,
                    Operador = FiltroConsulta.OP_EQ,
                    Propiedad = "PSEUDO_TRANSFERIBLE_TRAMITE",
                    Valor = "true",
                    ValorString = "true"
                });

            }
            else
            {
                filtros.Add(new FiltroConsulta()
                {
                    Negacion = false,
                    NivelFuzzy = 0,
                    Operador = FiltroConsulta.OP_EQ,
                    Propiedad = "PSEUDO_TRANSFERIBLE_OTRO",
                    Valor = "true",
                    ValorString = "true"
                });
            }



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

        [HttpPost(Name = "PostActivoTransferencia")]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        [ProducesResponseType(StatusCodes.Status200OK)]

        public async Task<ActionResult<ActivoTransferencia>> Post([FromBody] ActivoTransferencia entidad)
        {
            servicioActivoTransferencia.usuario = this.usuario;
            entidad = await servicioActivoTransferencia.CrearAsync(entidad).ConfigureAwait(false);
            return Ok(CreatedAtAction("GetActivoTransferencia", new { ActivoId = entidad.ActivoId }, entidad).Value);
        }


        [HttpPut(Name = "PutActivoTransferencia")]
        [Route("{id}")]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        [ProducesResponseType(StatusCodes.Status200OK)]

        public async Task<ActionResult<ActivoTransferencia>> Put(string id, [FromBody] ActivoTransferencia entidad)
        {
            if (entidad.Id != id) return BadRequest();
            servicioActivoTransferencia.usuario = this.usuario;
            await servicioActivoTransferencia.ActualizarAsync(entidad).ConfigureAwait(false);
            return Ok();
        }

        [HttpDelete(Name = "EliminaTodosActivosTransferencia")]
        [RouteAttribute("vinculos/todos/{id}")]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        [ProducesResponseType(StatusCodes.Status200OK)]

        public async Task<ActionResult> EliminaTodosActivosTransferencia(string id)
        {
            servicioActivoTransferencia.usuario = this.usuario;
            await servicioActivoTransferencia.EliminarVinculosTodos(id).ConfigureAwait(false);
            return Ok();
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
            servicioActivoTransferencia.usuario = this.usuario;
            var data = await servicioActivoTransferencia.ObtenerPaginadoAsync(
                       Query: query,
                       include: null)
                       .ConfigureAwait(false);

            return Ok(data);
        }


        [HttpGet("page/transferencia/{Id}/texto/{texto}", Name = "GetPageActivoTransferenciaPorTexto")]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        public async Task<ActionResult<Paginado<Activo>>> GetPageActivoTransferenciaPorTexto(string Id, string texto, [ModelBinder(typeof(GenericDataPageModelBinder))][FromQuery] Consulta query = null)
        {
            query.Filtros.Add(new FiltroConsulta()
            {
                Operador = FiltroConsulta.OP_EQ,
                Propiedad = "TransferenciaId",
                Valor = Id
            });

            var data = await servicioActivoTransferencia.ObtenerPaginadoAsync(WebUtility.UrlDecode(texto), Query: query, include: null).ConfigureAwait(false);

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
        public async Task<ActionResult> Delete(string ids)
        {
            servicioActivoTransferencia.usuario = this.usuario;
            string IdsTrim = "";

            foreach (string item in ids.Split(',').ToList().Where(x => !string.IsNullOrEmpty(x)).ToArray())
            {
                IdsTrim += item.Trim() + ",";
            }
            string[] lids = IdsTrim.Split(',').ToList()
           .Where(x => !string.IsNullOrEmpty(x)).ToArray();
            return Ok(await servicioActivoTransferencia.EliminarActivoTransferencia(lids).ConfigureAwait(false));
        }
    }
}