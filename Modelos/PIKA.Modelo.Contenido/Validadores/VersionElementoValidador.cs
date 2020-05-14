using System;
using System.Collections.Generic;
using System.Text;
using FluentValidation;
using Microsoft.Extensions.Localization;
using RepositorioEntidades;

namespace PIKA.Modelo.Contenido.Validadores
{
    public class VersionElementoValidador : AbstractValidator<Version>
    {
        public VersionElementoValidador(IStringLocalizer<Version> localizer)
        {
            RuleFor(x => x.ElementoId)
                .NotNull().WithMessage(x => localizer["El elemento es obligatorio"])
                .NotEmpty().WithMessage(x => localizer["El elemento es obligatorio"])
                .MinimumLength(1).WithMessage(x => localizer["El elemento debe tener entre {0} y {1} caracteres", 1, LongitudDatos.GUID])
                .MaximumLength(LongitudDatos.GUID).WithMessage(x => localizer["El elemento debe tener entre {0} y {1} caracteres", 1, LongitudDatos.GUID]);

            RuleFor(x => x.FechaActualizacion)
                .NotNull().WithMessage(x => localizer["La fecha de actualizaciòn es obligatorio"])
                .NotEmpty().WithMessage(x => localizer["La fecha de actualizaciòn es obligatorio"]);

            RuleFor(x => x.FechaCreacion)
             .NotNull().WithMessage(x => localizer["La fecha de creaciòn es obligatorio"])
             .NotEmpty().WithMessage(x => localizer["La fecha de creaciòn es obligatorio"]);


            RuleFor(x => x.Activa)
           .NotNull().WithMessage(x => localizer["El activo es obligatorio"])
           .NotEmpty().WithMessage(x => localizer["El activo es obligatorio"]);
        }
    }
}
