using FluentValidation;
using Microsoft.Extensions.Localization;
using RepositorioEntidades;
using System;
using System.Collections.Generic;
using System.Text;

namespace PIKA.Modelo.GestorDocumental.Validadores
{
    class ElementoClasificacionValidador : AbstractValidator<ElementoClasificacion>
    {
        public ElementoClasificacionValidador(IStringLocalizer<ElementoClasificacion> localizer)
        {
            RuleFor(x => x.Nombre)
                .NotNull().WithMessage(x => localizer["El nombre es obligatorio"])
                .NotEmpty().WithMessage(x => localizer["El nombre es obligatorio"])
                .MinimumLength(1).WithMessage(x => localizer["El nombre debe tener entre {0} y {1} caracteres", 1, LongitudDatos.Nombre])
                .MaximumLength(LongitudDatos.Nombre).WithMessage(x => localizer["El nombre debe tener entre {0} y {1} caracteres", 1, LongitudDatos.Nombre]);

            RuleFor(x => x.Clave)
                .NotNull().WithMessage(x => localizer["La clave es obligatoria"])
                .NotEmpty().WithMessage(x => localizer["La clave es obligatoria"])
                .MinimumLength(1).WithMessage(x => localizer["La clave debe tener entre {0} y {1} caracteres", 1, LongitudDatos.Nombre])
                .MaximumLength(LongitudDatos.Nombre).WithMessage(x => localizer["La clave debe tener entre {0} y {1} caracteres", 1, LongitudDatos.Nombre]);

            RuleFor(x => x.Posicion)
                .NotNull().WithMessage(x => localizer["La posición es obligatoria"])
                .NotEmpty().WithMessage(x => localizer["La posición es obligatoria"])
                .LessThanOrEqualTo(0).WithMessage(x => localizer["La posición tiene que ser mayor a cero"]);

            RuleFor(x => x.CuadroClasifiacionId)
                .NotNull().WithMessage(x => localizer["El cuadro clasificación es obligatorio"])
                .NotEmpty().WithMessage(x => localizer["El cuadro clasificación es obligatorio"]);


        }
    }
}
