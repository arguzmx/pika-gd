using System;
using System.Collections.Generic;
using System.Text;

namespace PIKA.Infraestructura.Comun.Tareas
{
    public class TareaEnDemanda
    {

        /// <summary>
        /// Identificador único de la tareas
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// NOmbre de la tareas
        /// </summary>
        public string Nombre { get; set; }

        /// <summary>
        /// URL relativo para la recolección del resultado del servicio
        /// </summary>
        public string URLRecoleccion { get; set; }


        /// <summary>
        /// Nombre de la clase ejecutora en el ensablado
        /// </summary>
        public string NombreEnsamblado { get; set; }

        /// <summary>
        /// Tipo de valor devuelto por el servicio
        /// </summary>
        public TareaEnDemandaTipoRespuesta TipoRespuesta { get; set; }

        /// <summary>
        /// Tipo de valor devuelto por el servicio
        /// </summary>
        public TareaEnDemandaPrioridad Prioridad { get; set; }

        /// <summary>
        ///  Horas antes de que el contenido sea eliminado
        /// </summary>
        public int HorasCaducidad { get; set; }
    }
}
