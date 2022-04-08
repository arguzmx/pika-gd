using System;
using System.Collections.Generic;
using System.Text;

namespace PIKA.Modelo.Contenido
{
    public class EstadoOCR
    {
        public long Completo { get; set; }
        public long Pendiente { get; set; }
        public long Error { get; set; }
    }
}
