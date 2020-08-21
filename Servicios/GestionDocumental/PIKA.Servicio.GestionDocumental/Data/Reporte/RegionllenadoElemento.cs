using System;
using System.Collections.Generic;
using System.Text;

namespace PIKA.Servicio.GestionDocumental.Data
{
    public class RegionElementoClasificacion
    {
        /// <summary>
        /// Identificador ùnico del Elemento.
        /// </summary>
        public string ElementoId { get; set; }
        /// <summary>
        /// Identificador ùnico del Cuadro Clasificaciòn
        /// </summary>
        public string CuadroClasificacionId { get; set; }
        /// <summary>
        /// Nivel del Elemento,esto representa la posicion de la columna que le correspode en el excel.
        /// </summary>
        public int Nivel { get; set; }
        /// <summary>
        /// Si el elemento esta eliminado agrega un 1 de lo contrario un 0
        /// </summary>
        public int Eliminado { get; set; }
        /// <summary>
        /// Esta propiedad representa la uniòn entre Nombre y clave del Elemento
        /// </summary>
        public string NombreClave { get; set; }

    }

}
