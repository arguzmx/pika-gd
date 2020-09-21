using System;
using System.Collections.Generic;
using System.Text;
using PIKA.Infraestructura.Comun;
using PIKA.Infraestructura.Comun.Constantes;
using PIKA.Infraestructura.Comun.Seguridad;
using PIKA.Modelo.Contenido;
using Version = PIKA.Modelo.Contenido.Version;

namespace PIKA.Servicio.Contenido
{
    public class AplicacionContenido : InformacionAplicacionBase, IInformacionAplicacion
    {
        public const string MODULO_BASE = "PIKA-GD-COTENIDO-";

        public override Aplicacion Info()
        {
            Aplicacion a = AplicacionRaiz.ObtieneAplicacionRaiz();
            a.Modulos = this.ModulosAplicacion();
            return a;
        }

        public List<ModuloAplicacion> ModulosAplicacion()
        {
            return this.ModulosAplicacionLocales(AplicacionRaiz.APP_ID, MODULO_BASE);
        }

        public override List<ElementoAplicacion> GetModulos()
        {
            List<ElementoAplicacion> m = new List<ElementoAplicacion>()
            {
                new ElementoAplicacion(MODULO_BASE, "AZURE-STORAGE" ) { 
                    Titulo = "Configuración de almacenamiento Azure", 
                    Descripcion = "Permite administrar la configuración de los almacenamientos Azure", 
                    Tipos = new List<Type> { typeof(GestorAzureConfig) }  
                },
                new ElementoAplicacion(MODULO_BASE, "SMB-STORAGE" ) {
                    Titulo = "Configuración de almacenamiento SMB",
                    Descripcion = "Permite administrar la configuración de los almacenamientos SMB",
                    Tipos = new List<Type> { typeof(GestorSMBConfig) }
                },
                new ElementoAplicacion(MODULO_BASE, "LOCAL-STORAGE" ) {
                    Titulo = "Configuración de almacenamiento local",
                    Descripcion = "Permite administrar la configuración de los almacenamientos locales",
                    Tipos = new List<Type> { typeof(GestorLocalConfig) }
                },
                new ElementoAplicacion(MODULO_BASE, "PARTES" ) {
                    Titulo = "Partes de versiones de elementos de contenido",
                    Descripcion = "Permite administrar las partes para las versiones de los elementos de contenido",
                    Tipos = new List<Type> { typeof(Parte) }
                },
                new ElementoAplicacion(MODULO_BASE, "VERSIONES" ) {
                    Titulo = "Versiones para elementos de contenido",
                    Descripcion = "Permite administrar Version versiones para los elementos de contenido",
                    Tipos = new List<Type> { typeof(Parte) }
                },
                new ElementoAplicacion(MODULO_BASE, "ELEMENTOS" ) {
                    Titulo = "Elementos de contenido del repositorio",
                    Descripcion = "Permite gestionar los elementos de contenido de un reositorio",
                    Tipos = new List<Type> { typeof(Elemento) }
                },
                new ElementoAplicacion(MODULO_BASE, "FOLDERS" ) {
                    Titulo = "Carpetas de repositorio",
                    Descripcion = "Permite gestionar las carpetas de un reositorio",
                    Tipos = new List<Type> { typeof(Carpeta) }
                },
                new ElementoAplicacion(MODULO_BASE, "VOL-PM" ) {
                    Titulo = "Volúmnes para puntos de montaje de contenido",
                    Descripcion = "Permite administrar los valomenes asociados a los puntos de montaje para los repositorios de contenidos",
                    Tipos = new List<Type> { typeof(VolumenPuntoMontaje) }
                },
                new ElementoAplicacion(MODULO_BASE, "PMS-1" ) {
                    Titulo = "Puntos de montaje de contenido",
                    Descripcion =  "Permite administrar los puntos de montaje para los repositorios de contenidos",
                    Tipos = new List<Type> { typeof(PuntoMontaje) }
                },
                new ElementoAplicacion(MODULO_BASE, "PMS-2" ) {
                    Titulo = "Gestores de entrada/salida",
                    Descripcion =  "Catálogo de gestores de entrada salida para los volumenes de contenido",
                    Tipos = new List<Type> { typeof(TipoGestorES) }
                },
                new ElementoAplicacion(MODULO_BASE, "PMS-3" ) {
                    Titulo = "Permisos sobre el contenido",
                    Descripcion =  "Permite administrar los permisos sobre el contenido",
                    Tipos = new List<Type> { typeof(Permiso) }
                },
                new ElementoAplicacion(MODULO_BASE, "PMS-4" ) {
                    Titulo = "Destinatarios de los permisos",
                    Descripcion =  "Permite administrar los destinatarios para un permiso de acceso",
                    Tipos = new List<Type> { typeof(DestinatarioPermiso) }
                },
                new ElementoAplicacion(MODULO_BASE, "PMS-5" ) {
                    Titulo =  "Volumenes de contenido",
                    Descripcion =  "Permite administrar los volumenes para los repositorios de contenidos",
                    Tipos = new List<Type> { typeof(Volumen) }
                }

            };
            


            return m;
        }

      
    }

}
