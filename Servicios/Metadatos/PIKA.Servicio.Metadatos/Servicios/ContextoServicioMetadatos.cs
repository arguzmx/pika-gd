using System.Collections.Generic;
using LazyCache;
using Microsoft.Extensions.Logging;
using PIKA.Infraestructura.Comun.Seguridad;
using PIKA.Infraestructura.Comun.Servicios;
using PIKA.Servicio.Metadatos.Data;
using RepositorioEntidades;

namespace PIKA.Servicio.Metadatos.Servicios
{
    public class ContextoServicioMetadatos
    {

        protected SeguridadMetadatos seguridad;
        protected ILogger<ServicioLog> logger;
        protected IProveedorOpcionesContexto<DbContextMetadatos> proveedorOpciones;
        protected IRegistroAuditoria registroAuditoria;
        protected DbContextMetadatos contexto;
        protected UnidadDeTrabajo<DbContextMetadatos> UDT;
        protected IAppCache cache;
        protected string APP_ID;
        protected string MODULO_ID;

        public ContextoServicioMetadatos(
            IRegistroAuditoria registroAuditoria,
            IProveedorOpcionesContexto<DbContextMetadatos> proveedorOpciones,
            ILogger<ServicioLog> Logger,
            IAppCache cache = null, string APP_ID = null, string MODULO_ID = null)
        {
            DbContextMetadatosFactory cf = new DbContextMetadatosFactory(proveedorOpciones);
            this.APP_ID = APP_ID;
            this.MODULO_ID = MODULO_ID;
            this.contexto = cf.Crear();
            this.logger = Logger;
            this.proveedorOpciones = proveedorOpciones;
            this.registroAuditoria = registroAuditoria;
            this.cache = cache;
            UDT = new UnidadDeTrabajo<DbContextMetadatos>(contexto);
        }

        public UsuarioAPI usuario { get; set; }
        public ContextoRegistroActividad RegistroActividad { get; set; }
        public PermisoAplicacion permisos { get; set; }
        public List<EventoAuditoriaActivo> EventosAuditoriaActivos { get; set; }

        public async void EstableceContextoSeguridad(UsuarioAPI usuario, ContextoRegistroActividad RegistroActividad, List<EventoAuditoriaActivo> Eventos)
        {
            this.usuario = usuario;
            this.RegistroActividad = RegistroActividad;
            this.EventosAuditoriaActivos = Eventos;
            seguridad = new SeguridadMetadatos(APP_ID, MODULO_ID, usuario, RegistroActividad, Eventos, registroAuditoria,
                cache, UDT, DiccionarioEntidad());
        }

        public static Dictionary<string, string> DiccionarioEntidad()
        {
            return new Dictionary<string, string>
            {
                //{ typeof(CuadroClasificacion).Name, DBContextGestionDocumental.TablaCuadrosClasificacion },
                //{ typeof(ElementoClasificacion).Name, DBContextGestionDocumental.TablaElementosClasificacion },
                //{ typeof(EntradaClasificacion).Name, DBContextGestionDocumental.TablaEntradaClasificacion }
            };
        }

    }
}
