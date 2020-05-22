using Microsoft.CodeAnalysis.Operations;
using System;
using System.Collections.Generic;
using System.Text;

namespace PIKA.Modelo.Metadatos
{
    /// <summary>
    /// Mentiene una copia en memoria del valor de la propuiedad de una plantilla
    /// </summary>
    public class ValorPropiedad
    {
        /// <summary>
        /// Identificador único de la propiedad
        /// </summary>
        public string PropiedadId { get; set; }

        /// <summary>
        /// Valor en foamto de texto compatible JSON
        /// </summary>
        public string Valor { get; set; }
    }
}
