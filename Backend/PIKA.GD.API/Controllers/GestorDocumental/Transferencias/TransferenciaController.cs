﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using DocumentFormat.OpenXml.Drawing;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;
using PIKA.GD.API.Filters;
using PIKA.GD.API.Model;
using PIKA.Infraestructura.Comun.Seguridad;
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
    [Route("api/v{version:apiVersion}/gd/[controller]")]
    public class TransferenciaController : ACLController
    {
        private IServicioTransferencia servicioTransferencia;
        private IProveedorMetadatos<Transferencia> metadataProvider;
        public TransferenciaController(ILogger<TransferenciaController> logger,
            IProveedorMetadatos<Transferencia> metadataProvider,
            IServicioTransferencia servicioTransferencia)
        {
            this.servicioTransferencia = servicioTransferencia;
            this.metadataProvider = metadataProvider;
        }

        public override void EmiteConfiguracionSeguridad(UsuarioAPI usuario, ContextoRegistroActividad RegistroActividad, List<EventoAuditoriaActivo> Eventos)
        {
            servicioTransferencia.EstableceContextoSeguridad(usuario, RegistroActividad, Eventos);
        }


        [HttpGet("page/archivo/{Id}/texto/{texto}", Name = "GetPageTransferenciaPorTexto")]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        public async Task<ActionResult<Paginado<Activo>>> GetPageTransferenciaPorTexto(string Id, string texto, [ModelBinder(typeof(GenericDataPageModelBinder))][FromQuery] Consulta query = null)
        {
            query.Filtros.Add(new FiltroConsulta()
            {
                Operador = FiltroConsulta.OP_EQ,
                Propiedad = "ArchivoId",
                Valor = Id
            });

            var data = await servicioTransferencia.ObtenerPaginadoAsync(WebUtility.UrlDecode(texto), Query: query, include: null).ConfigureAwait(false);

            return Ok(data);
        }


        /// <summary>
        /// Obtiene los metadatos relacionados con la 
        /// entidad Transferencia
        /// </summary>
        /// <returns></returns>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpGet("metadata", Name = "MetadataTransferencia")]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        public async Task<ActionResult<MetadataInfo>> GetMetadata([FromQuery]Consulta query = null)
        {
            return Ok(await metadataProvider.Obtener().ConfigureAwait(false));
        }


        /// <summary>
        /// Añade una nueva entidad 
        /// del Transferencia
        /// </summary>
        /// <param name="entidad"></param>
        /// <returns></returns>
        [HttpPost]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        [ProducesResponseType(StatusCodes.Status200OK)]

        public async Task<ActionResult<Transferencia>> Post([FromBody] Transferencia entidad)
        {
           entidad.UsuarioId = this.UsuarioId;
            entidad.EstadoTransferenciaId = EstadoTransferencia.ESTADO_NUEVA;
            entidad.FechaCreacion = DateTime.UtcNow;
            entidad.CantidadActivos = 0;
            if (string.IsNullOrEmpty(entidad.TemaId))
            {
                entidad = await servicioTransferencia.CrearAsync(entidad).ConfigureAwait(false);

            } else
            {
                entidad = await servicioTransferencia.CrearDesdeTemaAsync(entidad, entidad.TemaId, entidad.EliminarTema).ConfigureAwait(false);
            }
            return Ok(CreatedAtAction("GetTransferencia", new { id = entidad.Id.Trim() }, entidad).Value);
        }

        [HttpPost("webcommand/{command}")]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        public async Task<ActionResult<RespuestaComandoWeb>> Post(string command, [FromBody] object payload)
        {
            RespuestaComandoWeb r = await servicioTransferencia.ComandoWeb(command, payload).ConfigureAwait(false);
            return Ok(r);
        }


        /// <summary>
        /// Actualiza unq entidad Transferencia,
        /// el Id debe incluirse en el Querystring así como en 
        /// el serializado para la petición PUT
        /// </summary>
        /// <param name="id">Identificador único del dominio</param>
        /// <param name="entidad">Datos serialziados de la OU</param>
        /// <returns></returns>

        [HttpPut("{id}")]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> Put(string id, [FromBody]Transferencia entidad)
        {
            if (id.Trim() != entidad.Id.Trim())
            {
                return BadRequest();
            }

            await servicioTransferencia.ActualizarAsync(entidad).ConfigureAwait(false);
            return NoContent();

        }

        /// <summary>
        /// Devulve un alista de  Transferencia asociadas al objeto del tipo especificado
        /// </summary>
        /// <param name="query">Consulta para la paginación y búsqueda</param>
        /// <returns></returns>

        [HttpGet("page/archivo/{id}", Name = "GetPageTransferencia")]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<Transferencia>>> GetPage(string id, [FromQuery] Consulta query = null)
        {
            query.Filtros.Add(new FiltroConsulta() { Operador = "eq", Propiedad = "ArchivoOrigenId", Valor = id });
            var data = await servicioTransferencia.ObtenerPaginadoAsync(
                    Query: query,
                    include: null)
                    .ConfigureAwait(false);

            return Ok(data);
        }

        /// <summary>
        /// Obtiene un Transferencia en base al Id único
        /// </summary>
        /// <param name="id">Id único del Estado Cuadro Clasificacion</param>
        /// <returns></returns>
        [HttpGet("{id}")]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        [ProducesResponseType(StatusCodes.Status200OK)]

        public async Task<ActionResult<Transferencia>> Get(string id)
        {
            var o = await servicioTransferencia.UnicoAsync(x => x.Id.Trim() == id.Trim()).ConfigureAwait(false);
            if (o != null) return Ok(o);
            return NotFound(id);
        }
        /// <summary>
        /// Obtiene un Transferencia en base al Id único
        /// </summary>
        /// <param name="id">Id único del Estado Cuadro Clasificacion</param>
        /// <returns></returns>
        [HttpGet("pageReporte", Name = "GetReporteTransferencia")]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<Transferencia>> GetReporte(string TransferenciaId,string? columnas)
        {
            string[] Cols;
            if (!string.IsNullOrEmpty(columnas))
               Cols = columnas.Split(',').ToList().Where(x => !string.IsNullOrEmpty(x)).ToArray();
            else
                Cols = "EntradaClasificacion.Clave,EntradaClasificacion.Nombre,Nombre,Asunto,FechaApertura,FechaCierre,CodigoOptico,CodigoElectronico,Reservado,Confidencial,Ampliado".Split(',').ToList().Where(x => !string.IsNullOrEmpty(x)).ToArray();
            var o =  servicioTransferencia.ReporteTransferencia(TransferenciaId, Cols);
            if (o != null) return Ok(o);
            return NotFound(TransferenciaId);
        }
        /// <summary>
        /// Elimina de manera permanente un Transferencia en base al arreglo de identificadores recibidos
        /// </summary>
        /// <param name="ids">Arreglo de identificadores string</param>
        /// <returns></returns>
        [HttpDelete("{ids}")]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult> Delete(string ids)
        {
            string IdsTrim = "";
            foreach (string item in ids.Split(',').ToList().Where(x => !string.IsNullOrEmpty(x)).ToArray())
            {
                IdsTrim += item.Trim() + ",";
            }
            string[] Ids = IdsTrim.Split(',').ToList().Where(x => !string.IsNullOrEmpty(x)).ToArray();

            return Ok(await servicioTransferencia.Eliminar(Ids).ConfigureAwait(false));
        }

    }
}