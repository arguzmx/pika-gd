using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;
using System.Xml.Serialization;
using RepositorioEntidades;

namespace PIKA.Modelo.GestorDocumental
{
    public class ValoracionEntradaClasificacion
    {
       
      
        /// <summary>
        /// Identificaor único de la entrada del clasificación
        /// </summary>
        public string EntradaClasificacionId { get; set; }

        /// <summary>
        /// Identificaor único del tipo de valoraión para la entarda
        /// </summary>
        public string TipoValoracionDocumentalId { get; set; }

        [XmlIgnore]
        [JsonIgnore]
        public EntradaClasificacion EntradaClasificacion { get; set; }

        [XmlIgnore]
        [JsonIgnore]
        public TipoValoracionDocumental TipoValoracionDocumental { get; set; }

    }
}
