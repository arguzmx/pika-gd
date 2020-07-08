using PIKA.Modelo.Metadatos.Atributos;
using System;
using System.Collections.Generic;
using System.Text;

namespace PIKA.Modelo.Metadatos
{
   public  class AtributoEvento
    {

        public string  PropiedadId { get; set; }
        public string Entidad { get; set; }
        public string Parametro { get; set; }
        public Operaciones Operacion { get; set; }

        public Eventos Evento { get; set; }
        
    }
}
