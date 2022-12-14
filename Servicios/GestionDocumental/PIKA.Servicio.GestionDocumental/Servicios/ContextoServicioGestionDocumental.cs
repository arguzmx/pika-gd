using LazyCache;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PIKA.Infraestructura.Comun.Seguridad;
using PIKA.Infraestructura.Comun.Servicios;
using PIKA.Modelo.GestorDocumental;
using PIKA.Servicio.GestionDocumental.Data;
using RepositorioEntidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PIKA.Servicio.GestionDocumental.Servicios
{
    public class ContextoServicioGestionDocumental
    {
        protected SeguridadGestionDocumental seguridad;
        protected ILogger<ServicioLog> logger;
        protected IProveedorOpcionesContexto<DBContextGestionDocumental> proveedorOpciones;
        protected IRegistroAuditoria registroAuditoria;
        protected DBContextGestionDocumental contexto;
        protected UnidadDeTrabajo<DBContextGestionDocumental> UDT;
        protected IAppCache cache;
        protected string APP_ID;
        protected string MODULO_ID;
        public ContextoServicioGestionDocumental(
            IRegistroAuditoria registroAuditoria,
            IProveedorOpcionesContexto<DBContextGestionDocumental> proveedorOpciones,
            ILogger<ServicioLog> Logger,
            IAppCache cache  =null, string APP_ID =null, string MODULO_ID = null)
        {
            DbContextGestionDocumentalFactory cf = new DbContextGestionDocumentalFactory(proveedorOpciones);
            this.APP_ID = APP_ID;
            this.MODULO_ID = MODULO_ID;
            this.contexto = cf.Crear();
            this.logger = Logger;
            this.proveedorOpciones = proveedorOpciones;
            this.registroAuditoria = registroAuditoria;
            this.cache = cache;
            UDT = new UnidadDeTrabajo<DBContextGestionDocumental>(contexto);
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
            seguridad = new SeguridadGestionDocumental(APP_ID, MODULO_ID, usuario, RegistroActividad, Eventos, registroAuditoria,
                cache, UDT, DiccionarioEntidadTabla());
        }

        public static Dictionary<string, string> DiccionarioEntidadTabla()
        {
            return new Dictionary<string, string>
            {
                { typeof(CuadroClasificacion).Name, DBContextGestionDocumental.TablaCuadrosClasificacion },
                { typeof(ElementoClasificacion).Name, DBContextGestionDocumental.TablaElementosClasificacion },
                { typeof(EntradaClasificacion).Name, DBContextGestionDocumental.TablaEntradaClasificacion }
            };
        }

        public TipoArchivo TipoArchivoDeArchivo(string Id)
        {

            string sqls = $"select t.*  from gd$tipoarchivo t inner join gd$archivo a on t.Id = a.TipoArchivoId where a.Id = '{Id}'";
            TipoArchivo t = UDT.Context.TiposArchivo.FromSqlRaw(sqls).FirstOrDefault();
            return t;
        }


    }
}

