using System;
using System.Collections.Generic;
using System.Text;
using PIKA.Infraestructura.Comun;
using PIKA.Infraestructura.Comun.Constantes;
using PIKA.Infraestructura.Comun.Seguridad;
using PIKA.Modelo.Aplicacion.Plugins;

namespace PIKA.Servicio.AplicacionPlugin
{
    public class AplicacionAplicacionPlugin : IInformacionAplicacion
    {
        public const string MODULO_BASE_APLICACION = "PIKA-GD-APLI";
        public const string MODULO_APLICACION_PLUGIN = "PIKA-GD-APLI-PLUGIN";
        public const string MODULO_APLICACION_VERSION_PLUGIN = "PIKA-GD-APLI_VERSIONPLUGIN";
        public const string MODULO_APLICACION_PLUGIN_INSTALADO = "PIKA-GD-APLI-PLUGININSTALADO";

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

            string IdModuloAdminOrg = $"{MODULO_BASE_APLICACION}-ADMIN";


            /// Modulo administarción 
            //------------------------------------------------------------
            ModuloAplicacion mAdministracion = new ModuloAplicacion(
                ConstantesAplicacion.Id,
                IdModuloAdminOrg, true,
                "Administrador Seguridad",
                "Permite arministrar la seguridad de la aplicacion",
                "",
                "es-MX",
                PermisoAplicacion.PermisosAdministrables(),
                "",
                "");


            l.Add(mAdministracion);

            //------------------------------------------------------------
            //------------------------------------------------------------


            /// Modulo administarcion de Aplicacion
            //------------------------------------------------------------

            m = new ModuloAplicacion(ConstantesAplicacion.Id, MODULO_APLICACION_PLUGIN, true,
                "Aplicaciones",
                "Administrador de aplicaciones",
                "",
                "es-MX",
                PermisoAplicacion.PermisosAdministrables(), IdModuloAdminOrg,
                ConstantesAplicacion.Id);
            m.TiposAdministrados.Add(new TipoAdministradorModulo()
            {
                AplicacionId = ConstantesAplicacion.Id,
                ModuloId = m.Id,
                TiposAdministrados = new List<Type>() { typeof(Plugin) }
            });
            l.Add(m);
            //------------------------------------------------------------
            //------------------------------------------------------------



            /// Modulo aplicaciòn 
            //------------------------------------------------------------

            m = new ModuloAplicacion(ConstantesAplicacion.Id, MODULO_APLICACION_VERSION_PLUGIN, true,
                "Modulo aplicaciòn",
                "Administrador de Modulos Aplicaicones",
                "",
                "es-MX",
                PermisoAplicacion.PermisosAdministrables(), IdModuloAdminOrg,
                ConstantesAplicacion.Id);
            m.TiposAdministrados.Add(new TipoAdministradorModulo()
            {
                AplicacionId = ConstantesAplicacion.Id,
                ModuloId = m.Id,
                TiposAdministrados = new List<Type>() { typeof(VersionPlugin) }
            });
            l.Add(m);
            //------------------------------------------------------------
            //------------------------------------------------------------


            /// Modulo tipo administrador modulo
            //------------------------------------------------------------

            m = new ModuloAplicacion(ConstantesAplicacion.Id, MODULO_APLICACION_PLUGIN_INSTALADO, true,
                "Tipo administrador modulo",
                "Administrador de tipos administradores modulos",
                "",
                "es-MX",
                PermisoAplicacion.PermisosAdministrables(), IdModuloAdminOrg,
                ConstantesAplicacion.Id);
            m.TiposAdministrados.Add(new TipoAdministradorModulo()
            {
                AplicacionId = ConstantesAplicacion.Id,
                ModuloId = m.Id,
                TiposAdministrados = new List<Type>() { typeof(PluginInstalado) }
            });
            l.Add(m);
          
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
