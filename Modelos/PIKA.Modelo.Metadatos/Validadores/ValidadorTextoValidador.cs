using FluentValidation;
using Microsoft.Extensions.Localization;
using RepositorioEntidades;
using System;
using System.Collections.Generic;
using System.Text;

namespace PIKA.Modelo.Metadatos.Validadores
{
    public class ValidadorTextoValidador : AbstractValidator<ValidadorTexto>
    {
        public ValidadorTextoValidador(IStringLocalizer<ValidadorTexto> localizer)
        {
            RuleFor(x => x.PropiedadId)
                .NotNull().WithMessage(x => localizer["El id propiedad es obligatorio"])
                .NotEmpty().WithMessage(x => localizer["El id propiedad es obligatorio"])
                .MinimumLength(1).WithMessage(x => localizer["El id propiedad debe tener entre {0} y {1} caracteres", 1, LongitudDatos.Nombre])
                .MaximumLength(LongitudDatos.Nombre).WithMessage(x => localizer["El id propiedad debe tener entre {0} y {1} caracteres", 1, LongitudDatos.Nombre]);

            RuleFor(x => x.longmin)
          .NotNull().WithMessage(x => localizer["La longitud minima es obligatoria"])
          .NotEmpty().WithMessage(x => localizer["La longitud minima es obligatorio"]);
            RuleFor(x => x.longmax)
                    .NotNull().WithMessage(x => localizer["La longuitud màxima es obligatoria"])
                    .NotEmpty().WithMessage(x => localizer["La longuitud màxima es obligatorio"]);
       
            RuleFor(x => x.valordefaulr)
                          .NotNull().WithMessage(x => localizer["El valor default es obligatorio"])
                          .NotEmpty().WithMessage(x => localizer["El valor default es obligatorio"])
                          .MinimumLength(1).WithMessage(x => localizer["El valor default debe tener entre {0} y {1} caracteres", 1, LongitudDatos.Nombre])
                          .MaximumLength(LongitudDatos.Nombre).WithMessage(x => localizer["El valor default debe tener entre {0} y {1} caracteres", 1, LongitudDatos.Nombre]);
            RuleFor(x => x.regexp)
                   .NotNull().WithMessage(x => localizer["El regexp es obligatorio"])
                   .NotEmpty().WithMessage(x => localizer["El regexp es obligatorio"])
                   .MinimumLength(1).WithMessage(x => localizer["El regexp debe tener entre {0} y {1} caracteres", 1, LongitudDatos.Nombre])
                   .MaximumLength(LongitudDatos.Nombre).WithMessage(x => localizer["El regexp debe tener entre {0} y {1} caracteres", 1, LongitudDatos.Nombre]);

        }
    }
}
