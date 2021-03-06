﻿using RepositorioEntidades;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;
using System.Xml.Serialization;

namespace PIKA.Modelo.GestorDocumental
{


    /// <summary>
    /// Asunto del activo, se encuentra separado para hacer mas eficiente 
    /// las querys en la base de datos
    /// </summary>
    public class Asunto: Entidad<string>
    {
        /// <summary>
        /// Tiene una relación 1 a 1 con el activo
        /// </summary>
        public string ActivoId { get; set; }

        /// <summary>
        /// Contenio del asunto
        /// </summary>
        public string Contenido { get; set; }
        //#sin tamaño para que genere el texto con tamaño máximo


        [XmlIgnore]
        [JsonIgnore]
        public Activo Activo { get; set; }
    }
}
