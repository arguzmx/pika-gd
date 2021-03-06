﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;
using System.Xml.Serialization;

namespace PIKA.Modelo.GestorDocumental
{

    /// <summary>
    /// Mantien las relaciómn da activos incluidos en una transferenci
    /// </summary>
    public class ActivoTransferencia
    {
        /// <summary>
        /// Identificador único del activo
        /// </summary>
        public string ActivoId { get; set; }

        /// <summary>
        /// Identificador único de la trasnfernecia
        /// </summary>
        public string TransferenciaId { get; set; }
        [XmlIgnore]
        [JsonIgnore]
        public Activo Activo { get; set; }
        [XmlIgnore]
        [JsonIgnore]
        public Transferencia Transferencia { get; set; }
    }
}
