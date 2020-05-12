﻿using RepositorioEntidades;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace PIKA.Modelo.Metadatos
{
    public class ValidadorNumero: Entidad<string>
    {
        public string PropiedadId { get; set; }
        public float min { get; set; }
        public float max { get; set; }
        public float valordefault { get; set; }


        /// <summary>
        /// Propeidad de navegación
        /// </summary>
        [NotMapped]
        public Propiedad Propiedad { get; set; }

        /// <summary>
        /// Propeidad de navegación
        /// </summary>
        [NotMapped]
        public PropiedadPlantilla PropiedadPlantilla { get; set; }
    }
}
