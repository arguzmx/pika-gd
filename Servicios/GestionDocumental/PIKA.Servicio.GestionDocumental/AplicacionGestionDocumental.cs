using DocumentFormat.OpenXml.Drawing.Charts;
using PIKA.Constantes.Aplicaciones.GestorDocumental;
using PIKA.Infraestructura.Comun;
using PIKA.Infraestructura.Comun.Seguridad.Auditoria;
using PIKA.Modelo.GestorDocumental;
using PIKA.Servicio.GestionDocumental.Servicios;
using System;
using System.Collections.Generic;
using System.Text.Json;

namespace PIKA.Servicio.GestionDocumental
{
    public class AplicacionGestionDocumental : InformacionAplicacionBase, IInformacionAplicacion
    {

        public override Aplicacion Info()
        {
            Aplicacion a = AplicacionRaiz.ObtieneAplicacionRaiz();
            a.Modulos = this.ModulosAplicacion();

            foreach (var m in a.Modulos)
            {
                if (m.EventosAuditables != null)
                {
                    Console.WriteLine($"{m.Id} : {m.EventosAuditables.Count}");
                }
            }


            return a;
        }

        public List<ModuloAplicacion> ModulosAplicacion()
        {
            return this.ModulosAplicacionLocales(ConstantesAppGestionDocumental.APP_ID, ConstantesAppGestionDocumental.APP_ID);
        }

        public enum EventosAdicionales
        {
            EntregarPrestamo = 100, DevolverPrestamo = 101, EnviarTransferencia = 102, RecibirTransferencia = 103,
            RecibirTrasnferenciaParcial = 104, DeclinarTrasnferencia = 105
        }

        private List<TipoEventoAuditoria> EventosModuloCuadroClasificacion()
        {
            List<TipoEventoAuditoria> l = new List<TipoEventoAuditoria>();
            l.AddRange("EV-CUADRO-CLASIFICACION".EventoComunes<CuadroClasificacion>(ConstantesAppGestionDocumental.APP_ID, ConstantesAppGestionDocumental.MODULO_CUADROCLASIF));
            l.AddRange("EV-FOLDER-SERIE-DOCUMENTAL".EventoComunes<ElementoClasificacion>(ConstantesAppGestionDocumental.APP_ID, ConstantesAppGestionDocumental.MODULO_CUADROCLASIF));
            l.AddRange("EV-SERIE-DOCUMENTAL".EventoComunes<EntradaClasificacion>(ConstantesAppGestionDocumental.APP_ID, ConstantesAppGestionDocumental.MODULO_CUADROCLASIF));
            l.AddRange("EV-VALORACION-ENTRADA".EventoComunes<ValoracionEntradaClasificacion>(ConstantesAppGestionDocumental.APP_ID, ConstantesAppGestionDocumental.MODULO_CUADROCLASIF));

            return l;
        }

        private List<TipoEventoAuditoria> EventosModuloInventario()
        {
            List<TipoEventoAuditoria> l = new List<TipoEventoAuditoria>();
            l.AddRange("EV-ACTIVO".EventoComunes<Activo>(ConstantesAppGestionDocumental.APP_ID, ConstantesAppGestionDocumental.MODULO_ACTIVOS));
            l.AddRange("EV-AMPLIACION".EventoComunes<Ampliacion>(ConstantesAppGestionDocumental.APP_ID, ConstantesAppGestionDocumental.MODULO_ACTIVOS));
            return l;
        }

        private List<TipoEventoAuditoria> EventosModuloCatalogo()
        {
            List<TipoEventoAuditoria> l = new List<TipoEventoAuditoria>();
            // l.AddRange("EV-CAT-ESTADO-CUADRO-CLASIFICACION".EventoComunes<EstadoCuadroClasificacion>(ConstantesAppGestionDocumental.APP_ID, ConstantesAppGestionDocumental.MODULO_CATALOGOSCC));
            l.AddRange("EV-CAT-DISPOSICION".EventoComunes<TipoDisposicionDocumental>(ConstantesAppGestionDocumental.APP_ID, ConstantesAppGestionDocumental.MODULO_CATALOGOSCC));
            l.AddRange("EV-CAT-VALORACION".EventoComunes<TipoValoracionDocumental>(ConstantesAppGestionDocumental.APP_ID, ConstantesAppGestionDocumental.MODULO_CATALOGOSCC));
            l.AddRange("EV-CAT-AMPLIACION".EventoComunes<TipoAmpliacion>(ConstantesAppGestionDocumental.APP_ID, ConstantesAppGestionDocumental.MODULO_CATALOGOSCC));
            l.AddRange("EV-CAT-TIPO-ARCHIVO".EventoComunes<TipoArchivo>(ConstantesAppGestionDocumental.APP_ID, ConstantesAppGestionDocumental.MODULO_CATALOGOSCC));
            return l;
        }


        private List<TipoEventoAuditoria> EventosModuloUnidadesAdministrativas()
        {
            List<TipoEventoAuditoria> l = new List<TipoEventoAuditoria>();
            l.AddRange("EV-UNIDADES-ADMIN".EventoComunes<UnidadAdministrativaArchivo>(ConstantesAppGestionDocumental.APP_ID, ConstantesAppGestionDocumental.MODULO_UNIDADESADMIN));
            l.AddRange("EV-PERMISOS-UNIDADES-ADMIN".EventoComunes<PermisosUnidadAdministrativaArchivo>(ConstantesAppGestionDocumental.APP_ID, ConstantesAppGestionDocumental.MODULO_UNIDADESADMIN));
            return l;
        }

        private List<TipoEventoAuditoria> EventosModuloArchivos()
        {
            List<TipoEventoAuditoria> l = new List<TipoEventoAuditoria>();
            l.AddRange("EV-ARCHIVO".EventoComunes<Archivo>(ConstantesAppGestionDocumental.APP_ID, ConstantesAppGestionDocumental.MODULO_ARCHIVOS));
            l.AddRange("EV-ALMACEN-ARCHIVO".EventoComunes<AlmacenArchivo>(ConstantesAppGestionDocumental.APP_ID, ConstantesAppGestionDocumental.MODULO_ARCHIVOS));
            l.AddRange("EV-PERMISOS-ARCHIVO".EventoComunes<PermisosArchivo>(ConstantesAppGestionDocumental.APP_ID, ConstantesAppGestionDocumental.MODULO_ARCHIVOS));
            l.AddRange("EV-ZONA-ARCHIVO".EventoComunes<ZonaAlmacen>(ConstantesAppGestionDocumental.APP_ID, ConstantesAppGestionDocumental.MODULO_ARCHIVOS));
            l.AddRange("EV-POSICION-ALMACEN".EventoComunes<PosicionAlmacen>(ConstantesAppGestionDocumental.APP_ID, ConstantesAppGestionDocumental.MODULO_ARCHIVOS));
            l.AddRange("EV-CONTENEDOR-ALMACEN".EventoComunes<ContenedorAlmacen>(ConstantesAppGestionDocumental.APP_ID, ConstantesAppGestionDocumental.MODULO_ARCHIVOS));
            l.AddRange("EV-ACTIVO-CONTENEDOR-ALMACEN".EventoComunes<ActivoContenedorAlmacen>(ConstantesAppGestionDocumental.APP_ID, ConstantesAppGestionDocumental.MODULO_ARCHIVOS));
            return l;
        }


        private List<TipoEventoAuditoria> EventosModuloPrestamo()
        {
            List<TipoEventoAuditoria> l = new List<TipoEventoAuditoria>();
            l.AddRange("EV-PRESTAMO".EventoComunes<Prestamo>(ConstantesAppGestionDocumental.APP_ID, ConstantesAppGestionDocumental.MODULO_PRESTAMO));
            l.AddRange("EV-ACTIVOS-PRESTAMO".EventoComunes<ActivoPrestamo>(ConstantesAppGestionDocumental.APP_ID, ConstantesAppGestionDocumental.MODULO_PRESTAMO));

            Type t = typeof(Prestamo);

            l.Add(new TipoEventoAuditoria()
            {
                AppId = ConstantesAppGestionDocumental.APP_ID,
                Descripcion = "EV-ENTREGA-PRESTAMO",
                ModuloId = ConstantesAppGestionDocumental.MODULO_PRESTAMO,
                TipoEntidad = t.Name,
                TipoEvento = 100
            });

            l.Add(new TipoEventoAuditoria()
            {
                AppId = ConstantesAppGestionDocumental.APP_ID,
                Descripcion = "EV-DEVOLUCION-PRESTAMO",
                ModuloId = ConstantesAppGestionDocumental.MODULO_PRESTAMO,
                TipoEntidad = t.Name,
                TipoEvento = 101
            });

            // NO ESTA N USO l.AddRange("EV-PERMISOS-UNIDADES-ADMIN".EventoComunes<ComentarioPrestamo>(ConstantesAppGestionDocumental.APP_ID, ConstantesAppGestionDocumental.MODULO_PRESTAMO));
            return l;
        }


        private List<TipoEventoAuditoria> EventosModuloTransferencias()
        {
            List<TipoEventoAuditoria> l = new List<TipoEventoAuditoria>();
            l.AddRange("EV-TRANSFERENCIA".EventoComunes<Transferencia>(ConstantesAppGestionDocumental.APP_ID, ConstantesAppGestionDocumental.MODULO_TRANSFERENCIA));
            l.AddRange("EV-ACTIVO-TRANSFERENCIA".EventoComunes<ActivoTransferencia>(ConstantesAppGestionDocumental.APP_ID, ConstantesAppGestionDocumental.MODULO_TRANSFERENCIA));


            Type t = typeof(Transferencia);

            l.Add(new TipoEventoAuditoria()
            {
                AppId = ConstantesAppGestionDocumental.APP_ID,
                Descripcion = "EV-ENVIAR-TX",
                ModuloId = ConstantesAppGestionDocumental.MODULO_TRANSFERENCIA,
                TipoEntidad = t.Name,
                TipoEvento = 102
            });

            l.Add(new TipoEventoAuditoria()
            {
                AppId = ConstantesAppGestionDocumental.APP_ID,
                Descripcion = "EV-ACEPTAR-OK-TX",
                ModuloId = ConstantesAppGestionDocumental.MODULO_TRANSFERENCIA,
                TipoEntidad = t.Name,
                TipoEvento = 103
            });

            l.Add(new TipoEventoAuditoria()
            {
                AppId = ConstantesAppGestionDocumental.APP_ID,
                Descripcion = "EV-ACEPTAR-PARCIAL-TX",
                ModuloId = ConstantesAppGestionDocumental.MODULO_TRANSFERENCIA,
                TipoEntidad = t.Name,
                TipoEvento = 104
            });

            l.Add(new TipoEventoAuditoria()
            {
                AppId = ConstantesAppGestionDocumental.APP_ID,
                Descripcion = "EV-DECLINAR-TX",
                ModuloId = ConstantesAppGestionDocumental.MODULO_TRANSFERENCIA,
                TipoEntidad = t.Name,
                TipoEvento = 105
            });


            //NO EN USO l.AddRange("EV-UNIDADES-ADMIN".EventoComunes<HistorialArchivoActivo>(ConstantesAppGestionDocumental.APP_ID, ConstantesAppGestionDocumental.MODULO_TRANSFERENCIA));
            //NO EN USO l.AddRange("EV-UNIDADES-ADMIN".EventoComunes<EventoTransferencia>(ConstantesAppGestionDocumental.APP_ID, ConstantesAppGestionDocumental.MODULO_TRANSFERENCIA));
            return l;
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
                    EventosAuditables = EventosModuloCuadroClasificacion()
                },
                new ElementoAplicacion(ConstantesAppGestionDocumental.APP_ID, ConstantesAppGestionDocumental.MODULO_ACTIVOS ) {
                    Titulo = "Gestión del acervo",
                    Descripcion = "Permite administrar el acervo para su gestión documental",
                    Tipos = new List<Type> { typeof(Activo),
                        typeof(Ampliacion)
                    },
                    EventosAuditables = EventosModuloInventario()
                },
                new ElementoAplicacion(ConstantesAppGestionDocumental.APP_ID, ConstantesAppGestionDocumental.MODULO_CATALOGOSCC ) {
                    Titulo = "Catálogos gestión documental",
                    Descripcion = "Permite administrar los catálogos relacionado con los cuadros de clasifiación",
                    Tipos = new List<Type> { typeof(EstadoCuadroClasificacion),
                            typeof(TipoDisposicionDocumental),
                            typeof(TipoValoracionDocumental),
                            typeof(TipoAmpliacion),
                            typeof(TipoArchivo)
                    },
                    EventosAuditables = EventosModuloCatalogo()
                },
                new ElementoAplicacion(ConstantesAppGestionDocumental.APP_ID, ConstantesAppGestionDocumental.MODULO_ARCHIVOS ) {
                    Titulo = "Gestión de archivos",
                    Descripcion = "Permite administrar los archivos para la localización del inventario",
                    Tipos = new List<Type> { typeof(Archivo), 
                        typeof(AlmacenArchivo), 
                        typeof(PermisosArchivo),
                        typeof(ZonaAlmacen), 
                        typeof(PosicionAlmacen), 
                        typeof(ContenedorAlmacen),
                        typeof(ActivoContenedorAlmacen) },
                    EventosAuditables = EventosModuloArchivos()
                },
                new ElementoAplicacion(ConstantesAppGestionDocumental.APP_ID, ConstantesAppGestionDocumental.MODULO_UNIDADESADMIN ) {
                    Titulo = "Gestión de unidades administrativas",
                    Descripcion = "Permite gestionar las unidades administrativas para el archivo",
                    Tipos = new List<Type> { typeof(UnidadAdministrativaArchivo) ,
                        typeof(PermisosUnidadAdministrativaArchivo)
                    },
                    EventosAuditables = EventosModuloUnidadesAdministrativas()
                },
                new ElementoAplicacion(ConstantesAppGestionDocumental.APP_ID, ConstantesAppGestionDocumental.MODULO_PRESTAMO ) {
                    Titulo = "Gestión de préstamo",
                    Descripcion = "Permite realizar la getsión del préstamo físico de los activos del acervo",
                    Tipos = new List<Type> { typeof(Prestamo), 
                        typeof(ActivoPrestamo), 
                        typeof(ComentarioPrestamo) },
                    EventosAuditables = EventosModuloPrestamo()
                },
                new ElementoAplicacion(ConstantesAppGestionDocumental.APP_ID, ConstantesAppGestionDocumental.MODULO_TRANSFERENCIA ) {
                    Titulo = "Gestión de trasnferencias entre archivos",
                    Descripcion = "Permite realizar la gestión de las trasnferencias de inventarios entre archivos",
                    Tipos = new List<Type> { typeof(Transferencia), 
                        typeof(EventoTransferencia),
                        typeof(ComentarioTransferencia),
                        typeof(ActivoTransferencia),
                        typeof(EstadoTransferencia),
                        typeof(HistorialArchivoActivo)},
                     EventosAuditables = EventosModuloTransferencias()
                },
                // SE INCLUYO EN LOS CATALOGOS new ElementoAplicacion(ConstantesAppGestionDocumental.APP_ID, ConstantesAppGestionDocumental.MODULO_CAT_ARCHIVO ) {
                //    Titulo = "Catálogos archivo",
                //    Descripcion = "Permite administrar los catálogos relacionados con los archivos",
                //    Tipos = new List<Type> { typeof(TipoArchivo) }
                //},
                //NO SE UTILIZA EL ESTADO DE LAS TRANSFERENCIAS ES FIJO new ElementoAplicacion(ConstantesAppGestionDocumental.APP_ID, ConstantesAppGestionDocumental.MODULO_CAT_TRANSFERENCIA ) {
                //    Titulo = "Catálogos transferencias",
                //    Descripcion = "Permite realizar la gestión de catálogos relacionados con las transferencia",
                //    Tipos = new List<Type> { typeof(EstadoTransferencia) }
                //},
                //NO SE UTILIZA new ElementoAplicacion(ConstantesAppGestionDocumental.APP_ID,ConstantesAppGestionDocumental.MODULO_RESUMEN_ACTIVOS)
                //{
                //    Titulo="Resumen de activos por cuadro clasificación y Archivo",
                //    Descripcion="Permite realizar un resumen de todos los activos que se encuentren almacenados en la estadística",
                //    Tipos=new List<Type>{ typeof(EstadisticaClasificacionAcervo)}
                //}
              };
            return m;
        }



    }
}
