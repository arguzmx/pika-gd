using RepositorioEntidades;
using System;
using System.Collections.Generic;
using System.Text;

namespace PIKA.Modelo.Contacto
{

    public class HorarioMedioContacto: HorarioBase
    {
        /// <summary>
        /// Identificador único del medio de contacto al que pertenece el horario
        /// </summary>
        public string MedioContactoId { get; set; }



        /// <summary>
        /// Medio de conacto para el horario
        /// </summary>
        public MedioContacto MedioContacto { get; set; }
    }
}
