using PIKA.Infraestructura.Comun;
using PIKA.Infraestructura.Comun.Constantes;
using PIKA.Infraestructura.Comun.Seguridad;
using PIKA.Modelo.Organizacion;
using System;
using System.Collections.Generic;
using System.Text;

namespace PIKA.Servicio.Organizacion
{
    public class AplicacionOrganizacion : IInformacionAplicacion
    {
        public const  string MODULO_BASE_ORGANIZACION = "PIKA-GD-ORG";
        public const string MODULO_ORGANIZACION_UNIDADES_ORGANIZACIONALES = "PIKA-GD-ORG-UOS";
        public const string MODULO_ORGANIZACION_DOMINIOS = "PIKA-GD-ORG-DOMINIO";
        public const string MODULO_ORGANIZACION_ROLES = "PIKA-GD-ORG-ROLES";
        public const string MODULO_ORGANIZACION_DIRECCIONES_POSTALES = "PIKA-GD-ORG-DIRPOSTALES";

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
            
            string IdModuloAdminOrg = $"{MODULO_BASE_ORGANIZACION}-ADMIN";


            /// Modulo administarción 
            //------------------------------------------------------------
            ModuloAplicacion mAdministracion = new ModuloAplicacion(
                ConstantesAplicacion.Id, 
                IdModuloAdminOrg, true,
                "Administrador Organización",
                "Permite arministrar los recuros asociados a la organización",
                "",
                "es-MX",
                PermisoAplicacion.PermisosAdministrables(),
                "",
                "" );


            l.Add(mAdministracion);

            //------------------------------------------------------------
            //------------------------------------------------------------


            /// Modulo administarcion de Dominios
            //------------------------------------------------------------

            m = new ModuloAplicacion(ConstantesAplicacion.Id, MODULO_ORGANIZACION_DOMINIOS, true,
                "Dominios",
                "Administrador de dominios del usuario",
                "",
                "es-MX",
                PermisoAplicacion.PermisosAdministrables(), IdModuloAdminOrg,
                ConstantesAplicacion.Id);
            m.TiposAdministrados.Add( new TipoAdministradorModulo() { AplicacionId= ConstantesAplicacion.Id, 
                ModuloId = m.Id,  
                TiposAdministrados= new List<Type>() { typeof(Dominio) } });
            l.Add(m);
            //------------------------------------------------------------
            //------------------------------------------------------------



            /// Modulo administarcion de Unidades Organizacionales 
            //------------------------------------------------------------

            m = new ModuloAplicacion(ConstantesAplicacion.Id, MODULO_ORGANIZACION_UNIDADES_ORGANIZACIONALES, true, 
                "Unidad Organizacional", 
                "Administrador de unidades organizacionales del dominio",
                "",
                "es-MX", 
                PermisoAplicacion.PermisosAdministrables(), IdModuloAdminOrg,
                ConstantesAplicacion.Id);
            m.TiposAdministrados.Add(new TipoAdministradorModulo()
            {
                AplicacionId = ConstantesAplicacion.Id,
                ModuloId = m.Id,
                TiposAdministrados = new List<Type>() { typeof(UnidadOrganizacional) }
            });
            l.Add(m);
            //------------------------------------------------------------
            //------------------------------------------------------------


            /// Modulo administarcion de Roles 
            //------------------------------------------------------------

            m = new ModuloAplicacion(ConstantesAplicacion.Id, MODULO_ORGANIZACION_ROLES, true,
                "Roles",
                "Administrador de roles del dominio",
                "",
                "es-MX",
                PermisoAplicacion.PermisosAdministrables(), IdModuloAdminOrg,
                ConstantesAplicacion.Id);
            m.TiposAdministrados.Add(new TipoAdministradorModulo()
            {
                AplicacionId = ConstantesAplicacion.Id,
                ModuloId = m.Id,
                TiposAdministrados = new List<Type>() { typeof(Rol) }
            });
            l.Add(m);
            //------------------------------------------------------------
            //------------------------------------------------------------

            /// Modulo administarcion de Roles 
            //------------------------------------------------------------

            m = new ModuloAplicacion(ConstantesAplicacion.Id, MODULO_ORGANIZACION_ROLES, true,
                "Usuariosn en rol",
                "Administrador de los usuarios perteneicnetes a un rol",
                "",
                "es-MX",
                PermisoAplicacion.PermisosAdministrables(), IdModuloAdminOrg,
                ConstantesAplicacion.Id);
            m.TiposAdministrados.Add(new TipoAdministradorModulo()
            {
                AplicacionId = ConstantesAplicacion.Id,
                ModuloId = m.Id,
                TiposAdministrados = new List<Type>() { typeof(UsuariosRol) }
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

            foreach(var m in a.Modulos)
            {

                foreach(var t in m.TiposAdministrados)
                {
                    tipos.Add(t);
                }
            }

            return tipos;
        }
    }
}
