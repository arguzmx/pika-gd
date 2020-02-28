using System;
using System.Collections.Generic;
using System.Text;

namespace PIKA.Infraestructura.Comun
{
    /// <summary>
    /// Proporciona lode detalles de un módulo perteneciente a una aplciación
    /// </summary>
    public class ModuloAplicacion
    {
       
        public ModuloAplicacion()
        {
            Modulos = new HashSet<ModuloAplicacion>();
            Traducciones = new HashSet<TraduccionAplicacionModulo>();
        }

        /// <summary>
        /// Unique appliction ID
        /// </summary>
        public string AplicacionId { get; set; }

        /// <summary>
        /// Identificador único del modulo de la aplicación
        /// </summary>
        public string ModuloId { get; set; }



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
            string Nombre, string Descripcion, string Icon, string UICulture, ulong PermisosDisponibles,
            string ModuloPadreId, string AplicacionPadreId)
        {
            this.AplicacionPadreId = AplicacionPadreId;
            this.ModuloPadreId = ModuloPadreId;
            this.PermisosDisponibles = PermisosDisponibles;
            this.UICulture = UICulture;
            this.Icono = Icon;
            this.Descripcion = Descripcion;
            this.Nombre = Nombre;
            this.Asegurable = Asegurable;
            this.ModuloId = ModuloId;
            this.AplicacionId = AplicacionId;
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
        public ulong PermisosDisponibles { get; set; }

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


        public ICollection<TipoAdministradorModulo> TiposAdministrados { get; set; }

    }
}
