using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;
using System.Xml.Serialization;

namespace PIKA.Modelo.GestorDocumental
{

    /// <summary>
    /// Cuando un activo sea declinado para una trasnferencia debe indicarse utilizando esta entidad
    /// </summary>
    public class ActivoDeclinado
    {
        /// <summary>
        /// Identificador único del activo
        /// </summary>
        public string ActivoId { get; set; }

        /// <summary>
        /// Identificador único de la trasnfernecia
        /// </summary>
        public string TransferenciaId { get; set; }

        public string Motivo { get; set; }
        // Obligatoriuo 2048
        [XmlIgnore]
        [JsonIgnore]
        public Activo Activo { get; set; }
        [XmlIgnore]
        [JsonIgnore]
        public Transferencia Transferencia { get; set; }
    }
}
