using PIKA.Modelo.Metadatos.Atributos;
using System;
using System.Collections.Generic;
using System.Text;

namespace PIKA.Modelo.Metadatos
{

 
    public class EntidadVinculada
    {
        public  TipoCardinalidad Cardinalidad { get; set; }
        public string EntidadHijo { get; set; }
        public string PropiedadPadre { get; set; }
        public string PropiedadHijo { get; set; }
    }
}
