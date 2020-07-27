using System;
using System.Collections.Generic;
using System.Text;
using FluentValidation;
using Microsoft.Extensions.Localization;
using RepositorioEntidades;

namespace PIKA.Modelo.Contenido
{
    public class VolumenValidador : AbstractValidator<Volumen>
    {
        public VolumenValidador(IStringLocalizer<Volumen> localizer)
        {
            RuleFor(x => x.Nombre).NotNull().NotEmpty().MinimumLength(1)
              .MaximumLength(LongitudDatos.Nombre);

            RuleFor(x => x.TipoOrigenId).NotEmpty().MaximumLength(LongitudDatos.GUID);

            RuleFor(x => x.OrigenId).NotEmpty().MaximumLength(LongitudDatos.GUID);

            RuleFor(x => x.TipoGestorESId).NotEmpty().MaximumLength(LongitudDatos.GUID);

            RuleFor(x => x.TamanoMaximo).NotNull().GreaterThanOrEqualTo(0);


        }
    }
}
