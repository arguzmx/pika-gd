using RepositorioEntidades;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using System.Text.Json.Serialization;
using System.Xml.Serialization;

namespace PIKA.Modelo.Metadatos
{
    public class AtributoTabla: Entidad<string>
    {
        public string PropiedadId { get; set; }
        public bool Incluir { get; set; }
        public bool Visible { get; set; }
        public bool Alternable { get; set; }
        public int IndiceOrdebnamiento { get; set; }
        public string IdTablaCliente { get; set; }
        

        /// <summary>
        /// Propedad de navegación
        /// </summary>
        [NotMapped]
        [XmlIgnore]
        [JsonIgnore]
        public Propiedad Propiedad { get; set; }

        /// <summary>
        /// Propedad de navegación
        /// </summary>
        [XmlIgnore]
        [JsonIgnore]
        public PropiedadPlantilla PropiedadPlantilla { get; set; }
    }
}
