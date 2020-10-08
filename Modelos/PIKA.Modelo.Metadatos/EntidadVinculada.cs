using PIKA.Modelo.Metadatos.Atributos;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;
using System.Xml.Serialization;

namespace PIKA.Modelo.Metadatos
{

 
    public class EntidadVinculada
    {
        public  TipoCardinalidad Cardinalidad { get; set; }
        public string EntidadHijo { get; set; }
        public string PropiedadIdMiembro{ get; set; }
        public string PropiedadPadre { get; set; }
        public string PropiedadHijo { get; set; }
        public bool HijoDinamico { get; set; }
        public string TokenSeguridad { get; set; }
        public TipoDespliegueVinculo TipoDespliegue { get; set; }
        public  List<DiccionarioEntidadVinculada> DiccionarioEntidadesVinculadas { get; set; }
    }
}
