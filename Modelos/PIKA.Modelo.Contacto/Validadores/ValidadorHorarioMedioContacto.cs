using FluentValidation;
using Microsoft.Extensions.Localization;
using RepositorioEntidades;
using System;
using System.Collections.Generic;
using System.Text;

namespace PIKA.Modelo.Contacto
{

    
    public class ValidadorHorarioMedioContacto : AbstractValidator<HorarioMedioContacto>
    {
        public ValidadorHorarioMedioContacto(IStringLocalizer<Pais> localizer) {
            RuleFor(x => x.SinHorario).NotNull();
            RuleFor(x => x.DiaSemana).NotNull();
            RuleFor(x => x.MedioContactoId).NotNull().MaximumLength(LongitudDatos.GUID);
            //Inicio datetime(6)
            //Fin datetime(6) 
        }
    }
}
