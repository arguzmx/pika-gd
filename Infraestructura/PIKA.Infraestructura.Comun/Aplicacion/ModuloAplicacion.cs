using PIKA.Infraestructura.Comun.Seguridad.Auditoria;
using RepositorioEntidades;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using System.Xml.Serialization;

namespace PIKA.Infraestructura.Comun
{
    /// <summary>
    /// Proporciona lode detalles de un módulo perteneciente a una aplciación
    /// </summary>
    public class ModuloAplicacion : Entidad<string>, IEntidadNombrada
    {

        public enum TipoModulo { 
            Administracion=0, UsuarioFinal=1
        }

        public ModuloAplicacion()
        {
            Modulos = new HashSet<ModuloAplicacion>();
            Traducciones = new HashSet<TraduccionAplicacionModulo>();
        }

        /// <summary>
        /// Tipo de módulo
        /// </summary>
        public TipoModulo Tipo { get; set; }


        /// <summary>
        /// Unique appliction ID
        /// </summary>
        public string AplicacionId { get; set; }


        /// <summary>
        /// Indica si el módulo recibe configuración de seguridad de acceso
        /// </summary>
        public bool Asegurable { get; set; }

        /// <summary>
        /// Nombre del módulo
        /// </summary>
        public string Nombre { get; set; }

        /// <summary>
        /// descripción del módulo
        /// </summary>
        public string Descripcion { get; set; }

        public ModuloAplicacion(string AplicacionId, string ModuloId, bool Asegurable,
            string Nombre, string Descripcion, string Icon, string UICulture, int PermisosDisponibles,
            string ModuloPadreId, string AplicacionPadreId, TipoModulo Tipo = TipoModulo.Administracion)
        {
            this.AplicacionPadreId = AplicacionPadreId;
            this.ModuloPadreId = ModuloPadreId;
            this.PermisosDisponibles = PermisosDisponibles;
            this.UICulture = UICulture;
            this.Icono = Icon;
            this.Descripcion = Descripcion;
            this.Nombre = Nombre;
            this.Asegurable = Asegurable;
            this.Id = ModuloId;
            this.AplicacionId = AplicacionId;
            this.Tipo = Tipo;
            Modulos = new HashSet<ModuloAplicacion>();
            Traducciones = new HashSet<TraduccionAplicacionModulo>();
            TiposAdministrados = new HashSet<TipoAdministradorModulo>();
        }

        /// <summary>
        /// Identificador únio del módulo padre
        /// </summary>
        public string ModuloPadreId { get; set; }

        /// <summary>
        /// Identificador únio del módulo padre, 
        /// Este campo debe mantenerse para permitir crear las claves primarias en un RDBMS
        /// </summary>
        public string AplicacionPadreId { get; set; }


        /// <summary>
        /// Icono del módulo
        /// </summary>
        public string Icono { get; set; }

        /// <summary>
        /// Identificador del idioma de la entrada
        /// </summary>
        public string UICulture { get; set; }


        /// <summary>
        /// Permisos configurables para el módulo
        /// </summary>
        public int PermisosDisponibles { get; set; }

        /// <summary>
        /// Modulos descendientes
        /// </summary>
        public ICollection<ModuloAplicacion> Modulos { get; set; }


        public ModuloAplicacion ModuloPadre { get; set; }

        public Aplicacion Aplicacion { get; set; }


        /// <summary>
        /// Modulos descendientes
        /// </summary>
        public ICollection<TraduccionAplicacionModulo> Traducciones { get; set; }

        [XmlIgnore]
        [JsonIgnore]
        public ICollection<TipoAdministradorModulo> TiposAdministrados { get; set; }

        [NotMapped]
        public ICollection<TipoEventoAuditoria> EventosAuditables { get; set; }

    }
}
