using LazyCache;
using Microsoft.EntityFrameworkCore;
using PIKA.Infraestructura.Comun.Seguridad;
using PIKA.Infraestructura.Comun.Seguridad.Auditoria;
using RepositorioEntidades;
using System.Collections.Generic;
using System.Threading.Tasks;

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


        public async Task<bool> UsuarioEnDominio(string UsuarioId, string DominioId)
        {
            var u = await UDT.Context.Usuarios.FirstOrDefaultAsync(x => x.Id == UsuarioId);
            if (u != null)
            {
                if(!u.Inactiva &&  !u.Eliminada)
                {
                    var ds = await UDT.Context.UsuariosDominio.FirstOrDefaultAsync(x => x.ApplicationUserId == UsuarioId && x.DominioId == DominioId);
                    if(ds != null)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

    }
}
