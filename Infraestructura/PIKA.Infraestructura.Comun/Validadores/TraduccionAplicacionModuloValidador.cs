using FluentValidation;
using Microsoft.Extensions.Localization;
using RepositorioEntidades;
using System;
using System.Collections.Generic;
using System.Text;

namespace PIKA.Infraestructura.Comun.Validadores
{
    class TraduccionAplicacionModuloValidador : AbstractValidator<TraduccionAplicacionModulo>
    {
        public TraduccionAplicacionModuloValidador(IStringLocalizer<TraduccionAplicacionModulo> localizer)
        {
            RuleFor(x => x.Id)
              .NotNull().WithMessage(x => localizer["El traduccion aplicacion modulo id es obligatorio"])
              .NotEmpty().WithMessage(x => localizer["El traduccion aplicacion modulo id es obligatorio"])
              .MinimumLength(1).WithMessage(x => localizer["El traduccion aplicacion modulo id debe tener entre {0} y {1} caracteres", 1, LongitudDatos.GUID])
              .MaximumLength(LongitudDatos.GUID).WithMessage(x => localizer["El traduccion aplicacion modulo id debe tener entre {0} y {1} caracteres", 1, LongitudDatos.GUID]);

            RuleFor(x => x.AplicacionId)
                .NotNull().WithMessage(x => localizer["La aplicacion id es obligatoria"])
                .NotEmpty().WithMessage(x => localizer["La Aplicacion id es obligatoria"])
                .MinimumLength(1).WithMessage(x => localizer["La aplicacion id debe tener entre {0} y {1} caracteres", 1, LongitudDatos.GUID])
                .MaximumLength(LongitudDatos.GUID).WithMessage(x => localizer["La aplicacion id debe tener entre {0} y {1} caracteres", 1, LongitudDatos.GUID]);

            RuleFor(x => x.ModuloId)
              .NotNull().WithMessage(x => localizer["La modulo id es obligatoria"])
              .NotEmpty().WithMessage(x => localizer["La modulo id es obligatoria"])
              .MinimumLength(1).WithMessage(x => localizer["La modulo id debe tener entre {0} y {1} caracteres", 1, LongitudDatos.GUID])
              .MaximumLength(LongitudDatos.GUID).WithMessage(x => localizer["La modulo id debe tener entre {0} y {1} caracteres", 1, LongitudDatos.GUID]);

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

            RuleFor(x => x.UICulture)
                 .NotNull().WithMessage(x => localizer["La Cultura es obligatoria"])
                 .NotEmpty().WithMessage(x => localizer["La Cultura es obli gatoria"])
                 .MinimumLength(1).WithMessage(x => localizer["La Cultura debe tener entre {0} y {1} caracteres", 1, LongitudDatos.UICulture])
                 .MaximumLength(LongitudDatos.Icono).WithMessage(x => localizer["La Cultura debe tener entre {0} y {1} caracteres", 1, LongitudDatos.UICulture]);

        }
    }
}
