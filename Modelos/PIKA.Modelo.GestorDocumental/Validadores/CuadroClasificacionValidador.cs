using FluentValidation;
using Microsoft.Extensions.Localization;
using RepositorioEntidades;
using System;
using System.Collections.Generic;
using System.Text;

namespace PIKA.Modelo.GestorDocumental.Validadores
{
    public class CuadroClasificacionValidador : AbstractValidator<CuadroClasificacion>
    {
        public CuadroClasificacionValidador(IStringLocalizer<CuadroClasificacion> localizer) {
            RuleFor(x => x.Nombre)
                .NotNull().WithMessage(x => localizer["El nombre es obligatorio"])
                .NotEmpty().WithMessage(x => localizer["El nombre es obligatorio"])
                .MinimumLength(1).WithMessage(x => localizer["El nombre debe tener entre {0} y {1} caracteres", 1, LongitudDatos.Nombre])
                .MaximumLength(LongitudDatos.Nombre).WithMessage(x => localizer["El nombre debe tener entre {0} y {1} caracteres", 1, LongitudDatos.Nombre]);

            RuleFor(x => x.EstadoCuadroClasificacionId)
                .NotNull().WithMessage(x => localizer["El estado del cuadro es obligatorio"])
                .NotEmpty().WithMessage(x => localizer["El estado del cuadro es obligatorio"]);

            RuleFor(x => x.TipoOrigenId)
                .NotNull().WithMessage(x => localizer["El tipo de origen es obligatorio"])
                .NotEmpty().WithMessage(x => localizer["El tipo de origen es obligatorio"]);

            RuleFor(x => x.OrigenId)
                .NotNull().WithMessage(x => localizer["El origen es obligatorio"])
                .NotEmpty().WithMessage(x => localizer["El origen es obligatorio"]);
        }

    }
}
