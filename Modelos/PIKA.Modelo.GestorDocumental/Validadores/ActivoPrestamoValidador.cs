using FluentValidation;
using Microsoft.Extensions.Localization;
using RepositorioEntidades;
using System;
using System.Collections.Generic;
using System.Text;

namespace PIKA.Modelo.GestorDocumental.Validadores
{
    public class ActivoPrestamoValidador : AbstractValidator<ActivoPrestamo>
    {
        public ActivoPrestamoValidador(IStringLocalizer<ActivoPrestamo> localizer)
        {
            RuleFor(x => x.ActivoId)
                .NotNull().WithMessage(x => localizer["El archivo es obligatorio"])
                .NotEmpty().WithMessage(x => localizer["El archivo es obligatorio"]);

            RuleFor(x => x.PrestamoId)
                .NotNull().WithMessage(x => localizer["El prestamo es obligatorio"])
                .NotEmpty().WithMessage(x => localizer["El prestamo es obligatorio"]);
        }
    }
}
