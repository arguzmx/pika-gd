using FluentValidation;
using Microsoft.Extensions.Localization;
using RepositorioEntidades;
using System;
using System.Collections.Generic;
using System.Text;

namespace PIKA.Modelo.Metadatos.Validadores
{
    public class TipoDatoValidador : AbstractValidator<TipoDato>
    {
        public TipoDatoValidador(IStringLocalizer<TipoDato> localizer)
        {
            RuleFor(x => x.Nombre).NotEmpty().MinimumLength(1).MaximumLength(LongitudDatos.Nombre);

            ;
        }
    }
}
