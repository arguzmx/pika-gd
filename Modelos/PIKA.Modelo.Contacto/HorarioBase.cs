using RepositorioEntidades;
using System;
using System.Collections.Generic;
using System.Text;

namespace PIKA.Modelo.Contacto
{
    public enum DiaSemanaContacto
    {
        Ninguno = 0, Lunes = 1, Martes = 2, Miercoles = 4, Jueves = 8, Viernes = 16, Sabado = 32, Domingo = 64
    }
    public abstract class HorarioBase: Entidad<string>
    {
        /// <summary>
        /// Especifica que el contacto puede realizar sin horario 
        /// A cualquier hora, cualquier día
        /// </summary>
        public bool SinHorario { get; set; }

        /// <summary>
        /// Utiliza la enumeración de días de la semana para determinar los días activos
        /// </summary>
        public byte DiaSemana { get; set; }

        /// <summary>
        /// Hora de inicio de dsiponibilidad para el contacto
        /// </summary>
        public DateTime? Inicio { get; set; }

        /// <summary>
        /// Hora de finalización de dsiponibilidad para el contacto
        /// </summary>
        public DateTime? Fin { get; set; }
    }
}
