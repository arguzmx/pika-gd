using PIKA.Constantes.Aplicaciones.Organizacion;
using PIKA.Infraestructura.Comun;
using PIKA.Infraestructura.Comun.Seguridad.Auditoria;
using PIKA.Modelo.Organizacion;
using System;
using System.Collections.Generic;

namespace PIKA.Servicio.Organizacion
{
    public class AplicacionOrganizacion : InformacionAplicacionBase, IInformacionAplicacion
    {
    

        public override Aplicacion Info()
        {
            Aplicacion a = AplicacionRaiz.ObtieneAplicacionRaiz();
            a.Modulos = this.ModulosAplicacion();
            return a;
        }

        public List<ModuloAplicacion> ModulosAplicacion()
        {
            return this.ModulosAplicacionLocales(ConstantesAppOrganizacion.APP_ID, ConstantesAppOrganizacion.APP_ID);
        }
        public enum EventosAdicionales
        {
            ActualizaDatosOrganizacion = 100, VincularUsuariosRol=101, DesVincularUsuariosRol = 102
        }


        
        private List<TipoEventoAuditoria> EventosDominio()
        {
            List<TipoEventoAuditoria> l = new List<TipoEventoAuditoria>();
            // l.AddRange("EV-DOMINIO".EventoComunes<Dominio>(ConstantesAppOrganizacion.APP_ID, ConstantesAppOrganizacion.MODULO_DOMINIO));
            Type t = typeof(Dominio);

            l.Add(new TipoEventoAuditoria()
            {
                AppId = ConstantesAppOrganizacion.APP_ID,
                Descripcion = "EV-DOMINIO-UPDATE-ORG",
                ModuloId = ConstantesAppOrganizacion.MODULO_DOMINIO,
                TipoEntidad = t.Name,
                TipoEvento = EventosAdicionales.ActualizaDatosOrganizacion.GetHashCode()
            });

            return l;
        }

        private List<TipoEventoAuditoria> EventosUO()
        {
            List<TipoEventoAuditoria> l = new List<TipoEventoAuditoria>();
            // l.AddRange("EV-UNIDAD-ORG".EventoComunes<Dominio>(ConstantesAppOrganizacion.APP_ID, ConstantesAppOrganizacion.MODULO_DOMINIO));
            return l;
        }

        private List<TipoEventoAuditoria> EventosRol()
        {
            List<TipoEventoAuditoria> l = new List<TipoEventoAuditoria>();
            l.AddRange("EV-ROL".EventoComunes<Rol>(ConstantesAppOrganizacion.APP_ID, ConstantesAppOrganizacion.MODULO_ROL));
            l.AddRange("EV-USAURIOS-ROL".EventoComunes<UsuariosRol>(ConstantesAppOrganizacion.APP_ID, ConstantesAppOrganizacion.MODULO_ROL));

            Type t = typeof(Rol);

            l.Add(new TipoEventoAuditoria()
            {
                AppId = ConstantesAppOrganizacion.APP_ID,
                Descripcion = "EV-LINK-USR-ROL",
                ModuloId = ConstantesAppOrganizacion.MODULO_ROL,
                TipoEntidad = t.Name,
                TipoEvento = EventosAdicionales.VincularUsuariosRol.GetHashCode()
            });

            l.Add(new TipoEventoAuditoria()
            {
                AppId = ConstantesAppOrganizacion.APP_ID,
                Descripcion = "EV-UNLINK-USR-ROL",
                ModuloId = ConstantesAppOrganizacion.MODULO_ROL,
                TipoEntidad = t.Name,
                TipoEvento = EventosAdicionales.DesVincularUsuariosRol.GetHashCode()
            });

            return l;
        }

        public override List<ElementoAplicacion> GetModulos()
        {
            List<ElementoAplicacion> m = new List<ElementoAplicacion>()
            {
                new ElementoAplicacion(ConstantesAppOrganizacion.APP_ID, ConstantesAppOrganizacion.MODULO_DOMINIO ) {
                    Titulo = "Dominios",
                    Descripcion = "Permite administrar los dominios para la conformación de organizaciones",
                    Tipos = new List<Type> { typeof(Dominio) },
                    EventosAuditables = EventosDominio()
                },
                new ElementoAplicacion(ConstantesAppOrganizacion.APP_ID, ConstantesAppOrganizacion.MODULO_UNIDADORGANIZACIONAL ) {
                    Titulo = "Unidades organizacionales",
                    Descripcion = "Permite administrar las unidades organizacionales de un dominio",
                    Tipos = new List<Type> { typeof(UnidadOrganizacional) },
                    EventosAuditables = EventosUO()
                },
                new ElementoAplicacion(ConstantesAppOrganizacion.APP_ID, ConstantesAppOrganizacion.MODULO_ROL ) {
                    Titulo = "Roles",
                    Descripcion = "Permite administrar los roles de usuario de un dominio",
                    Tipos = new List<Type> { typeof(Rol), typeof(UsuariosRol) },
                    EventosAuditables = EventosRol()
                }
            };
            return m;
        }

    }
}
