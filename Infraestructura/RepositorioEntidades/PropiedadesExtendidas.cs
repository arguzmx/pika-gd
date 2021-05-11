using System;
using System.Collections.Generic;
using System.Text;

namespace RepositorioEntidades
{


    public class PropiedadesExtendidas
    {
        public PropiedadesExtendidas()
        {
            Propiedades = new List<PropiedadExtendida>();
            ValoresEntidad = new List<ValoresEntidad>();
        }

        /// <summary>
        /// Propiedaes incluidas en el paginado
        /// </summary>
        public List<PropiedadExtendida> Propiedades { get; set; }
        
        /// <summary>
        /// Lista de los valore aspciados en base al Id de la entidad devuelta
        /// </summary>
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
        public ValoresEntidad()
        {
            Valores = new List<string>();
        }

        public string Id { get; set; }

        /// <summary>
        /// Lista de los valores en el mismo orden que la definición de propiedades
        /// </summary>
        public List<string> Valores { get; set; }
    }


}
