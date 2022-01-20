using RepositorioEntidades;
using System;
using System.Collections.Generic;
using System.Text;

namespace PIKA.Infraestructura.Comun.Tareas
{
    public class BitacoraTarea: Entidad<string>
    {
        public override string Id { get; set; }
        /// <summary>
        /// Identificador único de la tarea programada
        /// </summary>
        public virtual string TareaId { get; set; }

        /// <summary>
        /// Determina cuando fue realizada la última ejecución
        /// </summary>
        public virtual DateTime FechaEjecucion { get; set; }

        /// <summary>
        /// Duración en minutos de la última ejecución de la tarea
        /// </summary>
        public virtual int Duracion { get; set; }


        /// <summary>
        /// Determina si la  última ejecución resulto exitosa
        /// </summary>
        public virtual bool Exito { get; set; }

        /// <summary>
        /// En el caso de que la tare falle almacena el error asociado
        /// </summary>
        public virtual string CodigoError { get; set; }


    }
}
