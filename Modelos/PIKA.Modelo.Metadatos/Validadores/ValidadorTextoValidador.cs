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
            RuleFor(x => x.PropiedadId).NotEmpty().MinimumLength(1).MaximumLength(LongitudDatos.Nombre);
            //RuleFor(x => x.longmin).NotEmpty();
            //RuleFor(x => x.longmax).NotEmpty();
            //RuleFor(x => x.valordefault).NotEmpty().MinimumLength(1).MaximumLength(LongitudDatos.Nombre);
            //RuleFor(x => x.regexp).NotEmpty().MinimumLength(1).MaximumLength(LongitudDatos.Nombre);

        }
    }
}
