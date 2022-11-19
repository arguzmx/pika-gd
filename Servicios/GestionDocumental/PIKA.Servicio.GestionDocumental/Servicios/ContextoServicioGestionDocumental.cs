using Microsoft.Extensions.Logging;
using PIKA.Infraestructura.Comun;
using PIKA.Infraestructura.Comun.Seguridad;
using PIKA.Infraestructura.Comun.Servicios;
using PIKA.Servicio.GestionDocumental.Data;
using RepositorioEntidades;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PIKA.Servicio.GestionDocumental.Servicios
{
    public class ContextoServicioGestionDocumental
    {
        protected ILogger<ServicioLog> logger;
        protected IProveedorOpcionesContexto<DBContextGestionDocumental> proveedorOpciones;
        protected IRegistroAuditoria registroAuditoria;
        protected DBContextGestionDocumental contexto;
        public ContextoServicioGestionDocumental(
            IRegistroAuditoria registroAuditoria,
            IProveedorOpcionesContexto<DBContextGestionDocumental> proveedorOpciones,
            ILogger<ServicioLog> Logger)
        {
            DbContextGestionDocumentalFactory cf = new DbContextGestionDocumentalFactory(proveedorOpciones);
            this.contexto = cf.Crear();
            this.logger = Logger;
            this.proveedorOpciones = proveedorOpciones;
            this.registroAuditoria = registroAuditoria;
        }


        public UsuarioAPI usuario { get; set; }
        public ContextoRegistroActividad RegistroActividad { get; set; }
        public PermisoAplicacion permisos { get; set; }

        public void EstableceContextoSeguridad(UsuarioAPI usuario, ContextoRegistroActividad RegistroActividad)
        {
            this.usuario = usuario;
            this.RegistroActividad = RegistroActividad;
        }

        private async Task<EventoAuditoria> RegistraEvento(int tipoEvento, string AppId, string ModuloId, bool Exitoso = true, string TextoAdicional = null)
        {
            EventoAuditoria ev = new EventoAuditoria()
            {
                DireccionRed = RegistroActividad.DireccionInternet,
                DominioId = RegistroActividad.DominioId,
                EsError = !Exitoso,
                Exitoso = Exitoso,
                Fecha = DateTime.UtcNow,
                FuenteEventoId = AppId,
                IdSesion = RegistroActividad.IdConexion,
                ModuloId = ModuloId,
                TipoEvento = tipoEvento,
                UAId = RegistroActividad.UnidadOrgId,
                UsuarioId = usuario.Id,
                Texto = TextoAdicional
            };

            ev = await registroAuditoria.InsertaEvento(ev);
            return ev;
        }


    }
}

