using System;
using System.Collections.Generic;
using System.Text;

namespace PIKA.Modelo.Metadatos
{
    public class RequestValoresPlantilla
    {
        public RequestValoresPlantilla()
        {
            this.Valores = new List<ValorPropiedad>();
        }

        public string Filtro { get; set; }

        public List<ValorPropiedad> Valores { get; set; }

    }
}
