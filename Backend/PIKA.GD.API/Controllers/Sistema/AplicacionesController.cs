﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PIKA.GD.API.Filters;
using PIKA.GD.API.Model;
using PIKA.Infraestructura.Comun;
using PIKA.Infraestructura.Comun.Seguridad;
using PIKA.Servicio.AplicacionPlugin;
using PIKA.Servicio.AplicacionPlugin.Interfaces;
using PIKA.Servicio.Seguridad.Interfaces;
using RepositorioEntidades;

namespace PIKA.GD.API.Controllers.Sistema
{
    [ApiController]
    [Authorize]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/Sistema/[controller]")]
    public class SeguridadController : ACLController
    {

        private ILogger<SeguridadController> logger;
        private IServicioInfoAplicacion servicioAplicacion;
        private IServicioSeguridadAplicaciones servicioSeguridad;
        public SeguridadController(
            IServicioSeguridadAplicaciones servicioSeguridad,
            ILogger<SeguridadController> logger,
            IServicioInfoAplicacion servicioAplicacion)
        {
            this.servicioSeguridad = servicioSeguridad;
            this.logger = logger;
            this.servicioAplicacion = servicioAplicacion;
        }

        [HttpGet("aplicaciones", Name = "GetPageAplicaciones")]
        [TypeFilter(typeof(AsyncACLActionFilter),
            Arguments = new object[] { AplicacionRaiz.APP_ID, AplicacionAplicaciones.MODULO_APPS })]
        public async Task<ActionResult<IEnumerable<Aplicacion>>> GetPage()
        {
            logger.LogError(LocalizadorEnsamblados.ObtieneRutaBin());
            var data = await servicioAplicacion.OntieneAplicaciones(LocalizadorEnsamblados.ObtieneRutaBin())
                .ConfigureAwait(false);

            return Ok(data);
        }

        [HttpPost("permisos/aplicar", Name = "PostPermisosCrear")]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<int>> PostPermisosCrear([FromBody] List<PermisoAplicacion> permisos)
        {
            await servicioSeguridad.CrearActualizarAsync(permisos.ToArray()).ConfigureAwait(false);
            return Ok();
        }

        [HttpPost("permisos/eliminar", Name = "PostPermisosEliminar")]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<int>> PostPermisosEliminar([FromBody] List<PermisoAplicacion> permisos)
        {
            await servicioSeguridad.CrearActualizarAsync(permisos.ToArray()).ConfigureAwait(false);
            return Ok();
        }


        [HttpGet("permisos/{tipo}/{id}", Name = "GetPermisosPorTipo")]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<ICollection<PermisoAplicacion>>> GetPermisosPorTipo(string  tipo, string id)
        {
            var data = await servicioSeguridad.ObtienePermisosAsync(tipo, id).ConfigureAwait(false);
            return Ok(data);
        }


       
    }
}
