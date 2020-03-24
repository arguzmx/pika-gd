using FluentValidation;
using Microsoft.Extensions.Localization;
using RepositorioEntidades;
using System;
using System.Collections.Generic;
using System.Text;

namespace PIKA.Modelo.GestorDocumental.Validadores
{
    public class FaseCicloVitalValidador : AbstractValidator<FaseCicloVital>
    {
        public FaseCicloVitalValidador(IStringLocalizer<FaseCicloVital> localizer)
        {
            RuleFor(x => x.Nombre)
               .NotNull().WithMessage(x => localizer["El nombre es obligatorio"])
               .NotEmpty().WithMessage(x => localizer["El nombre es obligatorio"])
               .MinimumLength(1).WithMessage(x => localizer["El nombre debe tener entre {0} y {1} caracteres", 1, LongitudDatos.Nombre])
               .MaximumLength(LongitudDatos.Nombre).WithMessage(x => localizer["El nombre debe tener entre {0} y {1} caracteres", 1, LongitudDatos.Nombre]);
        }
    }
}
