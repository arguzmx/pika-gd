using LazyCache;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PIKA.Constantes.Aplicaciones.Contenido;
using PIKA.Infraestructura.Comun;
using PIKA.Infraestructura.Comun.Interfaces;
using PIKA.Infraestructura.Comun.Seguridad;
using PIKA.Infraestructura.Comun.Servicios;
using PIKA.Modelo.Contenido;
using PIKA.Modelo.Contenido.Extensiones;
using PIKA.Modelo.Contenido.ui;
using PIKA.Servicio.Contenido.Helpers;
using PIKA.Servicio.Contenido.Interfaces;
using RepositorioEntidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace PIKA.Servicio.Contenido.Servicios
{
    public class ServicioVisor: ContextoServicioContenido,
        IServicioInyectable, IServicioVisor
    {

        public ServicioVisor(
            IRegistroAuditoria registroAuditoria,
            IAppCache cache,
            IProveedorOpcionesContexto<DbContextContenido> proveedorOpciones,
            ILogger<ServicioLog> Logger
        ) : base(registroAuditoria, proveedorOpciones, Logger,
            cache, ConstantesAppContenido.APP_ID, ConstantesAppContenido.MODULO_ESTRUCTURA_CONTENIDO)
        {

        }

        public async Task<int> ACLPuntoMontaje(string PuntoMontajeId)
        {
            if (usuario == null) return 0;
            int mascara = int.MaxValue;

            var pm = UDT.Context.PuntosMontaje.Where(x => x.Id == PuntoMontajeId).SingleOrDefault();
            if (pm != null)
            {
                foreach (var a in usuario.Accesos)
                {
                    if (a.Admin && pm.OrigenId == a.OU)
                    {
                        return mascara - MascaraPermisos.PDenegarAcceso;
                    }
                }
            }

            foreach (string r in usuario.Roles)
            {
                var permiso = await UDT.Context.PermisosPuntoMontaje.Where(x => x.DestinatarioId == r && x.PuntoMontajeId == PuntoMontajeId).SingleOrDefaultAsync();
                if (permiso != null)
                {
                    MascaraPermisos m = new MascaraPermisos()
                    {
                        Admin = false,
                        Ejecutar = false,
                        Eliminar = permiso.Elminar,
                        Escribir = permiso.Actualizar,
                        Leer = permiso.Leer
                    };

                    // asigna los permisos mínimos de acuerdo a los roles
                    mascara &= m.ObtenerMascara();
                }
            }

            return mascara;
        }


        public async Task<Documento> ObtieneDocumento(string IdElemento)
        {
            ComunesElemento elementos = new ComunesElemento(UDT);
            Documento d = null;
            Elemento e = await elementos.ObtieneElemento(IdElemento);

            if (e != null)
            {
                var permisos = await this.ACLPuntoMontaje(e.PuntoMontajeId);
                var m = new MascaraPermisos();
                m.EstablacerMascara(permisos);
                if (!m.Leer)
                {
                    await seguridad.EmiteDatosSesionIncorrectos();
                }

                if (!await seguridad.AccesoCachePuntoMontaje(e.PuntoMontajeId))
                {
                    await seguridad.EmiteDatosSesionIncorrectos();
                }

                var pm = await UDT.Context.PuntosMontaje.FirstOrDefaultAsync(x => x.Id == e.PuntoMontajeId);
                if (!string.IsNullOrEmpty(pm.VolumenDefaultId))
                {
                    await seguridad.AccesoCacheVolumen(pm.VolumenDefaultId);
                }

                d = new Documento() { Id = e.Id, Nombre = e.Nombre, VersionId = e.VersionId, Paginas = new List<Pagina>() };
            }
            return d;
        }

        public Task<Elemento> ObtienePerrmisos(string EntidadId, string DominioId, string UnidaddOrganizacionalId)
        {
            throw new NotImplementedException();
        }
    }
}
