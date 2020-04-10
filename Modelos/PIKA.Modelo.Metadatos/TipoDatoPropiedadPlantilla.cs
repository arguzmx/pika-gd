using System;
using System.Collections.Generic;
using System.Text;

namespace PIKA.Modelo.Metadatos
{

    /// <summary>
    /// MAntiene la relación de una propiedad de la plantilla con su tipo de datos
    /// </summary>
    public class TipoDatoPropiedadPlantilla
    {

        public string TipoDatoId { get; set; }

        public string PropiedadPlantillaId { get; set; }

        public virtual TipoDato Tipo { get; set; }
        public virtual PropiedadPlantilla Propiedad { get; set; }

    }
}
