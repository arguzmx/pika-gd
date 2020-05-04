using RepositorioEntidades;
using System;
using System.Collections.Generic;
using System.Text;

namespace PIKA.Modelo.GestorDocumental
{

    public class Ampliacion : Entidad<string>
    {

        /// <summary>
        /// Identificador único del activo al que se asocia la reserva
        /// </summary>
        public string ActivoId { get; set; }
        //# debe formar parte de un undice compuesto con  vigente

        /// <summary>
        /// Identifica si la amplaición se encuentra vigente
        /// </summary>
        public bool Vigente { get; set; }
        //# debe formar parte de un undice compuesto con  ActivoId

        /// <summary>
        /// Identificadorúnico del tipo de ampliacion
        /// </summary>
        public string TipoAmpliacionId { get; set; }
        //# requerido

        /// <summary>
        /// Especifica que la reserva debe tener una fecha de fin de reserva si este campo se encuentra activo
        /// en caso contrario deben indicarse un valor en anos, meses o días
        /// </summary>
        public bool FechaFija { get; set; }
        //WRequerido default=false

        /// <summary>
        /// Funcamento para la reserva
        /// </summary>
        public string FundamentoLegal { get; set; }
        // Requeirdo longitud 2000

        /// <summary>
        /// Fecha de inicio de la reserva, corresponde a la fecha de cierre si no se especifica
        /// </summary>
        public DateTime? Inicio { get; set; }




        /// <summary>
        /// Fecha de finalización de la reserva, si este campo es especificado no son tomados en cuenta los campos, anos, meses, dias
        /// </summary>
        public DateTime? Fin { get; set; }

                /// <summary>
        /// Años para la amepliación
        /// </summary>
        public int? Anos { get; set; }

        /// <summary>
        /// Meses para la ampliacion
        /// </summary>
        public int? Meses { get; set; }

        /// <summary>
        /// Dias para la ampliación
        /// </summary>
        public int? Dias { get; set; }



        public TipoAmpliacion TipoAmpliacion { get; set; }

        public Activo activo { get; set; }

    }
}
