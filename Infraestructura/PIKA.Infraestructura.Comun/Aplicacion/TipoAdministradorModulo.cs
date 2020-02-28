using System;
using System.Collections.Generic;
using System.Text;

namespace PIKA.Infraestructura.Comun
{
    public class TipoAdministradorModulo
    {

        /// <summary>
        /// Unique appliction ID
        /// </summary>
        public string AplicacionId { get; set; }

        /// <summary>
        /// Identificador único del modulo de la aplicación
        /// </summary>
        public string ModuloId { get; set; }


        /// <summary>
        /// Tipos del objeto administrador por el módulo
        /// </summary>
        public List<Type> TiposAdministrado { get; set; }


    }
}
