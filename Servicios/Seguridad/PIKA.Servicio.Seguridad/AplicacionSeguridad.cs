using System;
using System.Collections.Generic;
using PIKA.Constantes.Aplicaciones.Seguridad;
using PIKA.Infraestructura.Comun;
using PIKA.Infraestructura.Comun.Seguridad;
using PIKA.Infraestructura.Comun.Seguridad.Auditoria;
using PIKA.Modelo.Seguridad;

namespace PIKA.Servicio.Seguridad
{
    public class AplicacionSeguridad : InformacionAplicacionBase, IInformacionAplicacion
    {
        public override Aplicacion Info()
        {
            Aplicacion a = AplicacionRaiz.ObtieneAplicacionRaiz();
            a.Modulos = this.ModulosAplicacion();
            return a;
        }

        public List<ModuloAplicacion> ModulosAplicacion()
        {
            return this.ModulosAplicacionLocales(ConstantesAppSeguridad.APP_ID, ConstantesAppSeguridad.APP_ID);
        }

        public enum EventosAdicionales
        {
            CambioContrasenaAdmin=100, CambioContrasenaUsuario=101 , CambioACL=102, CambioConfigAuditoria=103, LecturaEventosAuditoria=104, EliminacionLogAuditoria=105, AccesoAlSistema=106
        }

        private List<TipoEventoAuditoria> EventosUsuarios()
        {
            List<TipoEventoAuditoria> l = new List<TipoEventoAuditoria>();
            l.AddRange("EV-USR-SIS".EventoComunes<PropiedadesUsuario>(ConstantesAppSeguridad.APP_ID, ConstantesAppSeguridad.MODULO_USUARIOS));

            Type t = typeof(ApplicationUser);

            l.Add(new TipoEventoAuditoria()
            {
                AppId = ConstantesAppSeguridad.APP_ID,
                Descripcion = "EV-USR-PASS-ADM",
                ModuloId = ConstantesAppSeguridad.MODULO_USUARIOS,
                TipoEntidad = t.Name,
                TipoEvento = EventosAdicionales.CambioContrasenaAdmin.GetHashCode()
            });


            l.Add(new TipoEventoAuditoria()
            {
                AppId = ConstantesAppSeguridad.APP_ID,
                Descripcion = "EV-USR-PASS-USR",
                ModuloId = ConstantesAppSeguridad.MODULO_USUARIOS,
                TipoEntidad = t.Name,
                TipoEvento = EventosAdicionales.CambioContrasenaUsuario.GetHashCode()
            });

            l.Add(new TipoEventoAuditoria()
            {
                AppId = ConstantesAppSeguridad.APP_ID,
                Descripcion = "EV-USR-LOGIN",
                ModuloId = ConstantesAppSeguridad.MODULO_USUARIOS,
                TipoEntidad = t.Name,
                TipoEvento = EventosAdicionales.AccesoAlSistema.GetHashCode()
            });

            return l;
        }

        private List<TipoEventoAuditoria> EventosAcceso()
        {
            List<TipoEventoAuditoria> l = new List<TipoEventoAuditoria>();

            Type t = typeof(PermisoAplicacion);
            l.Add(new TipoEventoAuditoria()
            {
                AppId = ConstantesAppSeguridad.APP_ID,
                Descripcion = "EV-ACL-UPDATE",
                ModuloId = ConstantesAppSeguridad.MODULO_ACL,
                TipoEntidad = t.Name,
                TipoEvento = EventosAdicionales.CambioACL.GetHashCode()
            });

            return l;
        }


        private List<TipoEventoAuditoria> EventosConfigAuditoria()
        {
            List<TipoEventoAuditoria> l = new List<TipoEventoAuditoria>();

            Type t = typeof(EventoAuditoriaActivo);
            l.Add(new TipoEventoAuditoria()
            {
                AppId = ConstantesAppSeguridad.APP_ID,
                Descripcion = "EV-LOG-CONFIG",
                ModuloId = ConstantesAppSeguridad.MODULO_CONFIG_AUDITORIA,
                TipoEntidad = t.Name,
                TipoEvento = EventosAdicionales.CambioConfigAuditoria.GetHashCode()
            });

            return l;
        }

        private List<TipoEventoAuditoria> EventosAuditoria()
        {
            List<TipoEventoAuditoria> l = new List<TipoEventoAuditoria>();

            Type t = typeof(EventoAuditoria);
            l.Add(new TipoEventoAuditoria()
            {
                AppId = ConstantesAppSeguridad.APP_ID,
                Descripcion = "EV-LOG-READ",
                ModuloId = ConstantesAppSeguridad.MODULO_AUDITORIA,
                TipoEntidad = t.Name,
                TipoEvento = EventosAdicionales.LecturaEventosAuditoria.GetHashCode()
            });



            l.Add(new TipoEventoAuditoria()
            {
                AppId = ConstantesAppSeguridad.APP_ID,
                Descripcion = "EV-LOG-DELETE",
                ModuloId = ConstantesAppSeguridad.MODULO_AUDITORIA,
                TipoEntidad = t.Name,
                TipoEvento = EventosAdicionales.EliminacionLogAuditoria.GetHashCode()
            });

            return l;
        }


        public override List<ElementoAplicacion> GetModulos()
        {
            List<ElementoAplicacion> m = new List<ElementoAplicacion>()
            {
                new ElementoAplicacion(ConstantesAppSeguridad.APP_ID, ConstantesAppSeguridad.MODULO_USUARIOS ) {
                    Titulo = "Usuarios del sistema",
                    Descripcion = "Permite gestionar los usuarios del sistema",
                    Tipos = new List<Type> { typeof(PropiedadesUsuario), typeof(Genero) },
                    EventosAuditables = EventosUsuarios()
                },
                new ElementoAplicacion(ConstantesAppSeguridad.APP_ID, ConstantesAppSeguridad.MODULO_ACL ) {
                    Titulo = "Control de acceso",
                    Descripcion = "Permite gestionar el control de acceso a las aplicaciones del sistema",
                    Tipos = new List<Type> { typeof(Aplicacion), typeof(PermisoAplicacion)},
                     EventosAuditables = EventosAcceso()
                },
                new ElementoAplicacion(ConstantesAppSeguridad.APP_ID, ConstantesAppSeguridad.MODULO_AUDITORIA) {
                    Titulo = "Bitácoras del sistema",
                    Descripcion = "Permite la consulta de la bitácora de auditoria",
                    Tipos = new List<Type> { typeof(EventoAuditoria) },
                    EventosAuditables = EventosAuditoria()
                },
                new ElementoAplicacion(ConstantesAppSeguridad.APP_ID, ConstantesAppSeguridad.MODULO_CONFIG_AUDITORIA) {
                    Titulo = "Configuración de auditoría",
                    Descripcion = "Permite definir configuración de los eventos de auditoría a registrar",
                    Tipos = new List<Type> { typeof(EventoAuditoriaActivo) },
                     EventosAuditables =EventosConfigAuditoria()
                },
                //                new ElementoAplicacion(ConstantesAppSeguridad.APP_ID, ConstantesAppSeguridad.MODULO_APLICACIONES ) {
                //    Titulo = "Gestor de aplicaciones",
                //    Descripcion = "Permite gestionar las aplicaciones del sistema",
                //    Tipos = new List<Type> { typeof(Aplicacion),
                //        typeof(ModuloAplicacion),
                //        typeof(TipoAdministradorModulo),
                //        typeof(TraduccionAplicacionModulo)
                //    }
                //},
                //                new ElementoAplicacion(ConstantesAppSeguridad.APP_ID, ConstantesAppSeguridad.MODULO_CAT_USUARIOS ) {
                //    Titulo = "Catálogos usuario",
                //    Descripcion = "Permite administrar los catálogos relacionados con los usuario del sistema",
                //    Tipos = new List<Type> { typeof(Genero) }
                //},
            };
            return m;
        }

    }

}
