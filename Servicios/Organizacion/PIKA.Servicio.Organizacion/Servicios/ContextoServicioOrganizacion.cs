using LazyCache;
using Microsoft.Extensions.Logging;
using PIKA.Infraestructura.Comun;
using PIKA.Infraestructura.Comun.Seguridad;
using PIKA.Infraestructura.Comun.Servicios;
using RepositorioEntidades;
using System;
using System.Collections.Generic;
using System.Text;

namespace PIKA.Servicio.Organizacion
{

    public class ContextoServicioOrganizacion
    {
        protected SeguridadOrganizacion seguridad;
        protected ILogger<ServicioLog> logger;
        protected IProveedorOpcionesContexto<DbContextOrganizacion> proveedorOpciones;
        protected IRegistroAuditoria registroAuditoria;
        protected DbContextOrganizacion contexto;
        protected UnidadDeTrabajo<DbContextOrganizacion> UDT;
        protected IAppCache cache;
        protected string APP_ID;
        protected string MODULO_ID;
        public ContextoServicioOrganizacion(
            IRegistroAuditoria registroAuditoria,
            IProveedorOpcionesContexto<DbContextOrganizacion> proveedorOpciones,
            ILogger<ServicioLog> Logger,
            IAppCache cache = null, string APP_ID = null, string MODULO_ID = null)
        {
            DbContextOrganizacionFactory cf = new DbContextOrganizacionFactory(proveedorOpciones);
            this.APP_ID = APP_ID;
            this.MODULO_ID = MODULO_ID;
            this.contexto = cf.Crear();
            this.logger = Logger;
            this.proveedorOpciones = proveedorOpciones;
            this.registroAuditoria = registroAuditoria;
            this.cache = cache;
            UDT = new UnidadDeTrabajo<DbContextOrganizacion>(contexto);
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
            seguridad = new SeguridadOrganizacion(APP_ID, MODULO_ID, usuario, RegistroActividad, Eventos, registroAuditoria,
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
