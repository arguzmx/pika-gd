using System;
using System.Collections.Generic;
using System.Text;

namespace PIKA.Modelo.GestorDocumental
{
    public class Expediente
    {
        public Expediente() {
            Documentos = new HashSet<Documento>();
        }

        public ICollection<Documento> Documentos { get; set; }
    }
}
