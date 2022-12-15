using System;
using System.Collections.Generic;
using System.Text;

namespace PIKA.Infraestructura.Comun.Seguridad
{
    /// <summary>
    /// MAntiene una lista de los evento auditables por dominio
    /// </summary>
    public class EventoAuditoriaActivo
    {

        /// <summary>
        /// Identificador único de la entarda para el CRUD
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Dominio en el que ocurre el evento
        /// </summary>
        public string DominioId { get; set; }

        /// <summary>
        /// Unidad organizacional en el que se registra el evento
        /// </summary>
        public string UAId { get; set; }


        /// <summary>
        /// Identificador único de la aplicaicón que origina el evento
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
        /// Identifica si el evento se encuentra activo para ser auditado
        /// </summary>
        public bool Auditar { get; set; }
    }
}
