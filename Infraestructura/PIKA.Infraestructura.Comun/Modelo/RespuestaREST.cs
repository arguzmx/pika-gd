using System;
using System.Collections.Generic;
using System.Text;

namespace PIKA.Infraestructura.Comun
{
    /// <summary>
    /// Respuesta generíca para operaciones REST
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class RespuestaREST<T>
    {

        /// <summary>
        /// Asume que el resultado es incorecto y debe establcerse el valos de la variable
        /// EsCorrecto manualmente
        /// </summary>
        RespuestaREST() {
            Errores = new HashSet<string>();
            this.FechaInicio = DateTime.UtcNow;
            this.EsCorrecto = false;
        }

        /// <summary>
        /// Inicio UTC de la operación 
        /// </summary>
        public DateTime FechaInicio { get; set; }

        /// <summary>
        /// Fin de la oepración
        /// </summary>
        public DateTime FechaFinalizacion { get; set; }

        /// <summary>
        /// Datos devueltos por la ejecución
        /// </summary>
        public T Datos { get; set; }

        /// <summary>
        /// Indica si el llamdado finalizó satosfactoriamente
        /// </summary>
        public bool EsCorrecto { get; set; }


        /// <summary>
        /// Lista de errores durante la ejecución
        /// </summary>
        public ICollection<string> Errores { get; set; }


    }
}
