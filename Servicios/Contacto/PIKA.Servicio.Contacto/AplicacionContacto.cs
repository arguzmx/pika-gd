using PIKA.Infraestructura.Comun;
using PIKA.Infraestructura.Comun.Constantes;
using PIKA.Infraestructura.Comun.Seguridad;
using PIKA.Modelo.Contacto;
using System;
using System.Collections.Generic;
using System.Text;

namespace PIKA.Servicio.Contacto
{
    public class AplicacionContacto : IInformacionAplicacion
    {
        public const string MODULO_BASE_CONTACTO = "PIKA-GD-CONTACTO";
        public const string MODULO_CONTACTO_ADMIN = "PIKA-GD-ADMIN";
        public const string MODULO_ESTADO = "PIKA-GD-CONTACTO-ESTADO";
        public const string MODULO_PAIS = "PIKA-GD-CONTACTO-PAIS";
        public const string MODULO_TIPO_MEDIOS = "PIKA-GD-CONTACTO-TMEDIOS";
        public const string MODULO_FUENTES = "PIKA-GD-CONTACTO-FUENTE";
        public const string MODULO_DIRECCION_POSTAL = "PIKA-GD-CONTACTO-POSTAL";
        public const string MODULO_MEDIO_CONTACTO = "PIKA-GD-CONTACTO-MEDIOCONTACTO";

        public static string ID_APLICAICON { get { return ConstantesAplicacion.Id; } }

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

            
            /// Modulo raiz administarción 
            //------------------------------------------------------------
            ModuloAplicacion mAdministracion = new ModuloAplicacion(
                ConstantesAplicacion.Id,
                MODULO_CONTACTO_ADMIN, true,
                "Administrador contacto",
                "Permite arministrar los recuros relacionados con el contacto",
                "",
                "es-MX",
                PermisoAplicacion.PermisosAdministrables(),
                "",
                "");
            l.Add(mAdministracion);


            /// Modulo administarcion de paises
            //------------------------------------------------------------

            m = new ModuloAplicacion(ConstantesAplicacion.Id, MODULO_PAIS , true,
                "Paises",
                "Administrador de paises",
                "",
                "es-MX",
                PermisoAplicacion.PermisosAdministrables(), MODULO_CONTACTO_ADMIN,
                ConstantesAplicacion.Id);
            m.TiposAdministrados.Add(new TipoAdministradorModulo()
            {
                AplicacionId = ConstantesAplicacion.Id,
                ModuloId = m.Id,
                TiposAdministrados = new List<Type>() { typeof(Pais) }
            });
            l.Add(m);
            //------------------------------------------------------------


            /// Modulo administarcion de estados
            //------------------------------------------------------------

            m = new ModuloAplicacion(ConstantesAplicacion.Id, MODULO_ESTADO, true,
                "Estados",
                "Administrador de estados",
                "",
                "es-MX",
                PermisoAplicacion.PermisosAdministrables(), MODULO_CONTACTO_ADMIN,
                ConstantesAplicacion.Id);
            m.TiposAdministrados.Add(new TipoAdministradorModulo()
            {
                AplicacionId = ConstantesAplicacion.Id,
                ModuloId = m.Id,
                TiposAdministrados = new List<Type>() { typeof(Estado) }
            });
            l.Add(m);
            //------------------------------------------------------------




            /// Modulo administarcion de medios de conatcto
            //------------------------------------------------------------

            m = new ModuloAplicacion(ConstantesAplicacion.Id, MODULO_TIPO_MEDIOS, true,
                "Tipos medio",
                "Administrador de medios de contacto",
                "",
                "es-MX",
                PermisoAplicacion.PermisosAdministrables(), MODULO_CONTACTO_ADMIN,
                ConstantesAplicacion.Id);
            m.TiposAdministrados.Add(new TipoAdministradorModulo()
            {
                AplicacionId = ConstantesAplicacion.Id,
                ModuloId = m.Id,
                TiposAdministrados = new List<Type>() { typeof(TipoMedio) }
            });
            l.Add(m);
            //------------------------------------------------------------



            /// Modulo administarcion de medios de fuenes de contacto
            //------------------------------------------------------------

            m = new ModuloAplicacion(ConstantesAplicacion.Id, MODULO_FUENTES, true,
                "Fuentes",
                "Administrador de fuentes de contacto",
                "",
                "es-MX",
                PermisoAplicacion.PermisosAdministrables(), MODULO_CONTACTO_ADMIN,
                ConstantesAplicacion.Id);
            m.TiposAdministrados.Add(new TipoAdministradorModulo()
            {
                AplicacionId = ConstantesAplicacion.Id,
                ModuloId = m.Id,
                TiposAdministrados = new List<Type>() { typeof(TipoFuenteContacto) }
            });
            l.Add(m);
            //------------------------------------------------------------


            /// Modulo administarcion de medios de fuenes de contacto
            //------------------------------------------------------------

            m = new ModuloAplicacion(ConstantesAplicacion.Id, MODULO_DIRECCION_POSTAL, true,
                "Direcciones",
                "Gestión de direcciones postales",
                "",
                "es-MX",
                PermisoAplicacion.PermisosAdministrables(), MODULO_CONTACTO_ADMIN,
                ConstantesAplicacion.Id, ModuloAplicacion.TipoModulo.UsuarioFinal);
            m.TiposAdministrados.Add(new TipoAdministradorModulo()
            {
                AplicacionId = ConstantesAplicacion.Id,
                ModuloId = m.Id,
                TiposAdministrados = new List<Type>() { typeof(DireccionPostal) }
            });
            l.Add(m);
            //------------------------------------------------------------


            /// Modulo administarcion de medios de fuenes de contacto
            //------------------------------------------------------------

            m = new ModuloAplicacion(ConstantesAplicacion.Id, MODULO_MEDIO_CONTACTO, true,
                "Medios contacto",
                "Gestión de medios de contacto",
                "",
                "es-MX",
                PermisoAplicacion.PermisosAdministrables(), MODULO_CONTACTO_ADMIN,
                ConstantesAplicacion.Id, ModuloAplicacion.TipoModulo.UsuarioFinal);
            m.TiposAdministrados.Add(new TipoAdministradorModulo()
            {
                AplicacionId = ConstantesAplicacion.Id,
                ModuloId = m.Id,
                TiposAdministrados = new List<Type>() { typeof(DireccionPostal) }
            });
            l.Add(m);
            //------------------------------------------------------------


            return l;
        }

    }
}
