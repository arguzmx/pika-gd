using System;
using System.Collections.Generic;
using System.Text;
using PIKA.Infraestructura.Comun;
using PIKA.Infraestructura.Comun.Constantes;
using PIKA.Infraestructura.Comun.Seguridad;
using PIKA.Modelo.Seguridad;

namespace PIKA.Servicio.Seguridad
{
    public class AplicacionSeguridad : IInformacionAplicacion
    {
        public const string MODULO_BASE_SEGURIDAD = "PIKA-GD-SEG";
        public const string MODULO_SEGURIDAD_USUARIOS = "PIKA-GD-SEG_USUSARIOS";
        public const string MODULO_SEGURIDAD_APLICACION = "PIKA-GD-SEG-APLICIACION";
        public const string MODULO_SEGURIDAD_MODULO_APLICACION = "PIKA-GD-SEG_MODAPLI";
        public const string MODULO_SEGURIDAD_TIPO_ADMINISTRADOR_MODULO = "PIKA-GD-SEG-TIPO-ADMONAPP";
        public const string MODULO_SEGURIDAD_TRADUCCION_APLICACION_MODULO = "PIKA-GD-SEG-TRADU-APPMODU";

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

            string IdModuloBase = $"{MODULO_BASE_SEGURIDAD}-ADMIN";


            /// Modulo administarción 
            //------------------------------------------------------------
            ModuloAplicacion mAdministracion = new ModuloAplicacion(
                ConstantesAplicacion.Id,
                IdModuloBase, true,
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

            m = new ModuloAplicacion(ConstantesAplicacion.Id, MODULO_SEGURIDAD_APLICACION, true,
                "Aplicaciones",
                "Administrador de aplicaciones",
                "",
                "es-MX",
                PermisoAplicacion.PermisosAdministrables(), IdModuloBase,
                ConstantesAplicacion.Id);
            m.TiposAdministrados.Add(new TipoAdministradorModulo()
            {
                AplicacionId = ConstantesAplicacion.Id,
                ModuloId = m.Id,
                TiposAdministrados = new List<Type>() { typeof(Aplicacion) }
            });
            l.Add(m);
            //------------------------------------------------------------
            //------------------------------------------------------------



            /// Modulo aplicaciòn 
            //------------------------------------------------------------

            m = new ModuloAplicacion(ConstantesAplicacion.Id, MODULO_SEGURIDAD_MODULO_APLICACION, true,
                "Modulo aplicaciòn",
                "Administrador de Modulos Aplicaicones",
                "",
                "es-MX",
                PermisoAplicacion.PermisosAdministrables(), IdModuloBase,
                ConstantesAplicacion.Id);
            m.TiposAdministrados.Add(new TipoAdministradorModulo()
            {
                AplicacionId = ConstantesAplicacion.Id,
                ModuloId = m.Id,
                TiposAdministrados = new List<Type>() { typeof(ModuloAplicacion) }
            });
            l.Add(m);
            //------------------------------------------------------------
            //------------------------------------------------------------


            /// Modulo tipo administrador modulo
            //------------------------------------------------------------

            m = new ModuloAplicacion(ConstantesAplicacion.Id, MODULO_SEGURIDAD_TIPO_ADMINISTRADOR_MODULO, true,
                "Tipo administrador modulo",
                "Administrador de tipos administradores modulos",
                "",
                "es-MX",
                PermisoAplicacion.PermisosAdministrables(), IdModuloBase,
                ConstantesAplicacion.Id);
            m.TiposAdministrados.Add(new TipoAdministradorModulo()
            {
                AplicacionId = ConstantesAplicacion.Id,
                ModuloId = m.Id,
                TiposAdministrados = new List<Type>() { typeof(TipoAdministradorModulo) }
            });
            l.Add(m);
            //------------------------------------------------------------
            //------------------------------------------------------------

            /// Modulo Sefuridad traduccion aplicacion modulo
            //------------------------------------------------------------

            m = new ModuloAplicacion(ConstantesAplicacion.Id, MODULO_SEGURIDAD_TRADUCCION_APLICACION_MODULO, true,
                "Traduccion aplicaciòn modulo ",
                "Administrador de traduccciones de aplicaciones de modulos",
                "",
                "es-MX",
                PermisoAplicacion.PermisosAdministrables(), IdModuloBase,
                ConstantesAplicacion.Id);
            m.TiposAdministrados.Add(new TipoAdministradorModulo()
            {
                AplicacionId = ConstantesAplicacion.Id,
                ModuloId = m.Id,
                TiposAdministrados = new List<Type>() { typeof(TraduccionAplicacionModulo) }
            });
            l.Add(m);
            //------------------------------------------------------------
            //------------------------------------------------------------

            /// Modulo usuarios de la aplicación
            //------------------------------------------------------------

            m = new ModuloAplicacion(ConstantesAplicacion.Id, MODULO_SEGURIDAD_USUARIOS, true,
                "Usuarios de la aplicación",
                "Administrador de usuarios de la aplicación",
                "",
                "es-MX",
                PermisoAplicacion.PermisosAdministrables(), IdModuloBase,
                ConstantesAplicacion.Id);
            m.TiposAdministrados.Add(new TipoAdministradorModulo()
            {
                AplicacionId = ConstantesAplicacion.Id,
                ModuloId = m.Id,
                TiposAdministrados = new List<Type>() { typeof(PropiedadesUsuario) }
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
