using System;
using System.Collections.Generic;
using System.Text;

namespace RepositorioEntidades
{


    public class PropiedadesExtendidas
    {
        public List<PropiedadExtendida> Propiedades { get; set; }
        public List<ValoresEntidad> ValoresEntidad { get; set; }
    }


    public class PropiedadExtendida {
        public string PlantillaId { get; set; }
        public string Id { get; set; }
        public string Nombre { get; set; }
        public string TipoDatoId { get; set; }
    }

    public class ValoresEntidad
    {
        public string Id { get; set; }
        public List<ValorPropiedad> Valores { get; set; }
    }

    public class ValorPropiedad
    {
        public string Id { get; set; }
        public string Valor { get; set; }
    }

}
