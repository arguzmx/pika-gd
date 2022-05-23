using System;
using System.Collections.Generic;
using System.Text;

namespace PIKA.Infraestructura.Comun.Eventos
{
    public class EventoEntidad
    {
        /// <summary>
        /// Identificador único del evento
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Fecha UTC del evento
        /// </summary>
        public DateTime Fecha { get; set; }
        
        /// <summary>
        /// Código del evento puede ser un tipo basdo en una enumeración
        /// </summary>
        public int Codigo { get; set; }

        /// <summary>
        /// Identificador único del usuario del evento
        /// </summary>
        public string UsuarioId { get; set; }

        /// <summary>
        /// Identificador único del proceso que genera el evento
        /// </summary>
        public string ProcesoId { get; set; }


        /// <summary>
        /// Identifica si el evento ha sido genrado por el sistema 
        /// </summary>
        public bool DeSistema { get; set; }

        /// <summary>
        /// Paylod específico de evento que almacena los detalles del evento
        /// </summary>
        public string Payload { get; set; }

    }
}
