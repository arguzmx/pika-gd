using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;
using System.Xml.Serialization;

namespace PIKA.Modelo.Contenido
{
    public class VolumenPuntoMontaje
    {
        /// <summary>
        /// Identificador único del volumen asociado al punto de montaje
        /// </summary>
        public string VolumenId { get; set; }


        /// <summary>
        /// Identificador único del punto de montaje
        /// </summary>
        public string PuntoMontajeId { get; set; }


        [XmlIgnore]
        [JsonIgnore]
        public PuntoMontaje PuntoMontaje { get; set; }

        public Volumen Volumen { get; set; }

    }
}
