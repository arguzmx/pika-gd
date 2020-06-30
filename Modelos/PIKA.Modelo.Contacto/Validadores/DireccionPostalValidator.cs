﻿using FluentValidation;
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
                .NotNull().WithMessage(x => localizer["El nombre es obligatorio"])
                .NotEmpty().WithMessage(x => localizer["El nombre es obligatorio"])
                .MinimumLength(1).WithMessage(x => localizer["El nombre debe tener entre {0} y {1} caracteres", 1, LongitudDatos.Nombre])
                .MaximumLength(LongitudDatos.Nombre).WithMessage(x => localizer["El nombre debe tener entre {0} y {1} caracteres", 1, LongitudDatos.Nombre]);

            RuleFor(x => x.Calle)
               .NotNull().WithMessage(x => localizer["La calle es obligatoria"])
               .NotEmpty().WithMessage(x => localizer["La calle es obligatoria"])
               .MinimumLength(1).WithMessage(x => localizer["La calle debe tener entre {0} y {1} caracteres", 1, LongitudDatos.Nombre])
               .MaximumLength(LongitudDatos.Nombre).WithMessage(x => localizer["La calle debe tener entre {0} y {1} caracteres", 1, LongitudDatos.Nombre]);

            RuleFor(x => x.TipoOrigenId)
               .NotNull().WithMessage(x => localizer["El tipo de orígen es obligatorio"])
               .NotEmpty().WithMessage(x => localizer["El tipo de orígen es obligatorio"]);

            RuleFor(x => x.OrigenId)
               .NotNull().WithMessage(x => localizer["El identificador de orígen es obligatorio"])
               .NotEmpty().WithMessage(x => localizer["El identificador de orígen es obligatorio"]);

        }
    }
}
