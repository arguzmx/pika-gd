using System;
using System.Collections.Generic;
using System.Text;
using FluentValidation;
using Microsoft.Extensions.Localization;
using RepositorioEntidades;

namespace PIKA.Modelo.Aplicacion.Plugins.Validadores
{
   public class PluginValidador : AbstractValidator<Plugin>
    {
        public PluginValidador(IStringLocalizer<Plugin> localizer)
        {
            RuleFor(x => x.Nombre)
                .NotNull().WithMessage(x => localizer["El nombre es obligatorio"])
                .NotEmpty().WithMessage(x => localizer["El nombre es obligatorio"])
                .MinimumLength(1).WithMessage(x => localizer["El nombre debe tener entre {0} y {1} caracteres", 1, LongitudDatos.Nombre])
                .MaximumLength(LongitudDatos.Nombre).WithMessage(x => localizer["El nombre debe tener entre {0} y {1} caracteres", 1, LongitudDatos.Nombre]);
            //RuleFor(x => x.VersionPluiginid)
            //    .NotNull().WithMessage(x => localizer["La Version del Pluigin id es obligatorio"])
            //    .NotEmpty().WithMessage(x => localizer["La Version del Pluigin id es obligatorio"])
            //    .MinimumLength(1).WithMessage(x => localizer["La Version del Pluigin id debe tener entre {0} y {1} caracteres", 1, LongitudDatos.Nombre])
            //    .MaximumLength(LongitudDatos.Nombre).WithMessage(x => localizer["La Version del Pluigin id debe tener entre {0} y {1} caracteres", 1, LongitudDatos.Nombre]);

            RuleFor(x => x.Gratuito)
           .NotNull().WithMessage(x => localizer["El Eliminado es obligatorio"])
           .NotEmpty().WithMessage(x => localizer["El Eliminado es obligatorio"]);


        }
    }
}
