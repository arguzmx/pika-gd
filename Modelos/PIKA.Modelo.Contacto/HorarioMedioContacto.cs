using PIKA.Modelo.Metadatos;
using PIKA.Modelo.Metadatos.Atributos;
using RepositorioEntidades;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace PIKA.Modelo.Contacto
{

    [Entidad( EliminarLogico: false)]
    public class HorarioMedioContacto: HorarioBase
    {
        /// <summary>
        /// Identificador único del medio de contacto al que pertenece el horario
        /// </summary>
        public string MedioContactoId { get; set; }


        public override string Id { get => base.Id; set => base.Id = value; }


        public override bool SinHorario { get => base.SinHorario; set => base.SinHorario = value; }


        public override byte DiaSemana { get => base.DiaSemana; set => base.DiaSemana = value; }


        public override DateTime? Fin { get => base.Fin; set => base.Fin = value; }


        public override DateTime? Inicio { get => base.Inicio; set => base.Inicio = value; }




        /// <summary>
        /// Medio de conacto para el horario
        /// </summary>
        public MedioContacto MedioContacto { get; set; }
    }
}
