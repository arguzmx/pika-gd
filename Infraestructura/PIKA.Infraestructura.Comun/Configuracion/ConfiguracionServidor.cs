using System;
using System.Collections.Generic;
using System.Text;

namespace PIKA.Infraestructura.Comun
{
    public class ConfiguracionServidor
    {

        public ConfiguracionServidor()
        {
            tamanocache = 1024;
        }


        /// <summary>
        /// Tamaño del cache en memoria de la aplciación 
        /// el tamaño se expresa en numero de elementos contenidos sin relación con el tamaño en memoria
        /// </summary>
        public int tamanocache { get; set; }

        public string jwtauth { get; set; }
        public string jwtaud { get; set; }

        public string header_dominio { get; set; }
        public string header_idusuario { get; set; }
        public string header_tenantid { get; set; }

        public int cache_seguridad_segundos { get; set; }

        public bool alamcenar_cache_seguridad { get; set; }

        public string jwtclient { get; set; }

        public string jwtclientsecret { get; set; }

    }
}
