using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PIKA.Constantes.Aplicaciones.Aplicaciones;
using PIKA.GD.API.Filters;
using PIKA.GD.API.Model;
using PIKA.Infraestructura.Comun;
using PIKA.Infraestructura.Comun.Seguridad;
using PIKA.Modelo.Aplicacion.Tareas;
using PIKA.Modelo.Metadatos;
using PIKA.Servicio.AplicacionPlugin.Interfaces;
using RepositorioEntidades;


namespace PIKA.GD.API.Controllers.AplicacionColaTareaEnDemanda
{
    [Authorize]
    [ApiVersion("1.0")]
    [ApiController]
    [Route("api/v{version:apiVersion}/Apli/[controller]")]
    public class TareaEnDemandaController : ACLController
    {
        private ILogger<TareaEnDemandaController> logger;
        private IServicioTareaEnDemanda servicioColaTareaEnDemanda;
        private IProveedorMetadatos<ColaTareaEnDemanda> metadataProvider;
        private readonly IServicioTokenSeguridad ServicioTokenSeguridad;
        public TareaEnDemandaController(
            ILogger<TareaEnDemandaController> logger,
            IProveedorMetadatos<ColaTareaEnDemanda> metadataProvider,
            IServicioCache servicioCache,
            IServicioTareaEnDemanda servicioColaTareaEnDemanda,
            IServicioTokenSeguridad ServicioTokenSeguridad)
        {
            this.logger = logger;
            this.servicioColaTareaEnDemanda = servicioColaTareaEnDemanda;
            this.metadataProvider = metadataProvider;
            this.ServicioTokenSeguridad = ServicioTokenSeguridad;
        }

        [HttpGet("metadata", Name = "MetadataColaTareaEnDemanda")]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        public async Task<ActionResult<MetadataInfo>> GetMetadata([FromQuery]Consulta query = null)
        {
            try
            {
                return Ok(await metadataProvider.Obtener().ConfigureAwait(false));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{ex}");
                throw;
            }
            
        }



        [HttpGet("page", Name = "GetPageColaTareaEnDemanda")]
        public async Task<ActionResult<Paginado<ColaTareaEnDemanda>>> GetPage([ModelBinder(typeof(GenericDataPageModelBinder))][FromQuery]Consulta query = null)
        {


            query.Filtros.AddRange(ObtieneFiltrosIdentidad());

            ///Añade las propiedaes del contexto para el filtro de ACL vía ACL Controller
            var data = await servicioColaTareaEnDemanda.ObtenerPaginadoAsync(
                Query: query,
                include: null)
                .ConfigureAwait(false);

            return Ok(data);
        }


        [HttpGet("page", Name = "GetPageColaTareaEnDemandaFinalizadas")]
        public async Task<ActionResult<Paginado<ColaTareaEnDemanda>>> GetPageFinalizadas([ModelBinder(typeof(GenericDataPageModelBinder))][FromQuery] Consulta query = null)
        {

            servicioColaTareaEnDemanda.usuario = this.usuario;


            query.Filtros.AddRange(ObtieneFiltrosIdentidad());
            query.Filtros.Add(new FiltroConsulta() { Propiedad = "Completada", Valor = "true", ValorString = "true" });
            query.Filtros.Add(new FiltroConsulta() { Propiedad = "Recogida", Valor = "false", ValorString = "false" });

            ///Añade las propiedaes del contexto para el filtro de ACL vía ACL Controller
            var data = await servicioColaTareaEnDemanda.ObtenerPaginadoAsync(
                Query: query,
                include: null)
                .ConfigureAwait(false);

            return Ok(data);
        }


    }
}