﻿using System;
using System.Collections.Generic;
using System.Text;

namespace PIKA.Infraestructura.Comun
{
    public class ConfiguracionServidor
    {

        public ConfiguracionServidor()
        {
            tamanocache = 1024;
            activo_desasociar_total = true;
        }


        /// <summary>
        /// Tamaño del cache en memoria de la aplciación 
        /// el tamaño se expresa en numero de elementos contenidos sin relación con el tamaño en memoria
        /// </summary>
        public int tamanocache { get; set; }

        public string jwtauth { get; set; }

        /// <summary>
        /// Ruta para el enpoint de monitoreo de salud
        /// </summary>
        public string healthendpoint { get; set; }

        public string jwtaud { get; set; }

        public string header_dominio { get; set; }
        public string header_idusuario { get; set; }
        public string header_tenantid { get; set; }

        /// <summary>
        /// Determina la duración del cacché de seguridad, el cache es delizante
        /// </summary>
        public int seguridad_cache_segundos { get; set; }

        /// <summary>
        /// Determina si los permisos efectivos son los mínimos del conjunto
        /// </summary>
        public bool seguridad_minimo_permisos { get; set; }

        /// <summary>
        /// Determina si almacena la seguridad en el cache o si debe evalaurse en cada llamada
        /// </summary>
        public bool seguridad_almacenar_cache { get; set; }

        public string jwtclient { get; set; }

        public string jwtclientsecret { get; set; }

        public string ruta_cache_fisico { get; set; }

        public string separador_ruta { get; set; }

        public string ruta_temporal { get; set; }
        
        public string ruta_tesseract { get; set; }

        public string ruta_jpegtran { get; set; }

        /// <summary>
        /// DEtermina si al remover un activo del contenedor de almacen también se remueve
        /// su asociacion al almacén y la zona o sólo del contenedor
        /// </summary>
        public bool? activo_desasociar_total { get; set; }

        /// <summary>
        /// DEermina si los catálogos deben tener el CRUD utilizando el dominio/OU
        /// </summary>
        public bool? dominio_en_catalogos { get; set; }
    }
}
