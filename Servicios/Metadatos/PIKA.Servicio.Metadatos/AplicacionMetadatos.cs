using PIKA.Infraestructura.Comun;
using PIKA.Infraestructura.Comun.Constantes;
using PIKA.Infraestructura.Comun.Seguridad;
using PIKA.Modelo.Metadatos;
using System;
using System.Collections.Generic;
using System.Text;

namespace PIKA.Servicio.Metadatos
{
   public class AplicacionMetadatos : IInformacionAplicacion
    {
                  
        public const string MODULO_BASE_METADATOS = "PIKA-GD-MET";
        public const string MODULO_GD_PLANTILLA = "PIKA-GD-MET-PLANTILLA";
        public const string MODULO_GD_PROPIEDAD_PLANTILLA = "PIKA-GD-MET-PROPIEDAD_PLANTILLA";
        public const string MODULO_GD_ATRIBUTO_METADATO = "PIKA-GD-MET-ATRIBUTO_METADATO";
        public const string MODULO_GD_ATRIBUTO_TABLA = "PIKA-GD-MET-ATRIBUTO_TABLA";
        public const string MODULO_GD_TIPO_DATO = "PIKA-GD-MET-TIPOS_DATO";
        public const string MODULO_GD_VALIDADOR_NUMERO = "PIKA-GD-MET-VALIDADOR_NUMERO";
        public const string MODULO_GD_VALIDADOR_TEXTO = "PIKA-GD-MET-VALIDADOR_TEXTO";
        public const string MODULO_GD_ALMACEN_DATOS = "PIKA-GD-MET-ALMACEN_DATOS";
        public const string MODULO_GD_TIPO_ALMACEN_DATOS = "PIKA-GD-MET-TIPO_ALAMACEN_DATOS";
        public const string MODULO_GD_ASOCIACION_PLANTILLA = "PIKA-GD-MET-ASOCIACION_PLANTILLA";
        public const string MODULO_GD_VALOR_LISTA_PLANTILLA = "PIKA-GD-MET-VALOR_LISTA_PLANTILLA";

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

            string IdModuloAdminOrg = $"{MODULO_BASE_METADATOS}-ADMIN";


            /// Modulo administarción 
            //------------------------------------------------------------
            ModuloAplicacion mAdministracion = new ModuloAplicacion(
                ConstantesAplicacion.Id,
                IdModuloAdminOrg, true,
                "Administrador Metadatos",
                "Permite arministrar los recuros asociados a los metadatos",
                "",
                "es-MX",
                PermisoAplicacion.PermisosAdministrables(),
                "",
                "");


            l.Add(mAdministracion);

            //------------------------------------------------------------
            //------------------------------------------------------------


            /// Modulo administarcion de plantillas
            //------------------------------------------------------------

            m = new ModuloAplicacion(ConstantesAplicacion.Id, MODULO_GD_PLANTILLA, true,
                "Pantillas",
                "Administrador de plantillas",
                "",
                "es-MX",
                PermisoAplicacion.PermisosAdministrables(), IdModuloAdminOrg,
                ConstantesAplicacion.Id);
            m.TiposAdministrados.Add(new TipoAdministradorModulo()
            {
                AplicacionId = ConstantesAplicacion.Id,
                ModuloId = m.Id,
                TiposAdministrados = new List<Type>() { typeof(Plantilla) }
            });
            l.Add(m);
            //------------------------------------------------------------
            //------------------------------------------------------------



            /// Modulo administarcion de propiedades plantilla
            //------------------------------------------------------------

            m = new ModuloAplicacion(ConstantesAplicacion.Id, MODULO_GD_PROPIEDAD_PLANTILLA, true,
                "Propiedades Plantillas",
                "Administrador de Propiedades de las plantillas",
                "",
                "es-MX",
                PermisoAplicacion.PermisosAdministrables(), IdModuloAdminOrg,
                ConstantesAplicacion.Id);
            m.TiposAdministrados.Add(new TipoAdministradorModulo()
            {
                AplicacionId = ConstantesAplicacion.Id,
                ModuloId = m.Id,
                TiposAdministrados = new List<Type>() { typeof(PropiedadPlantilla) }
            });
            l.Add(m);
            //------------------------------------------------------------
            //------------------------------------------------------------


            /// Modulo administarcion asociaciones plantilla
            //------------------------------------------------------------

            m = new ModuloAplicacion(ConstantesAplicacion.Id, MODULO_GD_ASOCIACION_PLANTILLA, true,
                "Asociados Plantillas",
                "Administrador de asociaciones plantillas",
                "",
                "es-MX",
                PermisoAplicacion.PermisosAdministrables(), IdModuloAdminOrg,
                ConstantesAplicacion.Id);
            m.TiposAdministrados.Add(new TipoAdministradorModulo()
            {
                AplicacionId = ConstantesAplicacion.Id,
                ModuloId = m.Id,
                TiposAdministrados = new List<Type>() { typeof(AsociacionPlantilla) }
            });
            l.Add(m);

            //------------------------------------------------------------
            //------------------------------------------------------------

            /// Modulo administarcion de Archivos 
            //------------------------------------------------------------

            m = new ModuloAplicacion(ConstantesAplicacion.Id, MODULO_GD_ALMACEN_DATOS, true,
                "Almacen de datos",
                "Administrador de Almacen de Datos",
                "",
                "es-MX",
                PermisoAplicacion.PermisosAdministrables(), IdModuloAdminOrg,
                ConstantesAplicacion.Id);
            m.TiposAdministrados.Add(new TipoAdministradorModulo()
            {
                AplicacionId = ConstantesAplicacion.Id,
                ModuloId = m.Id,
                TiposAdministrados = new List<Type>() { typeof(AlmacenDatos) }
            });
            l.Add(m);
            //------------------------------------------------------------
            //------------------------------------------------------------

            /// Modulo administarcion de Tipos de almacenes metadatos
            //------------------------------------------------------------

            m = new ModuloAplicacion(ConstantesAplicacion.Id, MODULO_GD_TIPO_ALMACEN_DATOS, true,
                "Tipos de de almacenes de datos",
                "Administrador de tipos de alamacenes de datos",
                "",
                "es-MX",
                PermisoAplicacion.PermisosAdministrables(), IdModuloAdminOrg,
                ConstantesAplicacion.Id);
            m.TiposAdministrados.Add(new TipoAdministradorModulo()
            {
                AplicacionId = ConstantesAplicacion.Id,
                ModuloId = m.Id,
                TiposAdministrados = new List<Type>() { typeof(TipoAlmacenMetadatos) }
            });
            l.Add(m);
            //------------------------------------------------------------
            //------------------------------------------------------------

            /// Modulo administarcion de tipod de datos
            //------------------------------------------------------------

            m = new ModuloAplicacion(ConstantesAplicacion.Id, MODULO_GD_TIPO_DATO, true,
                "Tipos de datos",
                "Administrador de tipos de datos",
                "",
                "es-MX",
                PermisoAplicacion.PermisosAdministrables(), IdModuloAdminOrg,
                ConstantesAplicacion.Id);
            m.TiposAdministrados.Add(new TipoAdministradorModulo()
            {
                AplicacionId = ConstantesAplicacion.Id,
                ModuloId = m.Id,
                TiposAdministrados = new List<Type>() { typeof(TipoDato) }
            });
            l.Add(m);


            
        
            //------------------------------------------------------------
            //------------------------------------------------------------

            /// Modulo administarcion de validador lista
            //------------------------------------------------------------

            m = new ModuloAplicacion(ConstantesAplicacion.Id, MODULO_GD_VALOR_LISTA_PLANTILLA, true,
                "Valor lista plantilla",
                "Administrador de valor lista plantilla",
                "",
                "es-MX",
                PermisoAplicacion.PermisosAdministrables(), IdModuloAdminOrg,
                ConstantesAplicacion.Id);
            m.TiposAdministrados.Add(new TipoAdministradorModulo()
            {
                AplicacionId = ConstantesAplicacion.Id,
                ModuloId = m.Id,
                TiposAdministrados = new List<Type>() { typeof(ValorListaPlantilla) }
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
