using System;
using System.Collections.Generic;
using System.Text;

namespace PIKA.Infraestructura.Comun
{
    public class TraduccionAplicacionModulo
    {   /// <summary>
        /// Unique appliction ID
        /// </summary>
        public string AplicacionId { get; set; }

        /// <summary>
        /// Identificador único del modulo de la aplicación
        /// </summary>
        public string ModuloId { get; set; }

        /// <summary>
        /// Identificador del idioma de la entrada
        /// </summary>
        public string UICulture { get; set; }

        /// <summary>
        /// Nombre del módulo
        /// </summary>
        public string Nombre { get; set; }

        /// <summary>
        /// descripción del módulo
        /// </summary>
        public string Descripcion { get; set; }
    }
}
