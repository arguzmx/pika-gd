using System;
using System.Collections.Generic;
using PIKA.Infraestructura.Comun;
using PIKA.Modelo.Contenido;
using Version = PIKA.Modelo.Contenido.Version;
using PIKA.Constantes.Aplicaciones.Contenido;

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
                        typeof(Volumen),
                        typeof(GestorLocalConfig)
                    }
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
                        typeof(Elemento)
                    }
                },
                     new ElementoAplicacion(ConstantesAppContenido.APP_ID, ConstantesAppContenido.MODULO_VISOR_CONTENIDO ) {
                    Titulo = "Acceso al visor de contenido",
                    Descripcion = "Permite acceder al visor de cotenids para su visualización y edición",
                    Tipos = new List<Type> {}
                },
                     new ElementoAplicacion(ConstantesAppContenido.APP_ID, ConstantesAppContenido.MODULO_PERMISOS_CONTENIDO ) {
                    Titulo = "Acceso al administrador de permisos de contenido",
                    Descripcion = "Permite acceder administardor de permisos de acceso de contenido que define la seguridad del mismo",
                    Tipos = new List<Type> {}
                }

            };



            return m;
        }


    }

}
