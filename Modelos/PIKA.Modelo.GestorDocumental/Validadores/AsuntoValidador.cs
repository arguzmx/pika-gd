using FluentValidation;
using Microsoft.Extensions.Localization;
using RepositorioEntidades;
using System;
using System.Collections.Generic;
using System.Text;

namespace PIKA.Modelo.GestorDocumental.Validadores
{
    public class AsuntoValidador : AbstractValidator<Asunto>
    {
        public AsuntoValidador(IStringLocalizer<Asunto> localizer)
        {
            RuleFor(x => x.Contenido)
                .NotNull().WithMessage(x => localizer["El contenido es obligatorio"])
                .NotEmpty().WithMessage(x => localizer["El contenido es obligatorio"])
                .MinimumLength(1).WithMessage(x => localizer["El nombre debe tener al menos {0} caracteres"]);

            RuleFor(x => x.ActivoId)
                .NotNull().WithMessage(x => localizer["El activo es obligatorio"])
                .NotEmpty().WithMessage(x => localizer["El activo es obligatorio"]);
        }
    }
}
