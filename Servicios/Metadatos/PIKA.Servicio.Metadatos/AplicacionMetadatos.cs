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


            /// Modulo METADATOS 
            //------------------------------------------------------------
            ModuloAplicacion mAdministracion = new ModuloAplicacion(
                ConstantesAplicacion.Id,
                IdModuloAdminOrg, true,
                "Administrador Metadatos",
                "Permite arministrar Metadatos de la aplicacion",
                "",
                "es-MX",
                PermisoAplicacion.PermisosAdministrables(),
                "",
                "");


            l.Add(mAdministracion);

            //------------------------------------------------------------
            //------------------------------------------------------------


            /// plantillas
            //------------------------------------------------------------

            m = new ModuloAplicacion(ConstantesAplicacion.Id, MODULO_GD_PLANTILLA, true,
                "Plantillas",
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



            /// propiedades plantillas 
            //------------------------------------------------------------

            m = new ModuloAplicacion(ConstantesAplicacion.Id, MODULO_GD_PROPIEDAD_PLANTILLA, true,
                "Propiedades de plantillas",
                "Administrador de propiedades Plantillas",
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



            /// atributos de tabla
            //------------------------------------------------------------

            m = new ModuloAplicacion(ConstantesAplicacion.Id, MODULO_GD_ATRIBUTO_TABLA, true,
                "Atributos de tabla ",
                "Administrador atributos de tabla",
                "",
                "es-MX",
                PermisoAplicacion.PermisosAdministrables(), IdModuloAdminOrg,
                ConstantesAplicacion.Id);
            m.TiposAdministrados.Add(new TipoAdministradorModulo()
            {
                AplicacionId = ConstantesAplicacion.Id,
                ModuloId = m.Id,
                TiposAdministrados = new List<Type>() { typeof(AtributoTabla) }
            });
            l.Add(m);

            //------------------------------------------------------------
            //------------------------------------------------------------

            /// TIPOS_DATO
            //------------------------------------------------------------

            m = new ModuloAplicacion(ConstantesAplicacion.Id, MODULO_GD_TIPO_DATO, true,
                "Tipo de Dato ",
                "Administrador Tipo de datos",
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

            /// Validador de nùmero
            //------------------------------------------------------------

            m = new ModuloAplicacion(ConstantesAplicacion.Id, MODULO_GD_VALIDADOR_NUMERO, true,
                "Validador de Nùmero",
                "Administrador Validador de nùmero",
                "",
                "es-MX",
                PermisoAplicacion.PermisosAdministrables(), IdModuloAdminOrg,
                ConstantesAplicacion.Id);
            m.TiposAdministrados.Add(new TipoAdministradorModulo()
            {
                AplicacionId = ConstantesAplicacion.Id,
                ModuloId = m.Id,
                TiposAdministrados = new List<Type>() { typeof(ValidadorNumero) }
            });
            l.Add(m);
            //------------------------------------------------------------
            //------------------------------------------------------------

            /// Validador de texto
            //------------------------------------------------------------

            m = new ModuloAplicacion(ConstantesAplicacion.Id, MODULO_GD_VALIDADOR_TEXTO, true,
                "Validador de texto",
                "Administrador Validador de texto",
                "",
                "es-MX",
                PermisoAplicacion.PermisosAdministrables(), IdModuloAdminOrg,
                ConstantesAplicacion.Id);
            m.TiposAdministrados.Add(new TipoAdministradorModulo()
            {
                AplicacionId = ConstantesAplicacion.Id,
                ModuloId = m.Id,
                TiposAdministrados = new List<Type>() { typeof(ValidadorTexto) }
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
