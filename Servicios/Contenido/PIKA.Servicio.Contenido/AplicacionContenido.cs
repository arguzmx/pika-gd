using System;
using System.Collections.Generic;
using PIKA.Infraestructura.Comun;
using PIKA.Modelo.Contenido;
using Version = PIKA.Modelo.Contenido.Version;
using PIKA.Constantes.Aplicaciones.Contenido;
using PIKA.Infraestructura.Comun.Seguridad.Auditoria;

namespace PIKA.Servicio.Contenido
{
    public class AplicacionContenido : InformacionAplicacionBase, IInformacionAplicacion
    {

        public override Aplicacion Info()
        {
            Aplicacion a = AplicacionRaiz.ObtieneAplicacionRaiz();
            a.Modulos = this.ModulosAplicacion();
            return a;
        }

        public List<ModuloAplicacion> ModulosAplicacion()
        {
            return this.ModulosAplicacionLocales(ConstantesAppContenido.APP_ID, ConstantesAppContenido.APP_ID);
        }


        public enum EventosAdicionales
        {
            AdicionarPaginas=100, EliminarPaginas=101, ModificarPaginas=102, ExportarPDF=103, ExportarZIP=104, AdicionarMetadatos =15, ModificarMetadatos=106, EliminarMetadatos = 107, 
            CrearVersion = 108, EliminarVersion=109, ActualizarVersion = 110, RotarPaginas=111, ReflejarPaginas= 112, MoverPaginas=113
        }

        private List<TipoEventoAuditoria> EventosAdminConfiguracion()
        {
            List<TipoEventoAuditoria> l = new List<TipoEventoAuditoria>();
            //l.AddRange("EV-GESTOR-CONFIG-LF".EventoComunes<GestorLaserficheConfig>(ConstantesAppContenido.APP_ID, ConstantesAppContenido.MODULO_ADMIN_CONFIGURACION));
            //l.AddRange("EV-GESTOR-CONFIG-AZURE".EventoComunes<GestorAzureConfig>(ConstantesAppContenido.APP_ID, ConstantesAppContenido.MODULO_ADMIN_CONFIGURACION));
            l.AddRange("EV-GESTOR-CONFIG-LOCAL".EventoComunes<GestorLocalConfig>(ConstantesAppContenido.APP_ID, ConstantesAppContenido.MODULO_ADMIN_CONFIGURACION));
            l.AddRange("EV-VOLUMEN-PM".EventoComunes<VolumenPuntoMontaje>(ConstantesAppContenido.APP_ID, ConstantesAppContenido.MODULO_ADMIN_CONFIGURACION));
            l.AddRange("EV-PM".EventoComunes<PuntoMontaje>(ConstantesAppContenido.APP_ID, ConstantesAppContenido.MODULO_ADMIN_CONFIGURACION));
            l.AddRange("EV-PERMISOS-PM".EventoComunes<PermisosPuntoMontaje>(ConstantesAppContenido.APP_ID, ConstantesAppContenido.MODULO_ADMIN_CONFIGURACION));
            // l.AddRange("EV-TIPOGESTOR-ES".EventoComunes<TipoGestorES>(ConstantesAppContenido.APP_ID, ConstantesAppContenido.MODULO_ADMIN_CONFIGURACION));
            l.AddRange("EV-VOLUMEN".EventoComunes<Volumen>(ConstantesAppContenido.APP_ID, ConstantesAppContenido.MODULO_ADMIN_CONFIGURACION));
            return l;
        }

        private List<TipoEventoAuditoria> EventosEstructuraContenido()
        {
            List<TipoEventoAuditoria> l = new List<TipoEventoAuditoria>();
            l.AddRange("EV-CARPETA".EventoComunes<Carpeta>(ConstantesAppContenido.APP_ID, ConstantesAppContenido.MODULO_ESTRUCTURA_CONTENIDO));
            l.AddRange("EV-PARTE".EventoComunes<Parte>(ConstantesAppContenido.APP_ID, ConstantesAppContenido.MODULO_ESTRUCTURA_CONTENIDO));
            l.AddRange("EV-VERSION".EventoComunes<Version>(ConstantesAppContenido.APP_ID, ConstantesAppContenido.MODULO_ESTRUCTURA_CONTENIDO));
            l.AddRange("EV-PERMISO".EventoComunes<Permiso>(ConstantesAppContenido.APP_ID, ConstantesAppContenido.MODULO_ESTRUCTURA_CONTENIDO));
            l.AddRange("EV-DESTINATARIO-PERMISO".EventoComunes<DestinatarioPermiso>(ConstantesAppContenido.APP_ID, ConstantesAppContenido.MODULO_ESTRUCTURA_CONTENIDO));
            l.AddRange("EV-ELEMENTO".EventoComunes<Elemento>(ConstantesAppContenido.APP_ID, ConstantesAppContenido.MODULO_ESTRUCTURA_CONTENIDO));



            Type t = typeof(Elemento);

            l.Add(new TipoEventoAuditoria()
            {
                AppId = ConstantesAppContenido.APP_ID,
                Descripcion = "EV-VERSION-ADD-PAG",
                ModuloId = ConstantesAppContenido.MODULO_ESTRUCTURA_CONTENIDO,
                TipoEntidad = t.Name,
                TipoEvento = EventosAdicionales.AdicionarPaginas.GetHashCode()
            });

            l.Add(new TipoEventoAuditoria()
            {
                AppId = ConstantesAppContenido.APP_ID,
                Descripcion = "EV-VERSION-DEL-PAG",
                ModuloId = ConstantesAppContenido.MODULO_ESTRUCTURA_CONTENIDO,
                TipoEntidad = t.Name,
                TipoEvento = EventosAdicionales.EliminarPaginas.GetHashCode()
            });


            l.Add(new TipoEventoAuditoria()
            {
                AppId = ConstantesAppContenido.APP_ID,
                Descripcion = "EV-VERSION-UPD-PAG",
                ModuloId = ConstantesAppContenido.MODULO_ESTRUCTURA_CONTENIDO,
                TipoEntidad = t.Name,
                TipoEvento = EventosAdicionales.ModificarPaginas.GetHashCode()
            });


            l.Add(new TipoEventoAuditoria()
            {
                AppId = ConstantesAppContenido.APP_ID,
                Descripcion = "EV-VERSION-ROT-PAG",
                ModuloId = ConstantesAppContenido.MODULO_ESTRUCTURA_CONTENIDO,
                TipoEntidad = t.Name,
                TipoEvento = EventosAdicionales.RotarPaginas.GetHashCode()
            });

            l.Add(new TipoEventoAuditoria()
            {
                AppId = ConstantesAppContenido.APP_ID,
                Descripcion = "EV-VERSION-MV-PAG",
                ModuloId = ConstantesAppContenido.MODULO_ESTRUCTURA_CONTENIDO,
                TipoEntidad = t.Name,
                TipoEvento = EventosAdicionales.MoverPaginas.GetHashCode()
            });

            l.Add(new TipoEventoAuditoria()
            {
                AppId = ConstantesAppContenido.APP_ID,
                Descripcion = "EV-VERSION-RX-PAG",
                ModuloId = ConstantesAppContenido.MODULO_ESTRUCTURA_CONTENIDO,
                TipoEntidad = t.Name,
                TipoEvento = EventosAdicionales.ReflejarPaginas.GetHashCode()
            });

            l.Add(new TipoEventoAuditoria()
            {
                AppId = ConstantesAppContenido.APP_ID,
                Descripcion = "EV-VERSION-EXPORT-PDF",
                ModuloId = ConstantesAppContenido.MODULO_ESTRUCTURA_CONTENIDO,
                TipoEntidad = t.Name,
                TipoEvento = EventosAdicionales.ExportarPDF.GetHashCode()
            });


            l.Add(new TipoEventoAuditoria()
            {
                AppId = ConstantesAppContenido.APP_ID,
                Descripcion = "EV-VERSION-EXPORT-ZIP",
                ModuloId = ConstantesAppContenido.MODULO_ESTRUCTURA_CONTENIDO,
                TipoEntidad = t.Name,
                TipoEvento = EventosAdicionales.ExportarZIP.GetHashCode()
            });


            l.Add(new TipoEventoAuditoria()
            {
                AppId = ConstantesAppContenido.APP_ID,
                Descripcion = "EV-VERSION-ADD-METADATA",
                ModuloId = ConstantesAppContenido.MODULO_ESTRUCTURA_CONTENIDO,
                TipoEntidad = t.Name,
                TipoEvento = EventosAdicionales.AdicionarMetadatos.GetHashCode()
            });


            l.Add(new TipoEventoAuditoria()
            {
                AppId = ConstantesAppContenido.APP_ID,
                Descripcion = "EV-VERSION-UPD-METADATA",
                ModuloId = ConstantesAppContenido.MODULO_ESTRUCTURA_CONTENIDO,
                TipoEntidad = t.Name,
                TipoEvento = EventosAdicionales.ModificarMetadatos.GetHashCode()
            });


            l.Add(new TipoEventoAuditoria()
            {
                AppId = ConstantesAppContenido.APP_ID,
                Descripcion = "EV-VERSION-DEL-METADATA",
                ModuloId = ConstantesAppContenido.MODULO_ESTRUCTURA_CONTENIDO,
                TipoEntidad = t.Name,
                TipoEvento = EventosAdicionales.EliminarMetadatos.GetHashCode()
            });


            l.Add(new TipoEventoAuditoria()
            {
                AppId = ConstantesAppContenido.APP_ID,
                Descripcion = "EV-VERSION-ADD-VER",
                ModuloId = ConstantesAppContenido.MODULO_ESTRUCTURA_CONTENIDO,
                TipoEntidad = t.Name,
                TipoEvento = EventosAdicionales.CrearVersion.GetHashCode()
            });

            l.Add(new TipoEventoAuditoria()
            {
                AppId = ConstantesAppContenido.APP_ID,
                Descripcion = "EV-VERSION-DEL-VER",
                ModuloId = ConstantesAppContenido.MODULO_ESTRUCTURA_CONTENIDO,
                TipoEntidad = t.Name,
                TipoEvento = EventosAdicionales.EliminarVersion.GetHashCode()
            });


            l.Add(new TipoEventoAuditoria()
            {
                AppId = ConstantesAppContenido.APP_ID,
                Descripcion = "EV-VERSION-UPD-VER",
                ModuloId = ConstantesAppContenido.MODULO_ESTRUCTURA_CONTENIDO,
                TipoEntidad = t.Name,
                TipoEvento = EventosAdicionales.ActualizarVersion.GetHashCode()
            });
            return l;
        }

        private List<TipoEventoAuditoria> EventosVisor()
        {
            List<TipoEventoAuditoria> l = new List<TipoEventoAuditoria>();
            return l;
        }

        private List<TipoEventoAuditoria> EventosPermisoContenido()
        {
            List<TipoEventoAuditoria> l = new List<TipoEventoAuditoria>();
            return l;
        }

        public override List<ElementoAplicacion> GetModulos()
        {
            List<ElementoAplicacion> m = new List<ElementoAplicacion>()
            {
                new ElementoAplicacion(ConstantesAppContenido.APP_ID, ConstantesAppContenido.MODULO_ADMIN_CONFIGURACION ) {
                    Titulo = "Configuración de almacenamiento de los repositorios",
                    Descripcion = "Permite administrar la configuración de los almacenamientos del repositorio",
                    Tipos = new List<Type> {
                        typeof(GestorLaserficheConfig),
                        typeof(GestorAzureConfig),
                        typeof(GestorLocalConfig),
                        typeof(VolumenPuntoMontaje),
                        typeof(PermisosPuntoMontaje),
                        typeof(PuntoMontaje),
                        typeof(TipoGestorES),
                        typeof(Volumen)
                    },
                    EventosAuditables = EventosAdminConfiguracion()
                },
                 new ElementoAplicacion(ConstantesAppContenido.APP_ID, ConstantesAppContenido.MODULO_ESTRUCTURA_CONTENIDO ) {
                    Titulo = "Gestión de elementos de contenido",
                    Descripcion = "Permite realizar la gestión de contenido del repositorio, por ejemplo carpetas y documentos",
                    Tipos = new List<Type> {
                        typeof(Carpeta),
                        typeof(Parte),
                        typeof(Version),
                        typeof(Permiso),
                        typeof(DestinatarioPermiso),
                        typeof(Elemento),
                        typeof(PuntoMontaje)
                    },
                     EventosAuditables = EventosEstructuraContenido()

                },

                new ElementoAplicacion(ConstantesAppContenido.APP_ID, ConstantesAppContenido.MODULO_VISOR_CONTENIDO ) {
                    Titulo = "Acceso al visor de contenido",
                    Descripcion = "Permite acceder al visor de cotenids para su visualización y edición",
                    Tipos = new List<Type> {},
                     EventosAuditables = EventosVisor()
                },
                     new ElementoAplicacion(ConstantesAppContenido.APP_ID, ConstantesAppContenido.MODULO_PERMISOS_CONTENIDO ) {
                    Titulo = "Acceso al administrador de permisos de contenido",
                    Descripcion = "Permite acceder administardor de permisos de acceso de contenido que define la seguridad del mismo",
                    Tipos = new List<Type> {},
                     EventosAuditables = EventosPermisoContenido()
                }

            };

            return m;
        }


    }

}
