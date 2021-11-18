using System;
using System.Collections.Generic;
using PIKA.Infraestructura.Comun;
using PIKA.Constantes.Aplicaciones.Contenido;
using PIKA.Modelo.CarpetaInteligente;
using PIKA.Constantes.Aplicaciones.CarpetaInteligente;

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
            return this.ModulosAplicacionLocales(ConstantesAppCarpetaInteligente.APP_ID, ConstantesAppCarpetaInteligente.APP_ID);
        }

        public override List<ElementoAplicacion> GetModulos()
        {
            List<ElementoAplicacion> m = new List<ElementoAplicacion>()
            {
                new ElementoAplicacion(ConstantesAppCarpetaInteligente.APP_ID, ConstantesAppCarpetaInteligente.ADMINISTRACION_ID) {
                    Titulo = "Configuración de la carpeta inteligente",
                    Descripcion = "Permite administrar la configuración de las carpetas inteligentes",
                    Tipos = new List<Type> {
                        typeof(CarpetaInteligente),
                    }
                },
                // new ElementoAplicacion(ConstantesAppContenido.APP_ID, ConstantesAppContenido.MODULO_ESTRUCTURA_CONTENIDO ) {
                //    Titulo = "Gestión de elementos de contenido",
                //    Descripcion = "Permite realizar la gestión de contenido del repositorio, por ejemplo carpetas y documentos",
                //    Tipos = new List<Type> {
                //        typeof(Carpeta),
                //        typeof(Parte),
                //        typeof(Version),
                //        typeof(Permiso),
                //        typeof(DestinatarioPermiso),
                //        typeof(Elemento)
                //    }
                //},
                //     new ElementoAplicacion(ConstantesAppContenido.APP_ID, ConstantesAppContenido.MODULO_VISOR_CONTENIDO ) {
                //    Titulo = "Acceso al visor de contenido",
                //    Descripcion = "Permite acceder al visor de cotenids para su visualización y edición",
                //    Tipos = new List<Type> {}
                //},
                //     new ElementoAplicacion(ConstantesAppContenido.APP_ID, ConstantesAppContenido.MODULO_PERMISOS_CONTENIDO ) {
                //    Titulo = "Acceso al administrador de permisos de contenido",
                //    Descripcion = "Permite acceder administardor de permisos de acceso de contenido que define la seguridad del mismo",
                //    Tipos = new List<Type> {}
                //}

            };



            return m;
        }


    }

}
