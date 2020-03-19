using System;
using System.Collections.Generic;
using System.Text;

namespace PIKA.Modelo.Metadatos
{
    public class AtributoMetadato
    {
        public string PropiedadId { get; set; }
        public string Id { get; set; }
        public object Valor { get; set; }
        public Propiedad Propiedad { get; set; }
    }
}
