using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;
using System.Xml.Serialization;

namespace PIKA.Modelo.Metadatos
{
    public class ValorListaPlantilla: ValorLista
    {
        /// <summary>
        ///  Identificadro único de la propiead a la que pertenece el elemento
        /// </summary>
        public string PropiedadId { get; set; }
        [XmlIgnore]
        [JsonIgnore]
        public PropiedadPlantilla Propiedad { get; set; }

    }
}
