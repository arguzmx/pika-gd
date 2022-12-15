using System;
using System.Collections.Generic;
using System.Text;

namespace PIKA.Infraestructura.Comun.Seguridad.Auditoria
{
    public class TipoEventoAuditoria
    {

        /// <summary>
        /// Identificador único de la fuente del evento por ejemplo un servicio o controlador
        /// </summary>
        public string AppId { get; set; }

        /// <summary>
        /// Identificador único del módulo que origina el evento en la fuente
        /// </summary>
        public string ModuloId { get; set; }

        /// <summary>
        /// Identificador único del tipo de evento de acuerdo al orígen, 
        /// es un catálogo que publica la fuente, puede baserse en un ENUM al interior del servicio
        /// </summary>
        public int TipoEvento { get; set; }

        /// <summary>
        /// Descripción del evento
        /// </summary>
        public string Desripcion { get; set; }

        /// <summary>
        /// Plantilla default del evento para la producción de lectura humana, 
        /// se llena combinando un objeto json dinámico en el formato {{propiedad}}
        /// </summary>
        public string PlantillaEvento { get; set; }

    }
}
