using Microsoft.Extensions.Logging;
using RabbitMQ.Client.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PIKA.GD.API.Servicios
{

    /// <summary>
    /// Alamcena lso valores e estao relevantes para la aplicación
    /// </summary>
    public class CacheAplicacion : ICacheAplicacion
    {
        public CacheAplicacion(ICacheMetadatosAplicacion metadatos)
        {
            Metadatos = metadatos;
        }
 
        /// <summary>
        /// Contiene los valores asociados con plantillas y metadatos
        /// </summary>
        public ICacheMetadatosAplicacion Metadatos { get; set; }
    }
}
