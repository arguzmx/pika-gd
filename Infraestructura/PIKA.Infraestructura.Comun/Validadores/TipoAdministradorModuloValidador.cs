using FluentValidation;
using Microsoft.Extensions.Localization;
using RepositorioEntidades;
using System;
using System.Collections.Generic;
using System.Text;

namespace PIKA.Infraestructura.Comun.Validadores
{
    class TipoAdministradorModuloValidador : AbstractValidator<TipoAdministradorModulo>
    {
        public TipoAdministradorModuloValidador(IStringLocalizer<TipoAdministradorModulo> localizer)
        {
            RuleFor(x => x.Id)
                .NotNull().WithMessage(x => localizer["El tipo administrador modulo es obligatorio"])
                .NotEmpty().WithMessage(x => localizer["El tipo administrador modulo es obligatorio"])
                .MinimumLength(1).WithMessage(x => localizer["El tipo administrador modulo debe tener entre {0} y {1} caracteres", 1, LongitudDatos.GUID])
                .MaximumLength(LongitudDatos.GUID).WithMessage(x => localizer["El tipo administrador modulo debe tener entre {0} y {1} caracteres", 1, LongitudDatos.GUID]);

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

        }
    }
}
