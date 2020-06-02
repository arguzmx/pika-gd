using Microsoft.Extensions.Localization;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;
using RepositorioEntidades;

namespace PIKA.Modelo.Organizacion.Validadores
{
    /// <summary>
    /// 
    /// </summary>
    public class RolValidator : AbstractValidator<Rol>
    {
        public RolValidator(IStringLocalizer<Rol> localizer)
        {
            RuleFor(x => x.Nombre)
                .NotNull().WithMessage(x => localizer["El nombre es obligatorio"])
                .NotEmpty().WithMessage(x => localizer["El nombre es obligatorio"])
                .MinimumLength(1).WithMessage(x => localizer["El nombre debe tener entre {0} y {1} caracteres", 1, LongitudDatos.Nombre])
                .MaximumLength(LongitudDatos.Nombre).WithMessage(x => localizer["El nombre debe tener entre {0} y {1} caracteres", 1, LongitudDatos.Nombre]);

            RuleFor(x => x.Descripcion)
                .NotNull().WithMessage(x => localizer["La descripción es  obligatoria"])
                .NotEmpty().WithMessage(x => localizer["La descripción es  es obligatoria"])
                .MinimumLength(1).WithMessage(x => localizer["La descripción debe tener entre {0} y {1} caracteres", 1, LongitudDatos.Nombre])
                .MaximumLength(LongitudDatos.Descripcion).WithMessage(x => localizer["La descripción debe tener entre {0} y {1} caracteres", 1, LongitudDatos.Nombre]);

            RuleFor(x => x.TipoOrigenId)
           .NotNull().WithMessage(x => localizer["El tipo de orígen es obligatorio"])
           .NotEmpty().WithMessage(x => localizer["El tipo de orígen es obligatorio"]);

            RuleFor(x => x.OrigenId)
               .NotNull().WithMessage(x => localizer["El identificador de orígen es obligatorio"])
               .NotEmpty().WithMessage(x => localizer["El identificador de orígen es obligatorio"]);

        }
    }
}
