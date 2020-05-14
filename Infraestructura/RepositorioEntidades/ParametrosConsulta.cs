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
        /// Número consecytivo paradar seguimiento en llamdas asínctonas
        /// </summary>
        public int consecutivo { get; set; }

        /// <summary>
        /// Nombre de la columna de ordenamoento
        /// </summary>
        public string ord_columna { get; set; }
        public string ord_direccion { get; set; }

        /// <summary>
        /// Especifica si los totales de paginado deben recalcularse
        /// </summary>
        public bool recalcular_totales { get; set; }

    }


}
