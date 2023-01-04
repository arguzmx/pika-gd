using System.Collections.Generic;
using LazyCache;
using Microsoft.Extensions.Logging;
using PIKA.Infraestructura.Comun.Seguridad;
using PIKA.Infraestructura.Comun.Servicios;
using RepositorioEntidades;

namespace PIKA.Servicio.Seguridad.Servicios
{
    public class ContextoServicioSeguridad
    {

        protected Seguridad seguridad;
        protected ILogger<ServicioLog> logger;
        protected IProveedorOpcionesContexto<DbContextSeguridad> proveedorOpciones;
        protected IRegistroAuditoria registroAuditoria;
        protected DbContextSeguridad contexto;
        protected UnidadDeTrabajo<DbContextSeguridad> UDT;
        protected IAppCache cache;
        protected string APP_ID;
        protected string MODULO_ID;
        public ContextoServicioSeguridad(
            IRegistroAuditoria registroAuditoria,
            IProveedorOpcionesContexto<DbContextSeguridad> proveedorOpciones,
            ILogger<ServicioLog> Logger,
            IAppCache cache = null, string APP_ID = null, string MODULO_ID = null)
        {
            DbContextSeguridadFactory cf = new DbContextSeguridadFactory(proveedorOpciones);
            this.APP_ID = APP_ID;
            this.MODULO_ID = MODULO_ID;
            this.contexto = cf.Crear();
            this.logger = Logger;
            this.proveedorOpciones = proveedorOpciones;
            this.registroAuditoria = registroAuditoria;
            this.cache = cache;
            UDT = new UnidadDeTrabajo<DbContextSeguridad>(contexto);
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
            seguridad = new Seguridad(APP_ID, MODULO_ID, usuario, RegistroActividad, Eventos, registroAuditoria,
                cache, UDT, DiccionarioEntidadTabla());
        }

        public static Dictionary<string, string> DiccionarioEntidadTabla()
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
