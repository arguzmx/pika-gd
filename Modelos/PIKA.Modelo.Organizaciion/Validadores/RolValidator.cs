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
        }
    }
}
