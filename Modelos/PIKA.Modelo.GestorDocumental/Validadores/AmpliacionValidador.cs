using FluentValidation;
using Microsoft.Extensions.Localization;
using RepositorioEntidades;
using System;
using System.Collections.Generic;
using System.Text;

namespace PIKA.Modelo.GestorDocumental.Validadores
{
    public class AmpliacionValidador : AbstractValidator<Ampliacion>
    {
        public AmpliacionValidador(IStringLocalizer<Ampliacion> localizer)
        {
            RuleFor(x => x.ActivoId)
               .NotNull().WithMessage(x => localizer["El activo es obligatorio"])
               .NotEmpty().WithMessage(x => localizer["El activo es obligatorio"]);

            RuleFor(x => x.TipoAmpliacionId)
               .NotNull().WithMessage(x => localizer["El tipo de ampliación es obligatorio"])
               .NotEmpty().WithMessage(x => localizer["El tipo de ampliación es obligatorio"]);

            RuleFor(x => x.FundamentoLegal)
                .NotNull().WithMessage(x => localizer["El fundamento legal es obligatorio"])
                .NotEmpty().WithMessage(x => localizer["El fundamento legal es obligatorio"])
                .MinimumLength(1).WithMessage(x => localizer["El fundamento legal debe tener entre {0} y {1} caracteres", 1, 2000])
                .MaximumLength(2000).WithMessage(x => localizer["El fundamento legal debe tener entre {0} y {1} caracteres", 1, 2000]);
        }
    }
}
