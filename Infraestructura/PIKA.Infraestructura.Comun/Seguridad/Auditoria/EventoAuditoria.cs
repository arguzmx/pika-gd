using System;
using System.Collections.Generic;
using System.Text;

namespace PIKA.Infraestructura.Comun.Seguridad
{
    public class EventoAuditoria
    {
        /// <summary>
        /// Identificador único del evento
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Fecha del evento
        /// </summary>
        public DateTime Fecha { get; set; }

        /// <summary>
        /// Dirección de red desde la que se registró la llamada que ocasiona el evento
        /// </summary>
        public string DireccionRed { get; set; }

        /// <summary>
        /// Identificador único de la sesión que provocó el evento, en el caso de accesos simultáneos
        /// </summary>
        public string IdSesion { get; set; }

        /// <summary>
        /// Usuario creador del evento, nulo para eventos del sistema
        /// </summary>
        public string UsuarioId { get; set; }

        /// <summary>
        /// Dominio en el que ocurre el evento
        /// </summary>
        public string DominioId { get; set; }
        
        /// <summary>
        /// Unidad organizacional en el que se registra el evento
        /// </summary>
        public string UAId { get; set; }

        /// <summary>
        /// Indica si la ejecución  del evento fue exitosa
        /// </summary>
        public bool Exitoso { get; set; }


        /// <summary>
        /// Indica si el evento es un error de la aplicación
        /// </summary>
        public bool EsError { get; set; }

        /// <summary>
        /// Identificador único de la fuente del evanto por ejemplo un servicio o controlado
        /// </summary>
        public string FuenteEventoId { get; set; }


        /// <summary>
        /// Identificador único del módulo que origina el evento en la fuente
        /// </summary>
        public string ModuloId { get; set; }

        /// <summary>
        /// Identificador único del tipo de evento de acuerdo al orígen, 
        /// es un catálogo que publica la fuente
        /// </summary>
        public int TipoEvento { get; set; }

        /// <summary>
        /// Texto asociado al evento, es colocado por la fuente cuando los datos deben ampliarse, 
        /// normalmente es nulo
        /// </summary>
        public string Texto { get; set; }
    }

}
