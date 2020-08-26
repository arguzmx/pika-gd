using System;
using System.Collections.Generic;
using System.Text;

namespace PIKA.Modelo.Metadatos
{
    public class ParametroReporte
    {
        public string Nombre { get; set; }
        public string Id { get; set; }
        public string Tipo { get; set; }
        public bool Contextual { get; set; }
        public string IdContextual { get; set; }

    }
}
