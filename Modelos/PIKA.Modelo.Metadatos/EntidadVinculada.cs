using PIKA.Modelo.Metadatos.Atributos;
using System;
using System.Collections.Generic;
using System.Text;

namespace PIKA.Modelo.Metadatos
{

 
    public class EntidadVinculada
    {
        public  TipoCardinalidad Cardinalidad { get; set; }
        public string Entidad { get; set; }
        public string Padre { get; set; }
        public string Hijo { get; set; }
    }
}
