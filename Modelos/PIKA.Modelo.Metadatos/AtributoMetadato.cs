using RepositorioEntidades;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace PIKA.Modelo.Metadatos
{
    public class AtributoMetadato: Entidad<string>
    {
        public string PropiedadId { get; set; }
        [NotMapped]
        public object Valor { get; set; }
        [NotMapped]
        public Propiedad Propiedad { get; set; }

        public PropiedadPlantilla PropiedadPlantilla { get; set; }
    }
}
