using PIKA.Infraestructura.Comun;
using PIKA.Infraestructura.Comun.Constantes;
using PIKA.Infraestructura.Comun.Seguridad;
using PIKA.Modelo.GestorDocumental;
using System;
using System.Collections.Generic;
using System.Text;

namespace PIKA.Servicio.GestionDocumental
{
    public class AplicacionGestionDocumental : IInformacionAplicacion
    {
        public const string MODULO_BASE_GESTION_DOCUMENTAL = "PIKA-GD-GD";
        public const string MODULO_GD_CUADRO_CLASIFICACION = "PIKA-GD-GD-CUADROS_CLASIFICACION";
        public const string MODULO_GD_ESTADO_CUADRO = "PIKA-GD-GD-ESTADOS_CUADRO";
        public const string MODULO_GD_ELEMENTO_CLASIFICACION = "PIKA-GD-GD-ELEMENTOS_CLASIFICACION";
        public const string MODULO_GD_ARCHIVO = "PIKA-GD-GD-ARCHIVOS";
        public const string MODULO_GD_TIPO_ARCHIVO = "PIKA-GD-GD-TIPOS_ARCHIVO";
        public const string MODULO_GD_FASE_CICLO_VITAL = "PIKA-GD-GD-FASE-CICLO-VITAL";
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

            string IdModuloAdminOrg = $"{MODULO_BASE_GESTION_DOCUMENTAL}-ADMIN";


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
                "");


            l.Add(mAdministracion);

            //------------------------------------------------------------
            //------------------------------------------------------------


            /// Modulo administarcion de Cuadro clasificacion
            //------------------------------------------------------------

            m = new ModuloAplicacion(ConstantesAplicacion.Id, MODULO_GD_CUADRO_CLASIFICACION, true,
                "Cuadros de clasificación",
                "Administrador de cuadros de clasificación del usuario",
                "",
                "es-MX",
                PermisoAplicacion.PermisosAdministrables(), IdModuloAdminOrg,
                ConstantesAplicacion.Id);
            m.TiposAdministrados.Add(new TipoAdministradorModulo()
            {
                AplicacionId = ConstantesAplicacion.Id,
                ModuloId = m.Id,
                TiposAdministrados = new List<Type>() { typeof(CuadroClasificacion) }
            });
            l.Add(m);
            //------------------------------------------------------------
            //------------------------------------------------------------



            /// Modulo administarcion de Elementos clasificacion
            //------------------------------------------------------------

            m = new ModuloAplicacion(ConstantesAplicacion.Id, MODULO_GD_ELEMENTO_CLASIFICACION, true,
                "Elementos clasificacion",
                "Administrador de elementos de clasificación del cuadro",
                "",
                "es-MX",
                PermisoAplicacion.PermisosAdministrables(), IdModuloAdminOrg,
                ConstantesAplicacion.Id);
            m.TiposAdministrados.Add(new TipoAdministradorModulo()
            {
                AplicacionId = ConstantesAplicacion.Id,
                ModuloId = m.Id,
                TiposAdministrados = new List<Type>() { typeof(ElementoClasificacion) }
            });
            l.Add(m);
            //------------------------------------------------------------
            //------------------------------------------------------------


            /// Modulo administarcion de Estados cuadro 
            //------------------------------------------------------------

            m = new ModuloAplicacion(ConstantesAplicacion.Id, MODULO_GD_ESTADO_CUADRO, true,
                "Estados cuadro clasificacion",
                "Administrador de estados del cuadro clasificacion ",
                "",
                "es-MX",
                PermisoAplicacion.PermisosAdministrables(), IdModuloAdminOrg,
                ConstantesAplicacion.Id);
            m.TiposAdministrados.Add(new TipoAdministradorModulo()
            {
                AplicacionId = ConstantesAplicacion.Id,
                ModuloId = m.Id,
                TiposAdministrados = new List<Type>() { typeof(EstadoCuadroClasificacion) }
            });
            l.Add(m);
            //------------------------------------------------------------
            //------------------------------------------------------------

            /// Modulo administarcion de Archivos 
            //------------------------------------------------------------

            m = new ModuloAplicacion(ConstantesAplicacion.Id, MODULO_GD_ARCHIVO, true,
                "Archivos",
                "Administrador de archivos del usuario",
                "",
                "es-MX",
                PermisoAplicacion.PermisosAdministrables(), IdModuloAdminOrg,
                ConstantesAplicacion.Id);
            m.TiposAdministrados.Add(new TipoAdministradorModulo()
            {
                AplicacionId = ConstantesAplicacion.Id,
                ModuloId = m.Id,
                TiposAdministrados = new List<Type>() { typeof(Archivo) }
            });
            l.Add(m);
            //------------------------------------------------------------
            //------------------------------------------------------------

            /// Modulo administarcion de Tipos archivo 
            //------------------------------------------------------------

            m = new ModuloAplicacion(ConstantesAplicacion.Id, MODULO_GD_TIPO_ARCHIVO, true,
                "Tipos de Archivos",
                "Administrador de tipos de archivos",
                "",
                "es-MX",
                PermisoAplicacion.PermisosAdministrables(), IdModuloAdminOrg,
                ConstantesAplicacion.Id);
            m.TiposAdministrados.Add(new TipoAdministradorModulo()
            {
                AplicacionId = ConstantesAplicacion.Id,
                ModuloId = m.Id,
                TiposAdministrados = new List<Type>() { typeof(TipoArchivo) }
            });
            l.Add(m);
            //------------------------------------------------------------
            //------------------------------------------------------------

            /// Modulo administarcion de Archivos 
            //------------------------------------------------------------

            m = new ModuloAplicacion(ConstantesAplicacion.Id, MODULO_GD_FASE_CICLO_VITAL, true,
                "Fases ciclo vital",
                "Administrador de fase ciclo vital",
                "",
                "es-MX",
                PermisoAplicacion.PermisosAdministrables(), IdModuloAdminOrg,
                ConstantesAplicacion.Id);
            m.TiposAdministrados.Add(new TipoAdministradorModulo()
            {
                AplicacionId = ConstantesAplicacion.Id,
                ModuloId = m.Id,
                TiposAdministrados = new List<Type>() { typeof(FaseCicloVital) }
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
