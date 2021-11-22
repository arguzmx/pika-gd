using System;
using System.Collections.Generic;
using System.Text;

namespace PIKA.Modelo.CarpetaInteligente
{
    public class ConfiguracionEjecucion
    {
        /// <summary>
        /// Especifica el tipo de periodicidad para la tarea
        /// </summary>
        public TipoPeriodicidad PeriodicidadEjecucion { get; set; }

        /// <summary>
        /// Unidad de medida para la ejecuión de la tareas
        /// </summary>
        public UnidadesTiempo UnidadesTiempoEjecucion { get; set; }

        /// <summary>
        /// Número de unidades del intervalo para calcular la periodicidad
        /// </summary>
        public int IntervaloPeriodicidadEjecucion { get; set; }

        /// <summary>
        /// FEcha programanda para la ejecución cuando se realiza una sola vez
        /// </summary>
        public DateTime FechaEjecucion { get; set; }

        /// <summary>
        /// Si la unidad de tiempo para la ejecución es dias de las semana determina cuales son
        /// </summary>
        public List<DayOfWeek> DiasEjecucion { get; set; }

        /// <summary>
        /// Determinal la hora de ejecución de la tarea
        /// </summary>
        public DateTime? HoraEjecucion { get; set; }

    }
}
