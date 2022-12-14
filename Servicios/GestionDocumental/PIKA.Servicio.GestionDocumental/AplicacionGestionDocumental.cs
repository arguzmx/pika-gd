﻿using PIKA.Constantes.Aplicaciones.GestorDocumental;
using PIKA.Infraestructura.Comun;
using PIKA.Infraestructura.Comun.Seguridad.Auditoria;
using PIKA.Modelo.GestorDocumental;
using PIKA.Servicio.GestionDocumental.Servicios;
using System;
using System.Collections.Generic;

namespace PIKA.Servicio.GestionDocumental
{
    public class AplicacionGestionDocumental : InformacionAplicacionBase, IInformacionAplicacion
    {
        public List<TipoEventoAuditoria> EventosAuditables()
        {
            var l = new List<TipoEventoAuditoria>();

            return l;
        }

        public override Aplicacion Info()
        {
            Aplicacion a = AplicacionRaiz.ObtieneAplicacionRaiz();
            a.Modulos = this.ModulosAplicacion();
            return a;
        }

        public List<ModuloAplicacion> ModulosAplicacion()
        {
            return this.ModulosAplicacionLocales(ConstantesAppGestionDocumental.APP_ID, ConstantesAppGestionDocumental.APP_ID);
        }


        public override List<ElementoAplicacion> GetModulos()
        {
            List<ElementoAplicacion> m = new List<ElementoAplicacion>()
            {
                new ElementoAplicacion(ConstantesAppGestionDocumental.APP_ID, ConstantesAppGestionDocumental.MODULO_CUADROCLASIF ) {
                    Titulo = "Cuadros clasificación",
                    Descripcion = "Permite administrar los cuadros de clasificación",
                    Tipos = new List<Type> { typeof(CuadroClasificacion),
                        typeof(ElementoClasificacion),
                        typeof(EntradaClasificacion),
                        typeof(ValoracionEntradaClasificacion)
                    },
                    EventosAuditables = EventosAuditablesModuloCuadroClasif()
                },
                new ElementoAplicacion(ConstantesAppGestionDocumental.APP_ID, ConstantesAppGestionDocumental.MODULO_ACTIVOS ) {
                    Titulo = "Gestión del acervo",
                    Descripcion = "Permite administrar el acervo para su gestión documental",
                    Tipos = new List<Type> { typeof(Activo), typeof(Ampliacion) },
                    EventosAuditables = EventosAuditablesModuloActivos()
                },
                new ElementoAplicacion(ConstantesAppGestionDocumental.APP_ID, ConstantesAppGestionDocumental.MODULO_CATALOGOSCC ) {
                    Titulo = "Catálogos gestión documental",
                    Descripcion = "Permite administrar los catálogos relacionado con los cuadros de clasifiación",
                    Tipos = new List<Type> { typeof(EstadoCuadroClasificacion),
                            typeof(TipoDisposicionDocumental),
                            typeof(TipoValoracionDocumental),
                            typeof(TipoAmpliacion),
                    }
                },
                new ElementoAplicacion(ConstantesAppGestionDocumental.APP_ID, ConstantesAppGestionDocumental.MODULO_ARCHIVOS ) {
                    Titulo = "Gestión de archivos",
                    Descripcion = "Permite administrar los archivos para la localización del inventario",
                    Tipos = new List<Type> { typeof(Archivo), typeof(AlmacenArchivo), typeof(PermisosArchivo),
                        typeof(ZonaAlmacen), typeof(PosicionAlmacen), typeof(ContenedorAlmacen),
                        typeof(ActivoContenedorAlmacen) },
                      EventosAuditables = EventosAuditablesModuloArchivo()
                },
                new ElementoAplicacion(ConstantesAppGestionDocumental.APP_ID, ConstantesAppGestionDocumental.MODULO_UNIDADESADMIN ) {
                    Titulo = "Gestión de unidades administrativas",
                    Descripcion = "Permite gestionar las unidads administrativas para el archivo",
                    Tipos = new List<Type> { typeof(UnidadAdministrativaArchivo) , typeof(PermisosUnidadAdministrativaArchivo) }
                },
                new ElementoAplicacion(ConstantesAppGestionDocumental.APP_ID, ConstantesAppGestionDocumental.MODULO_CAT_ARCHIVO ) {
                    Titulo = "Catálogos archivo",
                    Descripcion = "Permite administrar los catálogos relacionados con los archivos",
                    Tipos = new List<Type> { typeof(TipoArchivo) }
                },
                new ElementoAplicacion(ConstantesAppGestionDocumental.APP_ID, ConstantesAppGestionDocumental.MODULO_PRESTAMO ) {
                    Titulo = "Gestión de préstamo",
                    Descripcion = "Permite realizar la getsión del préstamo físico de los activos del acervo",
                    Tipos = new List<Type> { typeof(Prestamo), typeof(ActivoPrestamo), typeof(ComentarioPrestamo) }
                },
                //new ElementoAplicacion(ConstantesAppGestionDocumental.APP_ID, ConstantesAppGestionDocumental.MODULO_ALMACENARCHIVO ) {
                //    Titulo = "Gestión de almacenes de archivos",
                //    Descripcion = "Permite realizar la gestión de almacenes físicos para el archivo",
                //    Tipos = new List<Type> { typeof(AlmacenArchivo), typeof(Estante), typeof(EspacioEstante) }
                //},
                new ElementoAplicacion(ConstantesAppGestionDocumental.APP_ID, ConstantesAppGestionDocumental.MODULO_TRANSFERENCIA ) {
                    Titulo = "Gestión de trasnferencias entre archivos",
                    Descripcion = "Permite realizar la gestión de las trasnferencias de inventarios entre archivos",
                    Tipos = new List<Type> { typeof(Transferencia), typeof(EventoTransferencia),
                        typeof(ComentarioTransferencia),typeof(ActivoTransferencia),
                    typeof(HistorialArchivoActivo)},
                     EventosAuditables = EventosAuditablesModuloTransferencias()
                },
                new ElementoAplicacion(ConstantesAppGestionDocumental.APP_ID, ConstantesAppGestionDocumental.MODULO_CAT_TRANSFERENCIA ) {
                    Titulo = "Catálogos transferencias",
                    Descripcion = "Permite realizar la gestión de catálogos relacionados con las transferencia",
                    Tipos = new List<Type> { typeof(EstadoTransferencia) }
                },
                new ElementoAplicacion(ConstantesAppGestionDocumental.APP_ID,ConstantesAppGestionDocumental.MODULO_RESUMEN_ACTIVOS)
                {
                    Titulo="Resumen de activos por cuadro clasificación y Archivo",
                    Descripcion="Permite realizar un resumen de todos los activos que se encuentren almacenados en la estadística",
                    Tipos=new List<Type>{ typeof(EstadisticaClasificacionAcervo)}
                }
              };
            return m;
        }

        public static List<TipoEventoAuditoria> EventosAuditablesModuloCuadroClasif()
        {
            var l = new List<TipoEventoAuditoria>();
            l.AddRange(SeguridadGestionDocumental.EventosAuditoria());
            return l;
        }


        public static List<TipoEventoAuditoria> EventosAuditablesModuloActivos()
        {
            var l = new List<TipoEventoAuditoria>();
            return l;
        }

        public static List<TipoEventoAuditoria> EventosAuditablesModuloArchivo()
        {
            var l = new List<TipoEventoAuditoria>();
            return l;
        }

        public static List<TipoEventoAuditoria> EventosAuditablesModuloTransferencias()
        {
            var l = new List<TipoEventoAuditoria>();
            return l;
        }

    }
}
