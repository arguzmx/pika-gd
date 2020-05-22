using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace PIKA.Modelo.Metadatos
{

    /// <summary>
    /// Define las una propiedad asocida a una plantilla del repositoprio
    /// </summary>
    public class PropiedadPlantilla : Propiedad
    {

        public PropiedadPlantilla()
        {

        }

        /// <summary>
        /// Identificador de la plantilla a la que pertenece la propiedad
        /// </summary>
        public string PlantillaId { get; set; }


        /// <summary>
        /// Plantilla a la que pertenece la propiedad
        /// </summary>
        /// 
        [XmlIgnore]
        [JsonIgnore]
        public Plantilla Plantilla { get; set; }

       
        

    }
}
