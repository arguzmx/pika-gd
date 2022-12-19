using LazyCache;
using PIKA.Infraestructura.Comun.Seguridad;
using PIKA.Infraestructura.Comun.Seguridad.Auditoria;
using RepositorioEntidades;
using System.Collections.Generic;

namespace PIKA.Servicio.Seguridad.Servicios
{
    public class Seguridad: SevicioAuditableBase
    {
        private UnidadDeTrabajo<DbContextSeguridad> UDT;

        public Seguridad(
            string APP_ID, string MODULO_ID,
            UsuarioAPI usuario,
            ContextoRegistroActividad RegistroActividad,
            List<EventoAuditoriaActivo> EventosActivos,
            IRegistroAuditoria registroAuditoria,
            IAppCache cache,
            UnidadDeTrabajo<DbContextSeguridad> UDT,
            Dictionary<string, string> Tablas) :
            base(
                APP_ID, MODULO_ID,
                usuario, RegistroActividad, EventosActivos, registroAuditoria,
                cache, Tablas, UDT.Context)
        {
            this.UDT = UDT;
        }

    }
}
