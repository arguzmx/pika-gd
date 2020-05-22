using RepositorioEntidades;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using System.Text.Json.Serialization;
using System.Xml.Serialization;

namespace PIKA.Modelo.Metadatos
{
    public class ValidadorTexto: Entidad<string>
    {
        public string PropiedadId { get; set; }
        public int longmin { get; set; }
        public int longmax { get; set; }
        public string valordefault { get; set; }
        public string regexp { get; set; }
        
        /// <summary>
        /// Propieudad de navegación
        /// </summary>
        [NotMapped]
        [XmlIgnore]
        [JsonIgnore]
        public Propiedad Propiedad { get; set; }

        /// <summary>
        /// Propieudad de navegación
        /// </summary>
        [XmlIgnore]
        [JsonIgnore]
        public PropiedadPlantilla PropiedadPlantilla { get; set; }
    }
}
