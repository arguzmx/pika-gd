using RepositorioEntidades;
using System;
using System.Collections.Generic;
using System.Text;

namespace PIKA.Infraestructura.Comun.Tareas
{

    public enum PeriodoProgramacion
    {
        Unico =0 , Hora =1 , Diario=2, DiaSemana=3, DiaMes=4, Continuo=10
    }


    public enum EstadoTarea
    {
        Habilidata = 0, Enejecucion = 1, Pausada = 2, ErrorConfiguracion = 3
    }

    public abstract class ProgramacionTarea: Entidad<string>
    {
        /// <summary>
        /// En el caso de que la tare falle almacena el error asociado
        /// </summary>
        public virtual string CodigoError { get; set; }

        /// <summary>
        /// Determina si la  última ejecución resulto exitosa
        /// </summary>
        public virtual bool? Exito { get; set; }

        /// <summary>
        /// Determina cuando fue realizada la última ejecución
        /// </summary>
        public virtual DateTime? UltimaEjecucion { get; set; }

        /// <summary>
        /// Determina cuando será realizada la próxima ejecución
        /// </summary>
        public virtual DateTime? ProximaEjecucion { get; set; }

        /// <summary>
        /// Duración en minutos de la última ejecución de la tarea
        /// </summary>
        public virtual int? Duracion { get; set; }

        public virtual PeriodoProgramacion Periodo { get; set; }
        
        /// <summary>
        /// Fecha u hora de ejecución de acuerdo al tipo programado
        /// </summary>
        public virtual DateTime? FechaHoraEjecucion  { get; set; }

        /// <summary>
        /// Determina el intervalo para la ejecución de acuerdo al tipo
        /// Unico, no aplica
        /// Diario, no aplica
        /// Hora, se ejecuta cada Intervalo de horas
        /// DiaSemana, día de la semana iniciando en domingo =0
        /// DiaMes, día del mes si el mes tiene menos días se recorre al último disponible 
        /// </summary>
        public virtual int? Intervalo { get; set; }



    }
}
