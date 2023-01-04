using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace PIKA.Infraestructura.Comun.Seguridad
{
    public class EventoAuditoria
    {
        /// <summary>
        /// Identificador único del evento, es la fecha expresada como ticks
        /// </summary>
        public long Id { get; set; }

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
        /// Clase y método ejecutado
        /// </summary>
        public string Fuente { get; set; }

        /// <summary>
        /// Identificador único de la fuente del evanto por ejemplo un servicio o controlado
        /// </summary>
        public string AppId { get; set; }


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
        /// Especifica el tipo de falla para un evento ejecutado con error
        /// </summary>
        public int? TipoFalla { get; set; }

        /// <summary>
        /// Tipo de la entidad involucrada
        /// </summary>
        public string TipoEntidad { get; set; }

        /// <summary>
        /// Identificador único de la entidad asociada
        /// </summary>
        public string IdEntidad { get; set; }


        /// <summary>
        /// Nombre para identificar la entidad en el resumen presentado al usuario
        /// </summary>
        public string NombreEntidad { get; set; }

        /// <summary>
        /// Cambios realizados al comparar la entidad serializada
        /// </summary>
        public string Delta { get; set; }

        [NotMapped]
        public DateTime Fecha { get { return new DateTime(this.Id); }  set {  }  }
       
    }
}
