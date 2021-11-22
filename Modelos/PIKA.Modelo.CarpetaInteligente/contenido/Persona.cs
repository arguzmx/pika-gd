using System;
using System.Collections.Generic;
using System.Text;

namespace PIKA.Modelo.CarpetaInteligente
{

    /// <summary>
    /// PErmite determinar quien es el responsable de ejecutar una tarea manual
    /// </summary>
    public class Persona
    {
        /// <summary>
        /// Identificador único del grupo o usuario de acceso
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Tipo de entida de acceso
        /// </summary>
        public TipoEntidadAcceso Tipo { get; set; }
    }
}
