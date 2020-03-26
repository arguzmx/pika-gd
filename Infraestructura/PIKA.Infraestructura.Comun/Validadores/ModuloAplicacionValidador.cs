using FluentValidation;
using Microsoft.Extensions.Localization;
using RepositorioEntidades;
using System;
using System.Collections.Generic;
using System.Text;

namespace PIKA.Infraestructura.Comun.Validadores
{
    class ModuloAplicacionValidador : AbstractValidator<ModuloAplicacion>
    {
        public ModuloAplicacionValidador(IStringLocalizer<ModuloAplicacion> localizer)
        {
            RuleFor(x => x.Nombre)
                .NotNull().WithMessage(x => localizer["El nombre es obligatorio"])
                .NotEmpty().WithMessage(x => localizer["El nombre es obligatorio"])
                .MinimumLength(1).WithMessage(x => localizer["El nombre debe tener entre {0} y {1} caracteres", 1, LongitudDatos.Nombre])
                .MaximumLength(LongitudDatos.Nombre).WithMessage(x => localizer["El nombre debe tener entre {0} y {1} caracteres", 1, LongitudDatos.Nombre]);

            RuleFor(x => x.Descripcion)
                .NotNull().WithMessage(x => localizer["La descripcion es obligatoria"])
                .NotEmpty().WithMessage(x => localizer["La descripcion es obligatoria"])
                .MinimumLength(1).WithMessage(x => localizer["La descripcion debe tener entre {0} y {1} caracteres", 1, LongitudDatos.Descripcion])
                .MaximumLength(LongitudDatos.Descripcion).WithMessage(x => localizer["La descripcion debe tener entre {0} y {1} caracteres", 1, LongitudDatos.Descripcion]);

            RuleFor(x => x.Icono)
                .NotNull().WithMessage(x => localizer["El Icono es obligatoria"])
                .NotEmpty().WithMessage(x => localizer["El Icono es obli gatoria"])
                .MinimumLength(1).WithMessage(x => localizer["El Icono debe tener entre {0} y {1} caracteres", 1, LongitudDatos.Icono])
                .MaximumLength(LongitudDatos.Icono).WithMessage(x => localizer["El Icono debe tener entre {0} y {1} caracteres", 1, LongitudDatos.Icono]);

            RuleFor(x => x.UICulture)
             .NotNull().WithMessage(x => localizer["La Cultura es obligatoria"])
             .NotEmpty().WithMessage(x => localizer["La Cultura es obli gatoria"])
             .MinimumLength(1).WithMessage(x => localizer["La Cultura debe tener entre {0} y {1} caracteres", 1, LongitudDatos.UICulture])
             .MaximumLength(LongitudDatos.Icono).WithMessage(x => localizer["La Cultura debe tener entre {0} y {1} caracteres", 1, LongitudDatos.UICulture]);

            RuleFor(x => x.Asegurable)
            .NotNull().WithMessage(x => localizer["El Asegurable es obligatoria"])
            .NotEmpty().WithMessage(x => localizer["El Asegurable es obli gatoria"]);

        }
    }
}
