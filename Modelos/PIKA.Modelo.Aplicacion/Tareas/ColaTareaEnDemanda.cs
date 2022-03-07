using PIKA.Infraestructura.Comun.Tareas;
using System;
using System.Collections.Generic;
using System.Text;

namespace PIKA.Modelo.Aplicacion.Tareas
{
    /// <summary>
    /// Crea una cola de tareas en demanda que son atendidas por prioridad de manera secuencial
    /// </summary>
    public class ColaTareaEnDemanda
    {
        /// <summary>
        /// Identificador único de la tarea
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// FEcha de creación de la tarea
        /// </summary>
        public DateTime FechaCreacion { get; set; }

        /// <summary>
        /// Fecha de ejecución de la tares
        /// </summary>
        public DateTime? FechaEjecucion { get; set; }

        /// <summary>
        /// determina si la tarea ha sido completada
        /// </summary>
        public bool Completada { get; set; }

        /// <summary>
        /// Identificador único del usuario solicitante
        /// </summary>
        public string UsuarioId { get; set; }

        /// <summary>
        /// Identificador único del dominio del usuario solicitante
        /// </summary>
        public string DominioId { get; set; }

        /// <summary>
        /// Identificador único de la unidad organizacional del usuario solicitante
        /// </summary>
        public string TenantId { get; set; }


        /// <summary>
        /// Nombre de la clase ejecutora en el ensablado
        /// </summary>
        public string NombreEnsamblado { get; set; }

        /// <summary>
        /// Identificador único del método ejecutor de la tareas
        /// </summary>
        public string TareaProcesoId { get; set; }

        /// <summary>
        /// Datos necesarios para la ejecución de la tarea serializados como texto
        /// </summary>
        public string InputPayload { get; set; }

        /// <summary>
        /// Resultado de la ejecución de la tarea serializados como texto
        /// </summary>
        public string OutputPayload { get; set; }

        public TareaEnDemandaPrioridad Prioridad { get; set; }


        /// <summary>
        /// Descripción del error si ocurre durante el proceso
        /// </summary>
        public string Error { get; set; }

        /// <summary>
        /// URL relativo para la recolección del resultado del servicio
        /// </summary>
        public string URLRecoleccion { get; set; }


        /// <summary>
        /// Especifica cuanto tiempo la respuesta va a estar dispoinible para su consulta
        /// </summary>
        public DateTime? FechaCaducidad { get; set; }

        /// <summary>
        /// Tipo de valor devuelto por el servicio
        /// </summary>
        public TareaEnDemandaTipoRespuesta TipoRespuesta { get; set; }

        /// <summary>
        /// Indica si el resultado de la tarea ha sido consultado
        /// </summary>
        public bool Recogida { get; set; }


        /// <summary>
        ///  Horas antes de que el contenido sea eliminado
        /// </summary>
        public int HorasCaducidad { get; set; }
    }


}
