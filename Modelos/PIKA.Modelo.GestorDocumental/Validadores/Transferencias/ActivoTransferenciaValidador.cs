using FluentValidation;
using Microsoft.Extensions.Localization;
using RepositorioEntidades;
using System;
using System.Collections.Generic;
using System.Text;

namespace PIKA.Modelo.GestorDocumental.Validadores
{
    public class ActivoTransferenciaValidador : AbstractValidator<ActivoTransferencia>
    {
        public ActivoTransferenciaValidador(IStringLocalizer<ActivoTransferencia> localizer)
        {

            //RuleFor(x => x.ActivoId)
            //    .NotNull().WithMessage(x => localizer["El activo es obligatorio"])
            //    .NotEmpty().WithMessage(x => localizer["El activo es obligatorio"])
            //    .MinimumLength(1).WithMessage(x => localizer["El activo debe tener entre {0} y {1} caracteres", 1, LongitudDatos.GUID])
            //    .MaximumLength(LongitudDatos.GUID).WithMessage(x => localizer["El activo debe tener entre {0} y {1} caracteres", 1, LongitudDatos.GUID]);

            RuleFor(x => x.TransferenciaId)
                .NotNull().WithMessage(x => localizer["La transferencia es obligatorio"])
                .NotEmpty().WithMessage(x => localizer["La transferencia  es obligatorio"])
                .MinimumLength(1).WithMessage(x => localizer["La transferencia debe tener entre {0} y {1} caracteres", 1, LongitudDatos.GUID])
                .MaximumLength(LongitudDatos.GUID).WithMessage(x => localizer["La transferencia debe tener entre {0} y {1} caracteres", 1, LongitudDatos.GUID]);
        }
    }
}
