using System;
using System.Collections.Generic;
using System.Text;

namespace PIKA.Servicio.Contenido.ElasticSearch
{
    public class HighlightHit
    {
        public HighlightHit()
        {
            Highlights = new List<Highlight>();
        }
        
        public string ElasticId { get; set; }
        public List<Highlight> Highlights { get; set; }
        public string  ElementoId { get; set; }

    }

    public class Highlight {
        public string Texto { get; set; }
        public string ParteId { get; set; }
        public int Pagina { get; set; }
    }

}
