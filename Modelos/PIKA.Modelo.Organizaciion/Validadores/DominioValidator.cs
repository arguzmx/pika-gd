using FluentValidation;
using Microsoft.Extensions.Localization;
using PIKA.Modelo.Organizacion;
using RepositorioEntidades;
using System;

namespace PIKA.Modelo.Organizacion
{

    /// <summary>
    /// 
    /// </summary>
    public class DominioValidator: AbstractValidator<Dominio>
    {

        public DominioValidator(IStringLocalizer<Dominio> localizer)
        {
            RuleFor(x => x.Nombre)
                .NotNull().WithMessage(x => localizer["El nombre es obligatorio"])
                .NotEmpty().WithMessage(x => localizer["El nombre es obligatorio"] )
                .MinimumLength(1).WithMessage(x => localizer["El nombre debe tener entre {0} y {1} caracteres", 1, LongitudDatos.Nombre])
                .MaximumLength(LongitudDatos.Nombre).WithMessage(x => localizer["El nombre debe tener entre {0} y {1} caracteres", 1, LongitudDatos.Nombre] );

            RuleFor(x => x.TipoOrigenId)
                           .NotNull().WithMessage(x => localizer["El tipo de orígen es obligatorio"])
                           .NotEmpty().WithMessage(x => localizer["El tipo de orígen es obligatorio"]);

            RuleFor(x => x.OrigenId)
               .NotNull().WithMessage(x => localizer["El identificador de orígen es obligatorio"])
               .NotEmpty().WithMessage(x => localizer["El identificador de orígen es obligatorio"]);

        }

    }
}
