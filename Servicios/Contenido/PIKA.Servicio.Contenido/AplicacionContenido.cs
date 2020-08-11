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
    public class AplicacionContenido : IInformacionAplicacion
    {
        public const string MODULO_BASE = "PIKA-GD-COTENIDO-";
        public const string APP_ID = "PIKA-GD-COTENIDO";

        public Aplicacion Info()
        {
            Aplicacion a = ConstantesAplicacion.AplicacionPikaGD();
            a.Modulos = this.ModulosAplicacion();
            return a;
        }


     
        public List<ElementoAplicacion> GetModulos()
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
                new ElementoAplicacion(MODULO_BASE, "PMS" ) {
                    Titulo = "Puntos de montaje de contenido",
                    Descripcion =  "Permite administrar los puntos de montaje para los repositorios de contenidos",
                    Tipos = new List<Type> { typeof(PuntoMontaje) }
                },
                new ElementoAplicacion(MODULO_BASE, "PMS" ) {
                    Titulo = "Gestores de entrada/salida",
                    Descripcion =  "Catálogo de gestores de entrada salida para los volumenes de contenido",
                    Tipos = new List<Type> { typeof(TipoGestorES) }
                },
                new ElementoAplicacion(MODULO_BASE, "PMS" ) {
                    Titulo = "Permisos sobre el contenido",
                    Descripcion =  "Permite administrar los permisos sobre el contenido",
                    Tipos = new List<Type> { typeof(Permiso) }
                },
                new ElementoAplicacion(MODULO_BASE, "PMS" ) {
                    Titulo = "Destinatarios de los permisos",
                    Descripcion =  "Permite administrar los destinatarios para un permiso de acceso",
                    Tipos = new List<Type> { typeof(DestinatarioPermiso) }
                },
                new ElementoAplicacion(MODULO_BASE, "PMS" ) {
                    Titulo =  "Volumenes de contenido",
                    Descripcion =  "Permite administrar los volumenes para los repositorios de contenidos",
                    Tipos = new List<Type> { typeof(Volumen) }
                }

            };
            


            return m;
        }


        public List<ModuloAplicacion> ModulosAplicacion()
        {
            List<ModuloAplicacion> l = new List<ModuloAplicacion>();

            /// Modulo Raíz 
            //------------------------------------------------------------
            ModuloAplicacion mAdministracion = new ModuloAplicacion(
                ConstantesAplicacion.Id,
                MODULO_BASE, true,
                "Administración de contenido",
                "Agrupa las funciones para la administración de repositorios de contenido",
                "",
                "es-MX",
                PermisoAplicacion.PermisosAdministrables(),
                "",
                "");

            l.Add(mAdministracion);

            foreach (var item in GetModulos())
            {
                Console.WriteLine( item.Descripcion );
                l.Add(ElementoAplicacion.CreaModuloTipico(APP_ID, MODULO_BASE,
                    item.IdModulo, item.Titulo, item.Descripcion, item.Tipos));
            }

            return l;
        }

        public List<TipoAdministradorModulo> TiposAdministrados()
        {

            List<TipoAdministradorModulo> tipos = new List<TipoAdministradorModulo>();
            Aplicacion a = this.Info();

            foreach (var m in a.Modulos)
            {

                foreach (var t in m.TiposAdministrados)
                {
                    tipos.Add(t);
                }
            }

            return tipos;
        }
    }

}
