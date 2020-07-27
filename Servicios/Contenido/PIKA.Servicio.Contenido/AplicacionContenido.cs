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
        public const string MODULO_BASE_CONTENIDO = "PIKA-GD-COTENIDO-";
        public const string MODULO_GESTOR_ES = "GESTORES-ES";
        public const string MODULO_PERMISOS = "PERMISOS";
        public const string MODULO_DESTINATARIOS_PERMISOS = "DEST-PERMISOS";
        public const string MODULO_VOLUMENES = "VOLS";
        public const string MODULO_PUNTOS_MONTAJE = "PUNTOSMONTAJE";
        public const string MODULO_VOL_PUNTOS_MONTAJE = "VOL-PUNTOSMONTAJE";
        public const string MODULO_CARPETAS = "CARPETAS";
        public const string MODULO_ELEMENTOS = "ELEMENTOS";
        public const string MODULO_VERSIONES = "VERSIONS";
        public const string MODULO_VERSIONES_PARTES = "PARTESVERSIONES";
        public const string MODULO_CONFIG_ALMACENAMIENTO_LOCAL = "CONFIG-ALM-LOCAL";
        public const string MODULO_CONFIG_ALMACENAMIENTO_SMB = "CONFIG-ALM-SMB";
        public const string MODULO_CONFIG_ALMACENAMIENTO_AZURE = "CONFIG-ALM-AZURE";

        public static string ID_APLICAICON { get { return ConstantesAplicacion.Id; } }

        public Aplicacion Info()
        {
            Aplicacion a = ConstantesAplicacion.AplicacionPikaGD();
            a.Modulos = this.ModulosAplicacion();
            return a;
        }


        public List<ModuloAplicacion> ModulosAplicacion()
        {
            List<ModuloAplicacion> l = new List<ModuloAplicacion>();
            ModuloAplicacion m;

            string IdModuloRaiz = $"{MODULO_BASE_CONTENIDO}ADMIN";

            /// Modulo de MODULO_CONFIG_ALMACENAMIENTO_SMB 
            //------------------------------------------------------------

            m = new ModuloAplicacion(ConstantesAplicacion.Id, $"{MODULO_BASE_CONTENIDO}{MODULO_CONFIG_ALMACENAMIENTO_AZURE}", true,
                "Configuración de almacenamiento Azure",
                "Permite administrar la configuración de los almacenamientos Azure",
                "",
                "es-MX",
                PermisoAplicacion.PermisosAdministrables(), IdModuloRaiz,
                ConstantesAplicacion.Id);
            m.TiposAdministrados.Add(new TipoAdministradorModulo()
            {
                AplicacionId = ConstantesAplicacion.Id,
                ModuloId = m.Id,
                TiposAdministrados = new List<Type>() { typeof(GestorAzureConfig) }
            });
            l.Add(m);
            //--------


            /// Modulo de MODULO_CONFIG_ALMACENAMIENTO_SMB 
            //------------------------------------------------------------

            m = new ModuloAplicacion(ConstantesAplicacion.Id, $"{MODULO_BASE_CONTENIDO}{MODULO_CONFIG_ALMACENAMIENTO_SMB}", true,
                "Configuración de almacenamiento SMB",
                "Permite administrar la configuración de los almacenamientos SMB",
                "",
                "es-MX",
                PermisoAplicacion.PermisosAdministrables(), IdModuloRaiz,
                ConstantesAplicacion.Id);
            m.TiposAdministrados.Add(new TipoAdministradorModulo()
            {
                AplicacionId = ConstantesAplicacion.Id,
                ModuloId = m.Id,
                TiposAdministrados = new List<Type>() { typeof(GestorSMBConfig) }
            });
            l.Add(m);
            //--------


            /// Modulo de MODULO_CONFIG_ALMACENAMIENTO_LOCAL 
            //------------------------------------------------------------

            m = new ModuloAplicacion(ConstantesAplicacion.Id, $"{MODULO_BASE_CONTENIDO}{MODULO_CONFIG_ALMACENAMIENTO_LOCAL}", true,
                "Configuración de almacenamiento local",
                "Permite administrar la configuración de los almacenamientos locales",
                "",
                "es-MX",
                PermisoAplicacion.PermisosAdministrables(), IdModuloRaiz,
                ConstantesAplicacion.Id);
            m.TiposAdministrados.Add(new TipoAdministradorModulo()
            {
                AplicacionId = ConstantesAplicacion.Id,
                ModuloId = m.Id,
                TiposAdministrados = new List<Type>() { typeof(GestorLocalConfig) }
            });
            l.Add(m);
            //--------


            /// Modulo de MODULO_VERSIONES 
            //------------------------------------------------------------

            m = new ModuloAplicacion(ConstantesAplicacion.Id, $"{MODULO_BASE_CONTENIDO}{MODULO_VERSIONES_PARTES}", true,
                "Partes de versiones de elementos de contenido",
                "Permite administrar las partes para las versiones de los elementos de contenido",
                "",
                "es-MX",
                PermisoAplicacion.PermisosAdministrables(), IdModuloRaiz,
                ConstantesAplicacion.Id);
            m.TiposAdministrados.Add(new TipoAdministradorModulo()
            {
                AplicacionId = ConstantesAplicacion.Id,
                ModuloId = m.Id,
                TiposAdministrados = new List<Type>() { typeof(Parte) }
            });
            l.Add(m);
            //------------------------------------------------------------


            /// Modulo de MODULO_VERSIONES 
            //------------------------------------------------------------

            m = new ModuloAplicacion(ConstantesAplicacion.Id, $"{MODULO_BASE_CONTENIDO}{MODULO_VERSIONES}", true,
                "Versiones para elementos de contenido",
                "Permite administrar las versiones para los elementos de contenido",
                "",
                "es-MX",
                PermisoAplicacion.PermisosAdministrables(), IdModuloRaiz,
                ConstantesAplicacion.Id);
            m.TiposAdministrados.Add(new TipoAdministradorModulo()
            {
                AplicacionId = ConstantesAplicacion.Id,
                ModuloId = m.Id,
                TiposAdministrados = new List<Type>() { typeof(Version) }
            });
            l.Add(m);
            //------------------------------------------------------------



            /// Modulo de MODULO_ELEMENTOS 
            //------------------------------------------------------------

            m = new ModuloAplicacion(ConstantesAplicacion.Id, $"{MODULO_BASE_CONTENIDO}{MODULO_ELEMENTOS}", true,
                "Elementos de contenido del repositorio",
                "Permite gestionar los elementos de contenido de un reositorio",
                "",
                "es-MX",
                PermisoAplicacion.PermisosAdministrables(), IdModuloRaiz,
                ConstantesAplicacion.Id);
            m.TiposAdministrados.Add(new TipoAdministradorModulo()
            {
                AplicacionId = ConstantesAplicacion.Id,
                ModuloId = m.Id,
                TiposAdministrados = new List<Type>() { typeof(Elemento) }
            });
            l.Add(m);
            //------------------------------------------------------------



            /// Modulo de MODULO_CARPETAS 
            //------------------------------------------------------------

            m = new ModuloAplicacion(ConstantesAplicacion.Id, $"{MODULO_BASE_CONTENIDO}{MODULO_CARPETAS}", true,
                "CArpetas de repositorio",
                "Permite gestionar las carpetas de un reositorio",
                "",
                "es-MX",
                PermisoAplicacion.PermisosAdministrables(), IdModuloRaiz,
                ConstantesAplicacion.Id);
            m.TiposAdministrados.Add(new TipoAdministradorModulo()
            {
                AplicacionId = ConstantesAplicacion.Id,
                ModuloId = m.Id,
                TiposAdministrados = new List<Type>() { typeof(Carpeta) }
            });
            l.Add(m);
            //------------------------------------------------------------


            /// Modulo de MODULO_VOL_PUNTOS_MONTAJE 
            //------------------------------------------------------------

            m = new ModuloAplicacion(ConstantesAplicacion.Id, $"{MODULO_BASE_CONTENIDO}{MODULO_VOL_PUNTOS_MONTAJE}", true,
                "Volúmnes para puntos de montaje de contenido",
                "Permite administrar los valomenes asociados a los puntos de montaje para los repositorios de contenidos",
                "",
                "es-MX",
                PermisoAplicacion.PermisosAdministrables(), IdModuloRaiz,
                ConstantesAplicacion.Id);
            m.TiposAdministrados.Add(new TipoAdministradorModulo()
            {
                AplicacionId = ConstantesAplicacion.Id,
                ModuloId = m.Id,
                TiposAdministrados = new List<Type>() { typeof(VolumenPuntoMontaje) }
            });
            l.Add(m);
            //------------------------------------------------------------


            /// Modulo de MODULO_PUNTOS_MONTAJE 
            //------------------------------------------------------------

            m = new ModuloAplicacion(ConstantesAplicacion.Id, $"{MODULO_BASE_CONTENIDO}{MODULO_PUNTOS_MONTAJE}", true,
                "Puntos de montaje de contenido",
                "Permite administrar los puntos de montaje para los repositorios de contenidos",
                "",
                "es-MX",
                PermisoAplicacion.PermisosAdministrables(), IdModuloRaiz,
                ConstantesAplicacion.Id);
            m.TiposAdministrados.Add(new TipoAdministradorModulo()
            {
                AplicacionId = ConstantesAplicacion.Id,
                ModuloId = m.Id,
                TiposAdministrados = new List<Type>() { typeof(PuntoMontaje) }
            });
            l.Add(m);
            //------------------------------------------------------------
            //------------------------------------------------------------


            /// Modulo Raíz 
            //------------------------------------------------------------
            ModuloAplicacion mAdministracion = new ModuloAplicacion(
                ConstantesAplicacion.Id,
                IdModuloRaiz, true,
                "Administración de contenido",
                "Agrupa las funciones para la administarción de repositorios de contenido",
                "",
                "es-MX",
                PermisoAplicacion.PermisosAdministrables(),
                "",
                "");


            l.Add(mAdministracion);

            //------------------------------------------------------------
            //------------------------------------------------------------


            /// Modulo administarcion de Elementos
            //------------------------------------------------------------

            m = new ModuloAplicacion(ConstantesAplicacion.Id, $"{MODULO_BASE_CONTENIDO}{MODULO_GESTOR_ES}", true,
                "Gestores de entrada/salida",
                "Catálogo de gestores de entrada salida para los volumenes de contenido",
                "",
                "es-MX",
                PermisoAplicacion.PermisosAdministrables(), IdModuloRaiz,
                ConstantesAplicacion.Id);
            m.TiposAdministrados.Add(new TipoAdministradorModulo()
            {
                AplicacionId = ConstantesAplicacion.Id,
                ModuloId = m.Id,
                TiposAdministrados = new List<Type>() { typeof(TipoGestorES) }
            });
            l.Add(m);
            //------------------------------------------------------------
            //------------------------------------------------------------


            /// Modulo de MODULO_PERMISOS 
            //------------------------------------------------------------

            m = new ModuloAplicacion(ConstantesAplicacion.Id, $"{MODULO_BASE_CONTENIDO}{MODULO_PERMISOS}", true,
                "Permisos sobre el contenido",
                "Permite administrar los permisos sobre el contenido",
                "",
                "es-MX",
                PermisoAplicacion.PermisosAdministrables(), IdModuloRaiz,
                ConstantesAplicacion.Id);
            m.TiposAdministrados.Add(new TipoAdministradorModulo()
            {
                AplicacionId = ConstantesAplicacion.Id,
                ModuloId = m.Id,
                TiposAdministrados = new List<Type>() { typeof(Permiso) }
            });
            l.Add(m);
            //------------------------------------------------------------
            //------------------------------------------------------------

            /// Modulo de MODULO_DESTINATARIOS_PERMISOS 
            //------------------------------------------------------------

            m = new ModuloAplicacion(ConstantesAplicacion.Id, $"{MODULO_BASE_CONTENIDO}{MODULO_DESTINATARIOS_PERMISOS}", true,
                "Destinatarios de los permisos",
                "Permite administrar los destinatarios para un permiso de acceso",
                "",
                "es-MX",
                PermisoAplicacion.PermisosAdministrables(), IdModuloRaiz,
                ConstantesAplicacion.Id);
            m.TiposAdministrados.Add(new TipoAdministradorModulo()
            {
                AplicacionId = ConstantesAplicacion.Id,
                ModuloId = m.Id,
                TiposAdministrados = new List<Type>() { typeof(DestinatarioPermiso) }
            });
            l.Add(m);
            //------------------------------------------------------------
            //------------------------------------------------------------



            /// Modulo de MODULO_DESTINATARIOS_PERMISOS 
            //------------------------------------------------------------

            m = new ModuloAplicacion(ConstantesAplicacion.Id, $"{MODULO_BASE_CONTENIDO}{MODULO_VOLUMENES}", true,
                "Volumenes de contenido",
                "Permite administrar los volumenes para los repositorios de contenidos",
                "",
                "es-MX",
                PermisoAplicacion.PermisosAdministrables(), IdModuloRaiz,
                ConstantesAplicacion.Id);
            m.TiposAdministrados.Add(new TipoAdministradorModulo()
            {
                AplicacionId = ConstantesAplicacion.Id,
                ModuloId = m.Id,
                TiposAdministrados = new List<Type>() { typeof(Volumen) }
            });
            l.Add(m);
            //------------------------------------------------------------
            //------------------------------------------------------------


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
