using System;
using System.Collections.Generic;
using System.Text;

namespace PIKA.Modelo.Metadatos
{
    public class RequestValoresPlantilla
    {
        public RequestValoresPlantilla()
        {
            this.Valores = new List<ValorPropiedad>();
        }

        /// <summary>
        /// Tipo del objeto al que se asocia la plantilla
        /// </summary>
        public string Tipo { get; set; }

        /// <summary>
        /// Identificador único de l objeto al que se asocia la plantilla
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Elemento clave de filtrado para la plantilla, por ejemplo el nombre del volumen de datos
        /// </summary>
        public string Filtro { get; set; }

        /// <summary>
        /// Lista de valores para la plantilla
        /// </summary>
        public List<ValorPropiedad> Valores { get; set; }

    }
}
