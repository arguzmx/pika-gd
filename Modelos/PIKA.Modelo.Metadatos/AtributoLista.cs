using System;
using System.Collections.Generic;
using System.Text;

namespace PIKA.Modelo.Metadatos
{
    public class AtributoLista
    {

        public string PropiedadId { get; set; }

        public bool OrdenarAlfabetico { get; set; }

        public string Entidad { get; set; }

        public bool DatosRemotos { get; set; }

        public bool TypeAhead { get; set; }

        public string Default { get; set; }

        public string ValoresCSV { get; set; }
    }
}
