using FluentValidation;
using Microsoft.Extensions.Localization;
using RepositorioEntidades;
using System;
using System.Collections.Generic;
using System.Text;

namespace PIKA.Modelo.Metadatos.Validadores
{
  public class ValidadorNumeroValidador : AbstractValidator<ValidadorNumero>
    {
        public ValidadorNumeroValidador(IStringLocalizer<ValidadorNumero> localizer)
        {
            RuleFor(x => x.PropiedadId)
                .NotNull().WithMessage(x => localizer["El id propiedad es obligatorio"])
                .NotEmpty().WithMessage(x => localizer["El id propiedad es obligatorio"])
                .MinimumLength(1).WithMessage(x => localizer["El id propiedad debe tener entre {0} y {1} caracteres", 1, LongitudDatos.Nombre])
                .MaximumLength(LongitudDatos.Nombre).WithMessage(x => localizer["El id propiedad debe tener entre {0} y {1} caracteres", 1, LongitudDatos.Nombre]);

            RuleFor(x => x.min)
          .NotNull().WithMessage(x => localizer["El minimo es obligatoria"])
          .NotEmpty().WithMessage(x => localizer["El minimo es obligatorio"]);
            RuleFor(x => x.max)
                    .NotNull().WithMessage(x => localizer["El màximo es obligatoria"])
                    .NotEmpty().WithMessage(x => localizer["El màximo es obligatorio"]);
            RuleFor(x => x.valordefault)
                    .NotNull().WithMessage(x => localizer["El valor default es obligatoria"])
                    .NotEmpty().WithMessage(x => localizer["El valor default es obligatorio"]);


        }
    }
}
