﻿using FluentValidation;
using Microsoft.Extensions.Localization;
using RepositorioEntidades;
using System;
using System.Collections.Generic;
using System.Text;

namespace PIKA.Modelo.Organizacion.Validadores
{
    /// <summary>
    /// 
    /// </summary>
    public class UnidadOrganizacionalValidator : AbstractValidator<UnidadOrganizacional>
    {
        public UnidadOrganizacionalValidator(IStringLocalizer<UnidadOrganizacional> localizer)
        {
            RuleFor(x => x.Nombre)
                .NotNull().WithMessage(x => localizer["El nombre es obligatorio"])
                .NotEmpty().WithMessage(x => localizer["El nombre es obligatorio"])
                .MinimumLength(1).WithMessage(x => localizer["El nombre debe tener entre {0} y {1} caracteres", 1, LongitudDatos.Nombre])
                .MaximumLength(LongitudDatos.Nombre).WithMessage(x => localizer["El nombre debe tener entre {0} y {1} caracteres", 1, LongitudDatos.Nombre]);

            RuleFor(x => x.DominioId)
                .NotNull().WithMessage(x => localizer["El identificador de dominio es obligatorio"])
                .NotEmpty().WithMessage(x => localizer["El identificador de dominio es obligatorio"]);
        }
    }
}
