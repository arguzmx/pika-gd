using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
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
using PIKA.Servicio.GestionDocumental.Interfaces.Topologia;
using RepositorioEntidades;


namespace PIKA.GD.API.Controllers.GestorDocumental
{

    [Authorize]
    [ApiVersion("1.0")]
    [ApiController]
    [Route("api/v{version:apiVersion}/org/[controller]")]
    public class ActivoContenedorAlmacenController : ACLController
    {
        private ILogger<ActivoContenedorAlmacenController> logger;
        private IServicioActivoContenedorAlmacen servicioActivoContenedorAlmacen;
        private IServicioContenedorAlmacen servicioContenedorAlmacen;
        private IProveedorMetadatos<ActivoContenedorAlmacen> metadataProvider;

        public ActivoContenedorAlmacenController(ILogger<ActivoContenedorAlmacenController> logger,
          IServicioContenedorAlmacen servicioContenedorAlmacen,
          IProveedorMetadatos<ActivoContenedorAlmacen> metadataProvider,
          IServicioActivoContenedorAlmacen servicioActivoContenedorAlmacen)
        {
            this.logger = logger;
            this.servicioActivoContenedorAlmacen = servicioActivoContenedorAlmacen;
            this.metadataProvider = metadataProvider;
            this.servicioContenedorAlmacen = servicioContenedorAlmacen;
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

            var cont = await servicioContenedorAlmacen.UnicoAsync(x => x.Id == id);
            if (cont != null)
            {
                filtros.Add(new FiltroConsulta()
                {
                    Negacion = false,
                    NivelFuzzy = 0,
                    Operador = FiltroConsulta.OP_EQ,
                    Propiedad = "ArchivoId",
                    Valor = cont.ArchivoId,
                    ValorString = cont.ArchivoId
                });

            } else
            {
                filtros.Add(new FiltroConsulta()
                {
                    Negacion = false,
                    NivelFuzzy = 0,
                    Operador = FiltroConsulta.OP_EQ,
                    Propiedad = "ArchivoId",
                    Valor = "-",
                    ValorString = "-"
                });
            }

            f.Filtros = filtros;
            propiedades.Add(f);

            return Ok(propiedades);

        }

        public override void EmiteConfiguracionSeguridad(UsuarioAPI usuario, ContextoRegistroActividad RegistroActividad, List<EventoAuditoriaActivo> Eventos)
        {
            servicioActivoContenedorAlmacen.EstableceContextoSeguridad(usuario, RegistroActividad, Eventos);
        }

        /// <summary>
        /// Obtiene los metadatos relacionados con la entidad ActivoContenedorAlmacen
        /// </summary>
        /// <returns></returns>
        [HttpGet("metadata", Name = "MetadataActivoContenedorAlmacen")]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<MetadataInfo>> GetMetadata([FromQuery] Consulta query = null)
        {
            return Ok(await metadataProvider.Obtener().ConfigureAwait(false));
        }


        /// <summary>
        /// Añade una nueva entidad del tipo ActivoContenedorAlmacen
        /// </summary>
        /// <param name="entidad"></param>
        /// <returns></returns>
        [HttpPost]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<ActivoContenedorAlmacen>> Post([FromBody] ActivoContenedorAlmacen entidad)
        {
            entidad = await servicioActivoContenedorAlmacen.CrearAsync(entidad).ConfigureAwait(false);
            return Ok(entidad);
        }


        /// <summary>
        /// Vincula una lista de IDs al objeto ActivoContenedorAlmacen
        /// </summary>
        /// <param name="entidad"></param>
        /// <returns></returns>
        [HttpPost("{contenedoralmacenid}", Name = "PostMultipleActivoContenedorAlmacen")]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<int>> PostIds(string contenedoralmacenid, [FromBody] string[] ids)
        {
            int cantidad = await servicioActivoContenedorAlmacen.PostIds(contenedoralmacenid, ids).ConfigureAwait(false);
            return Ok(cantidad);
        }



        /// <summary>
        /// Obtiene una página de datos del dominio 
        /// </summary>
        /// <param name="query">Consulta para el paginado</param>
        /// <returns></returns>
        [HttpGet("page/{tipo}/{id}", Name = "GetPageActivoContenedorAlmacenes")]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<Paginado<ActivoContenedorAlmacen>>> GetPage(
              string tipo, string id, 
              [ModelBinder(typeof(GenericDataPageModelBinder))][FromQuery]Consulta query = null)
        {
            ///Añade las propiedaes del contexto para el filtro de ACL vía ACL Controller
            query.Filtros.Add(new FiltroConsulta() { Propiedad = "ContenedorAlmacenId", Operador = FiltroConsulta.OP_EQ, Valor = id });

            var data = await servicioActivoContenedorAlmacen.ObtenerPaginadoAsync(query).ConfigureAwait(false);
            return Ok(data);
        }



        /// <summary>
        /// Marca como eliminados una lista de dominiios en base al arreglo de identificadores recibidos
        /// </summary>
        /// <param name="ids">Arreglo de identificadores string</param>
        /// <returns></returns>
        [HttpDelete("{contenedoralmacenid}/{ids}")]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult> Delete(string contenedoralmacenid, string ids)
        {
            string IdsTrim = "";
            foreach (string item in ids.Split(',').ToList().Where(x => !string.IsNullOrEmpty(x)).ToArray())
            {
                IdsTrim += item.Trim() + ",";
            }
            string[] lids = IdsTrim.Split(',').ToList()
           .Where(x => !string.IsNullOrEmpty(x)).ToArray();
            return Ok(await servicioActivoContenedorAlmacen.DeleteIds(contenedoralmacenid, lids).ConfigureAwait(false));
        }

    }
}
