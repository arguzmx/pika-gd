using System;
using System.Collections.Generic;
using System.Text;

namespace PIKA.Modelo.Metadatos
{
    public class PropiedadPlantilla: Propiedad
    {

        public PropiedadPlantilla()
        {

        }

        /// <summary>
        /// Identificador de la plantilla a la que pertenece la propiedad
        /// </summary>
        public string PlantillaId { get; set; }


        public virtual  Plantilla Plantilla { get; set; }
    }
}
