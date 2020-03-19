using System;
using System.Collections.Generic;
using System.Text;

namespace RepositorioEntidades
{
    public class ParametrosConsulta
    {
        /// <summary>
        /// Indíce o página solicitada
        /// </summary>
        public int indice { get; set; }

        /// <summary>
        /// Tamaño de la página solicitada
        /// </summary>
        public int tamano { get; set; }

        /// <summary>
        /// Nombre de la columna de ordenamoento
        /// </summary>
        public string ord_columna { get; set; }
        public string ord_direccion { get; set; }
    }


}
