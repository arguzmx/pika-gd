using FluentValidation;
using Microsoft.Extensions.Localization;
using RepositorioEntidades;
using System;
using System.Collections.Generic;
using System.Text;

namespace PIKA.Modelo.Contacto
{
    public class DireccionPostalValidator : AbstractValidator<DireccionPostal>
    {
        public DireccionPostalValidator(IStringLocalizer<DireccionPostal> localizer)
        {
            RuleFor(x => x.Colonia).MaximumLength(LongitudDatos.Nombre);
            RuleFor(x => x.CP).MaximumLength(LongitudDatos.CodigoPostal);
            RuleFor(x => x.EstadoId).MaximumLength(LongitudDatos.GUID);
            RuleFor(x => x.PaisId).MaximumLength(LongitudDatos.GUID);
            RuleFor(x => x.OrigenId).MaximumLength(LongitudDatos.GUID);
            RuleFor(x => x.TipoOrigenId).MaximumLength(LongitudDatos.GUID);
            RuleFor(x => x.Municipio).MaximumLength(LongitudDatos.Nombre);
            RuleFor(x => x.NoExterno).MaximumLength(LongitudDatos.Nombre);
            RuleFor(x => x.NoInterno).MaximumLength(LongitudDatos.Nombre);

            RuleFor(x => x.Nombre)
                .NotNull()
                .NotEmpty()
                .MinimumLength(1)
                .MaximumLength(LongitudDatos.Nombre);

            RuleFor(x => x.Calle)
               .NotNull()
               .NotEmpty()
               .MinimumLength(1)
               .MaximumLength(LongitudDatos.Nombre);

            RuleFor(x => x.TipoOrigenId)
               .NotNull()
               .NotEmpty();

            RuleFor(x => x.OrigenId)
               .NotNull()
               .NotEmpty();

        }
    }
}
