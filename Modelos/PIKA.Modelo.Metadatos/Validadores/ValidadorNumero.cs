﻿using RepositorioEntidades;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using System.Text.Json.Serialization;
using System.Xml.Serialization;

namespace PIKA.Modelo.Metadatos
{
    public class ValidadorNumero: Entidad<string>
    {
        public string PropiedadId { get; set; }
        public float min { get; set; }
        public float max { get; set; }
        public float valordefault { get; set; }

        public bool UtilizarMax { get; set; }

        public bool UtilizarMin { get; set; }

        /// <summary>
        /// Propeidad de navegación
        /// </summary>
        [NotMapped]
        [XmlIgnore]
        [JsonIgnore]
        public Propiedad Propiedad { get; set; }

        /// <summary>
        /// Propeidad de navegación
        /// </summary>
        [NotMapped]
        [XmlIgnore]
        [JsonIgnore]
        public PropiedadPlantilla PropiedadPlantilla { get; set; }
    }
}
