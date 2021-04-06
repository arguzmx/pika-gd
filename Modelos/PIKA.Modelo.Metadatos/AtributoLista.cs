using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace PIKA.Modelo.Metadatos
{
    public class AtributoLista
    {
        public AtributoLista ()
        {
            Valores = new List<ValorLista>();
        }

        public string PropiedadId { get; set; }

        public bool OrdenarAlfabetico { get; set; }

        public string Entidad { get; set; }

        public bool DatosRemotos { get; set; }

        public bool TypeAhead { get; set; }

        public string Default { get; set; }

        public string ValoresCSV { get; set; }

        [NotMapped]
        public List<ValorLista> Valores { get; set; }
    }
}
