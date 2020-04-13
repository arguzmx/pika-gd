using RepositorioEntidades;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace PIKA.Modelo.Metadatos
{
    public class ValidadorTexto: Entidad<string>
    {
        public string PropiedadId { get; set; }
        public int longmin { get; set; }
        public int longmax { get; set; }
        public string valordefaulr { get; set; }
        public string regexp { get; set; }
        [NotMapped]
        public Propiedad Propiedad { get; set; }
        public PropiedadPlantilla PropiedadPlantilla { get; set; }
    }
}
