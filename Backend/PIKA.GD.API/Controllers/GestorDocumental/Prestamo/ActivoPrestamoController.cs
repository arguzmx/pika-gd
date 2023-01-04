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
    public class ActivoPrestamoController : ACLController
    {
        private readonly ILogger<ActivoPrestamoController> logger;
        private IServicioActivoPrestamo servicioActivoPrestamo;
        private IServicioPrestamo servicioPrestamo;
        private IProveedorMetadatos<ActivoPrestamo> metadataProviderActivo;
        public ActivoPrestamoController(
            ILogger<ActivoPrestamoController> logger,
            IProveedorMetadatos<ActivoPrestamo> metadataProviderActivo,
            IServicioActivoPrestamo servicioActivoPrestamo,
            IServicioPrestamo servicioPrestamo)
        {
            this.logger = logger;
            this.servicioActivoPrestamo = servicioActivoPrestamo;
            this.metadataProviderActivo = metadataProviderActivo;
            this.servicioPrestamo = servicioPrestamo;
        }
        public override void EmiteConfiguracionSeguridad(UsuarioAPI usuario, ContextoRegistroActividad RegistroActividad, List<EventoAuditoriaActivo> Eventos)
        {
            servicioActivoPrestamo.EstableceContextoSeguridad(usuario, RegistroActividad, Eventos);
            servicioPrestamo.EstableceContextoSeguridad(usuario, RegistroActividad, Eventos);
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
            var prestamo = await this.servicioPrestamo.UnicoAsync(x => x.Id == id);
            filtros.Add(new FiltroConsulta()
            {
                Negacion = false,
                NivelFuzzy = 0,
                Operador = FiltroConsulta.OP_EQ,
                Propiedad = "ArchivoId",
                Valor = prestamo.ArchivoId,
                ValorString = prestamo.ArchivoId
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


        #region Activos de prestamo

        [HttpGet("metadataActivo", Name = "MetadataActivoPrestamo")]
        [Route("metadata")]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        public async Task<ActionResult<MetadataInfo>> GetMetadataActivo([FromQuery] Consulta query = null)
        {
            return Ok(await metadataProviderActivo.Obtener().ConfigureAwait(false));
        }

        [HttpPost]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        public async Task<ActionResult<ActivoPrestamo>> Post([FromBody] ActivoPrestamo entidad)
        {

            entidad = await servicioActivoPrestamo.CrearAsync(entidad).ConfigureAwait(false);
            return Ok(CreatedAtAction("GetActivoPrestamo", new { id = entidad.ActivoId, entidad.PrestamoId }, entidad).Value);
        }


        [HttpGet("ActivoPage", Name = "GetPageActivoPrestamo")]
        [Route("page/prestamo/{PrestamoId}")]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        public async Task<ActionResult<IEnumerable<ActivoPrestamo>>> GetPageActivo(string PrestamoId, [ModelBinder(typeof(GenericDataPageModelBinder))][FromQuery] Consulta query = null)
        {
            query.Filtros.Add(new FiltroConsulta()
            {
                Propiedad = "PrestamoId",
                Operador = "eq",
                Valor = PrestamoId
            });
            ///Añade las propiedaes del contexto para el filtro de ACL vía ACL Controller
            var data = await servicioActivoPrestamo.ObtenerPaginadoAsync(
                       Query: query,
                       include: null)
                       .ConfigureAwait(false);

            return Ok(data);
        }


        [HttpGet("Activo/{ActivoId}")]
        [Route("{PrestamoId}/Activo/{ActivoId}/get")]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        public async Task<ActionResult<ActivoPrestamo>> Get(string ActivoId, string PrestamoId)
        {
            var o = await servicioActivoPrestamo.UnicoAsync(x => x.ActivoId == ActivoId && x.PrestamoId == PrestamoId).ConfigureAwait(false);
            if (o != null) return Ok(o);
            return NotFound(new { ActivoId, PrestamoId });
        }


        [HttpDelete("{ids}", Name = "DeleteActivo")]
        [Route("{PrestamoId}/Activo")]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        public async Task<ActionResult> DeleteActivo(string ids)
        {
            string IdsTrim = "";
            foreach (string item in ids.Split(',').ToList().Where(x => !string.IsNullOrEmpty(x)).ToArray())
            {
                IdsTrim += item.Trim() + ",";
            }
            string[] lids = IdsTrim.Split(',').ToList()
           .Where(x => !string.IsNullOrEmpty(x)).ToArray();
            return Ok(await servicioActivoPrestamo.Eliminar(lids).ConfigureAwait(false));
        }
        #endregion
    }
}