using System;
using System.Collections.Generic;
using System.Text;

namespace PIKA.Modelo.Metadatos
{
    public class ValidadorNumero
    {
        public string PropiedadId { get; set; }
        public float min { get; set; }
        public float max { get; set; }
        public float valordefault { get; set; }
        public Propiedad Propiedad { get; set; }
    }
}
