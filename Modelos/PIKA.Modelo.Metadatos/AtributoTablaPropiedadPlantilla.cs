using System;
using System.Collections.Generic;
using System.Text;

namespace PIKA.Modelo.Metadatos
{
    public class AtributoTablaPropiedadPlantilla
    {
        public string PropiedadPlantillaid { get; set; }
        public PropiedadPlantilla PropiedadPlantilla { get; set; }

        public string Atributotablaid { get; set; }

        public AtributoTabla AtributoTabla { get; set; }

    }
}
