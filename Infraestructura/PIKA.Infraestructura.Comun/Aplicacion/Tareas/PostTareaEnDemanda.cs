using System;
using System.Collections.Generic;
using System.Text;

namespace PIKA.Infraestructura.Comun.Tareas
{
    public class PostTareaEnDemanda
    {
        public PostTareaEnDemanda()
        {
            Completado = false;
            ConError = false;
            Error = null;
        }

        public string Id { get; set; }
        public string Tipo { get; set; }
        public DateTime Fecha { get; set; }
        public string PickupURL { get; set; }
        public bool Completado { get; set; }
        public bool ConError { get; set; }
        public string Error { get; set; }
        public TareaEnDemandaTipoRespuesta TipoRespuesta { get; set; }
        
        /// <summary>
        /// Texto descriptivo para el usuario
        /// </summary>
        public string Etiqueta { get; set; }
    }
}
