using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PIKA.Constantes.Aplicaciones.GestorDocumental;
using PIKA.GD.API.Filters;
using PIKA.GD.API.Model;
using PIKA.Infraestructura.Comun.Seguridad;
using PIKA.Modelo.GestorDocumental;
using PIKA.Modelo.Metadatos;
using PIKA.Servicio.GestionDocumental;
using PIKA.Servicio.GestionDocumental.Interfaces;
using RepositorioEntidades;


namespace PIKA.GD.API.Controllers.GestorDocumental
{
    [Authorize]
    [ApiVersion("1.0")]
    [ApiController]
    [Route("api/v{version:apiVersion}/gd/[controller]")]
    public class UnidadAdministrativaArchivoController : ACLController
    {
        private readonly ILogger<UnidadAdministrativaArchivoController> logger;
        private IServicioUnidadAdministrativaArchivo servicioUnidadAdmin;
        private IProveedorMetadatos<UnidadAdministrativaArchivo> metadataProvider;
        private readonly IServicioTokenSeguridad ServicioTokenSeguridad;
        public UnidadAdministrativaArchivoController(ILogger<UnidadAdministrativaArchivoController> logger,
            IProveedorMetadatos<UnidadAdministrativaArchivo> metadataProvider,
            IServicioUnidadAdministrativaArchivo servicioUnidadAdmin,
            IServicioTokenSeguridad ServicioTokenSeguridad)
        {
            this.logger = logger;
            this.servicioUnidadAdmin = servicioUnidadAdmin;
            this.metadataProvider = metadataProvider;
            this.ServicioTokenSeguridad = ServicioTokenSeguridad;
        }


        public override void EmiteConfiguracionSeguridad(UsuarioAPI usuario, ContextoRegistroActividad RegistroActividad, List<EventoAuditoriaActivo> Eventos)
        {
            servicioUnidadAdmin.EstableceContextoSeguridad(usuario, RegistroActividad, Eventos);
        }

        [HttpGet()]
        [Route("filtrobusqueda")]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        public async Task<ActionResult<List<FiltroConsultaPropiedad>>> OntieneFiltroBusqueda()
        {

            List<FiltroConsultaPropiedad> propiedades = new List<FiltroConsultaPropiedad>();

            FiltroConsultaPropiedad f = new FiltroConsultaPropiedad()
            {
                PropiedadId = "ArchivoConcentracionId"
            };

            List<FiltroConsulta> filtros = new List<FiltroConsulta>();
            filtros.Add(new FiltroConsulta()
            {
                Negacion = false,
                NivelFuzzy = 0,
                Operador = FiltroConsulta.OP_EQ,
                Propiedad = "OrigenId",
                Valor = this.contextoRegistro.UnidadOrgId,
                ValorString = this.contextoRegistro.UnidadOrgId
            });

            f.Filtros = filtros;
            propiedades.Add(f);


            f = new FiltroConsultaPropiedad()
            {
                PropiedadId = "ArchivoTramiteId"
            };

            filtros = new List<FiltroConsulta>();
            filtros.Add(new FiltroConsulta()
            {
                Negacion = false,
                NivelFuzzy = 0,
                Operador = FiltroConsulta.OP_EQ,
                Propiedad = "OrigenId",
                Valor = this.contextoRegistro.UnidadOrgId,
                ValorString = this.contextoRegistro.UnidadOrgId
            });

            f.Filtros = filtros;
            propiedades.Add(f);


            f = new FiltroConsultaPropiedad()
            {
                PropiedadId = "ArchivoHistoricoId"
            };

            filtros = new List<FiltroConsulta>();
            filtros.Add(new FiltroConsulta()
            {
                Negacion = false,
                NivelFuzzy = 0,
                Operador = FiltroConsulta.OP_EQ,
                Propiedad = "OrigenId",
                Valor = this.contextoRegistro.UnidadOrgId,
                ValorString = this.contextoRegistro.UnidadOrgId
            });

            f.Filtros = filtros;
            propiedades.Add(f);

            return Ok(propiedades);
        }

        /// <summary>
        /// Obtiene los metadatos relacionados con la entidad Almacen Archivo
        /// </summary>
        /// <returns></returns>
        [HttpGet("metadata", Name = "MetadataUnidadAdministrativaArchivo")]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<MetadataInfo>> GetMetadata([FromQuery] Consulta query = null)
        {
            return Ok(await metadataProvider.Obtener().ConfigureAwait(false));
        }


        /// <summary>
        /// Añade una nueva entidad del tipo Almacen Archivo
        /// </summary>
        /// <param name="entidad"></param>
        /// <returns></returns>
        [HttpPost]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<UnidadAdministrativaArchivo>> Post([FromBody] UnidadAdministrativaArchivo entidad)
        {
            var permiso = await ServicioTokenSeguridad.PermisosModuloId(this.UsuarioId, this.DominioId, ConstantesAppGestionDocumental.MODULO_ARCHIVOS)
            .ConfigureAwait(false);

            servicioUnidadAdmin.usuario = this.usuario;
            servicioUnidadAdmin.permisos = permiso;

            entidad = await servicioUnidadAdmin.CrearAsync(entidad).ConfigureAwait(false);
            return Ok(CreatedAtAction("Get", new { id = entidad.Id }, entidad).Value);
        }
        /// <summary>
        /// Actualiza unq entidad Almacen Archivo, el Id debe incluirse en el Querystring así como en 
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
        public async Task<IActionResult> Put(string id, [FromBody] UnidadAdministrativaArchivo entidad)
        {

            var permiso = await ServicioTokenSeguridad.PermisosModuloId(this.UsuarioId, this.DominioId, ConstantesAppGestionDocumental.MODULO_ARCHIVOS)
            .ConfigureAwait(false);

            servicioUnidadAdmin.usuario = this.usuario;
            servicioUnidadAdmin.permisos = permiso;

            if (id.Trim() != entidad.Id.Trim())
            {
                return BadRequest();
            }

            await servicioUnidadAdmin.ActualizarAsync(entidad).ConfigureAwait(false);
            return NoContent();

        }
        /// <summary>
        /// Devulve un alista de Almacen Archivo asociadas al objeto del tipo especificado
        /// </summary>
        /// <param name="query">Consulta para la paginación y búsqueda</param>
        /// <returns></returns>

        [HttpGet("page", Name = "GetPageUnidadAdministrativaArchivo")]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        [ProducesResponseType(StatusCodes.Status200OK)]

        public async Task<ActionResult<Paginado<UnidadAdministrativaArchivo>>> GetPage(
            [ModelBinder(typeof(GenericDataPageModelBinder))][FromQuery] Consulta query = null)
        {

            var permiso = await ServicioTokenSeguridad.PermisosModuloId(this.UsuarioId, this.DominioId, ConstantesAppGestionDocumental.MODULO_ARCHIVOS)
            .ConfigureAwait(false);
            servicioUnidadAdmin.usuario = this.usuario;
            servicioUnidadAdmin.permisos = permiso;
            query.Filtros.AddRange(ObtieneFiltrosIdentidad());

            var data = await servicioUnidadAdmin.ObtenerPaginadoAsync(
                Query: query,
                include: null)
                .ConfigureAwait(false);

            return Ok(data);
        }

        /// <summary>
        /// Obtiene un Almacen Archivo en base al Id único
        /// </summary>
        /// <param name="id">Id único del país</param>
        /// <returns></returns>
        [HttpGet("{id}")]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]

        public async Task<ActionResult<UnidadAdministrativaArchivo>> Get(string id)
        {
            var permiso = await ServicioTokenSeguridad.PermisosModuloId(this.UsuarioId, this.DominioId, ConstantesAppGestionDocumental.MODULO_ARCHIVOS)
                .ConfigureAwait(false);

            servicioUnidadAdmin.usuario = this.usuario;
            servicioUnidadAdmin.permisos = permiso;

            var o = await servicioUnidadAdmin.UnicoAsync(x => x.Id == id.Trim()).ConfigureAwait(false);
            if (o != null) return Ok(o);
            return NotFound(id);
        }

        /// <summary>
        /// Elimina de manera permanente un Almacen Archivo en base al arreglo de identificadores recibidos
        /// </summary>
        /// <param name="ids">Arreglo de identificadores string</param>
        /// <returns></returns>

        [HttpDelete("{ids}")]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult> Delete(string ids)
        {
            var permiso = await ServicioTokenSeguridad.PermisosModuloId(this.UsuarioId, this.DominioId, ConstantesAppGestionDocumental.MODULO_ARCHIVOS)
            .ConfigureAwait(false);

            servicioUnidadAdmin.usuario = this.usuario;
            servicioUnidadAdmin.permisos = permiso;

            string IdsTrim = "";
            foreach (string item in ids.Split(',').ToList().Where(x => !string.IsNullOrEmpty(x)).ToArray())
            {
                IdsTrim += item.Trim() + ",";
            }
            string[] lids = IdsTrim.Split(',').ToList()
           .Where(x => !string.IsNullOrEmpty(x)).ToArray();
            return Ok(await servicioUnidadAdmin.Eliminar(lids).ConfigureAwait(false));
        }


        /// <summary>
        /// Obtiene una lista de archivos en base a los parámetros de consulta
        /// </summary>
        /// <param name="query">Query de busqueda a la base de datos</param>
        /// <returns></returns>

        [HttpGet("pares", Name = "GetParesUAdmin")]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<List<ValorListaOrdenada>>> GetPares(
        [ModelBinder(typeof(GenericDataPageModelBinder))][FromQuery] Consulta query = null)
        {
            var permiso = await ServicioTokenSeguridad.PermisosModuloId(this.UsuarioId, this.DominioId, ConstantesAppGestionDocumental.MODULO_ARCHIVOS)
            .ConfigureAwait(false);
            servicioUnidadAdmin.usuario = this.usuario;
            servicioUnidadAdmin.permisos = permiso;
            query.Filtros.AddRange(ObtieneFiltrosIdentidad());

            var data = await servicioUnidadAdmin.ObtenerParesAsync(query)
                .ConfigureAwait(false);

            return Ok(data);
        }

        /// <summary>
        /// Obtiene una lista de archivs en base a con el parámetro ID de consulta
        /// </summary>
        /// <param name="ids">parametro Id para consulta a la base de datos</param>
        /// <returns></returns>

        [HttpGet("pares/{ids}", Name = "GetParesUAdminporId")]
        [TypeFilter(typeof(AsyncACLActionFilter))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<List<ValorListaOrdenada>>> GetParesporId(
              string ids)
        {
            var permiso = await ServicioTokenSeguridad.PermisosModuloId(this.UsuarioId, this.DominioId, ConstantesAppGestionDocumental.MODULO_ARCHIVOS)
            .ConfigureAwait(false);

            servicioUnidadAdmin.usuario = this.usuario;
            servicioUnidadAdmin.permisos = permiso;

            List<string> lids = ids.Split(',').ToList()
               .Where(x => !string.IsNullOrEmpty(x)).ToList();
            var data = await servicioUnidadAdmin.ObtenerParesPorId(lids)
                .ConfigureAwait(false);

            return Ok(data);
        }

    }
}