using PIKA.Infraestructura.Comun;
using PIKA.Infraestructura.Comun.Constantes;
using PIKA.Infraestructura.Comun.Seguridad;
using PIKA.Modelo.GestorDocumental;
using PIKA.Modelo.GestorDocumental.Topologia;
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
        public const string MODULO_GD_ACTIVO= "PIKA-GD-GD-ACTIVOS";
        public const string MODULO_GD_ASUNTO= "PIKA-GD-GD-ASUNTOS";
        public const string MODULO_GD_PRESTAMO= "PIKA-GD-GD-PRESTAMOS";
        public const string MODULO_GD_ACTIVO_PRESTAMO= "PIKA-GD-GD-ACTIVOS-PRESTAMO";
        public const string MODULO_GD_COMENTARIO_PRESTAMO= "PIKA-GD-GD-COMENTARIOS-PRESTAMO";
        public const string MODULO_GD_ALMACEN= "PIKA-GD-GD-ALMACEN-ARCHIVO";
        public const string MODULO_GD_ESTANTE= "PIKA-GD-GD-ESTANTE";
        public const string MODULO_GD_ESPACIO_ESTANTE= "PIKA-GD-GD-ESPACIO-ESTANTE";
        public const string MODULO_GD_HISTORIAL_ARCHIVO_ACTIVO = "PIKA-GD-GD-HISTORIAL-ARCHIVO-ACTIVO";
        public const string MODULO_GD_AMPLIACION = "PIKA-GD-GD-AMPLIACION";
        public const string MODULO_GD_TIPO_AMPLIACION= "PIKA-GD-GD-TIPO-AMPLIACION";

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

            /// Modulo administarcion de FAse Ciclo vital
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

            /// Modulo administarcion de ACtivos 
            //------------------------------------------------------------

            m = new ModuloAplicacion(ConstantesAplicacion.Id, MODULO_GD_ACTIVO, true,
                "Activos",
                "Administrador de activos del archivo",
                "",
                "es-MX",
                PermisoAplicacion.PermisosAdministrables(), IdModuloAdminOrg,
                ConstantesAplicacion.Id);
            m.TiposAdministrados.Add(new TipoAdministradorModulo()
            {
                AplicacionId = ConstantesAplicacion.Id,
                ModuloId = m.Id,
                TiposAdministrados = new List<Type>() { typeof(Activo) }
            });
            l.Add(m);
            //------------------------------------------------------------
            //------------------------------------------------------------

            /// Modulo administarcion de Asuntos de activo 
            //------------------------------------------------------------

            m = new ModuloAplicacion(ConstantesAplicacion.Id, MODULO_GD_ASUNTO, true,
                "Asuntos",
                "Administrador de asuntos de un activo",
                "",
                "es-MX",
                PermisoAplicacion.PermisosAdministrables(), IdModuloAdminOrg,
                ConstantesAplicacion.Id);
            m.TiposAdministrados.Add(new TipoAdministradorModulo()
            {
                AplicacionId = ConstantesAplicacion.Id,
                ModuloId = m.Id,
                TiposAdministrados = new List<Type>() { typeof(Asunto) }
            });
            l.Add(m);
            //------------------------------------------------------------
            //------------------------------------------------------------

            /// Modulo administarcion de Ampliacion
            //------------------------------------------------------------

            m = new ModuloAplicacion(ConstantesAplicacion.Id, MODULO_GD_AMPLIACION, true,
                "Ampliación",
                "Administrador de ampliaciones",
                "",
                "es-MX",
                PermisoAplicacion.PermisosAdministrables(), IdModuloAdminOrg,
                ConstantesAplicacion.Id);
            m.TiposAdministrados.Add(new TipoAdministradorModulo()
            {
                AplicacionId = ConstantesAplicacion.Id,
                ModuloId = m.Id,
                TiposAdministrados = new List<Type>() { typeof(Ampliacion) }
            });
            l.Add(m);
            //------------------------------------------------------------
            //------------------------------------------------------------

            /// Modulo administarcion de Comentarios préstamo
            //------------------------------------------------------------

            m = new ModuloAplicacion(ConstantesAplicacion.Id, MODULO_GD_TIPO_AMPLIACION, true,
                "Tipos de Ampliacion",
                "Administrador de tipos de ampliacion",
                "",
                "es-MX",
                PermisoAplicacion.PermisosAdministrables(), IdModuloAdminOrg,
                ConstantesAplicacion.Id);
            m.TiposAdministrados.Add(new TipoAdministradorModulo()
            {
                AplicacionId = ConstantesAplicacion.Id,
                ModuloId = m.Id,
                TiposAdministrados = new List<Type>() { typeof(TipoAmpliacion) }
            });
            l.Add(m);
            //------------------------------------------------------------
            //------------------------------------------------------------

            /// Modulo administarcion de Prestamos
            //------------------------------------------------------------

            m = new ModuloAplicacion(ConstantesAplicacion.Id, MODULO_GD_PRESTAMO, true,
                "Préstamos",
                "Administrador de Préstamos",
                "",
                "es-MX",
                PermisoAplicacion.PermisosAdministrables(), IdModuloAdminOrg,
                ConstantesAplicacion.Id);
            m.TiposAdministrados.Add(new TipoAdministradorModulo()
            {
                AplicacionId = ConstantesAplicacion.Id,
                ModuloId = m.Id,
                TiposAdministrados = new List<Type>() { typeof(Prestamo) }
            });
            l.Add(m);
            //------------------------------------------------------------
            //------------------------------------------------------------

            /// Modulo administarcion de Activos préstamo
            //------------------------------------------------------------

            m = new ModuloAplicacion(ConstantesAplicacion.Id, MODULO_GD_ACTIVO_PRESTAMO, true,
                "Activos préstamo",
                "Administrador de activos de un préstamo",
                "",
                "es-MX",
                PermisoAplicacion.PermisosAdministrables(), IdModuloAdminOrg,
                ConstantesAplicacion.Id);
            m.TiposAdministrados.Add(new TipoAdministradorModulo()
            {
                AplicacionId = ConstantesAplicacion.Id,
                ModuloId = m.Id,
                TiposAdministrados = new List<Type>() { typeof(ActivoPrestamo) }
            });
            l.Add(m);
            //------------------------------------------------------------
            //------------------------------------------------------------

            /// Modulo administarcion de Comentarios préstamo
            //------------------------------------------------------------

            m = new ModuloAplicacion(ConstantesAplicacion.Id, MODULO_GD_COMENTARIO_PRESTAMO, true,
                "Comentarios",
                "Administrador de comentarios de un préstamo",
                "",
                "es-MX",
                PermisoAplicacion.PermisosAdministrables(), IdModuloAdminOrg,
                ConstantesAplicacion.Id);
            m.TiposAdministrados.Add(new TipoAdministradorModulo()
            {
                AplicacionId = ConstantesAplicacion.Id,
                ModuloId = m.Id,
                TiposAdministrados = new List<Type>() { typeof(ComentarioPrestamo) }
            });
            l.Add(m);
            //------------------------------------------------------------
            //------------------------------------------------------------

            /// Modulo administarcion de Almacen archivo
            //------------------------------------------------------------

            m = new ModuloAplicacion(ConstantesAplicacion.Id, MODULO_GD_ALMACEN, true,
                "Almacen",
                "Administrador de almaen de un archivo",
                "",
                "es-MX",
                PermisoAplicacion.PermisosAdministrables(), IdModuloAdminOrg,
                ConstantesAplicacion.Id);
            m.TiposAdministrados.Add(new TipoAdministradorModulo()
            {
                AplicacionId = ConstantesAplicacion.Id,
                ModuloId = m.Id,
                TiposAdministrados = new List<Type>() { typeof(AlmacenArchivo) }
            });
            l.Add(m);
            //------------------------------------------------------------
            //------------------------------------------------------------

            /// Modulo administarcion de Estatnes
            //------------------------------------------------------------

            m = new ModuloAplicacion(ConstantesAplicacion.Id, MODULO_GD_ESTANTE, true,
                "Estantes",
                "Administrador de estantes de almacen",
                "",
                "es-MX",
                PermisoAplicacion.PermisosAdministrables(), IdModuloAdminOrg,
                ConstantesAplicacion.Id);
            m.TiposAdministrados.Add(new TipoAdministradorModulo()
            {
                AplicacionId = ConstantesAplicacion.Id,
                ModuloId = m.Id,
                TiposAdministrados = new List<Type>() { typeof(Estante) }
            });
            l.Add(m);
            //------------------------------------------------------------
            //------------------------------------------------------------

            /// Modulo administarcion de Comentarios préstamo
            //------------------------------------------------------------

            m = new ModuloAplicacion(ConstantesAplicacion.Id, MODULO_GD_ESPACIO_ESTANTE, true,
                "Espacios estante",
                "Administrador de espacios de un estante",
                "",
                "es-MX",
                PermisoAplicacion.PermisosAdministrables(), IdModuloAdminOrg,
                ConstantesAplicacion.Id);
            m.TiposAdministrados.Add(new TipoAdministradorModulo()
            {
                AplicacionId = ConstantesAplicacion.Id,
                ModuloId = m.Id,
                TiposAdministrados = new List<Type>() { typeof(EspacioEstante) }
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
