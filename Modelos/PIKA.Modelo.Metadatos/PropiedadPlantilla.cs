using System;
using System.Collections.Generic;
using System.Text;

namespace PIKA.Modelo.Metadatos
{
    public class PropiedadPlantilla : Propiedad
    {

        public PropiedadPlantilla()
        {

        }

        /// <summary>
        /// Identificador de la plantilla a la que pertenece la propiedad
        /// </summary>
        public string PlantillaId { get; set; }


        public virtual Plantilla Plantilla { get; set; }

        public string TipoDatoProiedadPlantillaid { get; set; }

        public virtual TipoDatoPropiedadPlantilla TipoDatoPropiedad { get; set; }

        //public string atributo
    }
}
