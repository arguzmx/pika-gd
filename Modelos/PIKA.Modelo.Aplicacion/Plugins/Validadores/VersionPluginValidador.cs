using System;
using System.Collections.Generic;
using System.Text;
using FluentValidation;
using Microsoft.Extensions.Localization;
using RepositorioEntidades;

namespace PIKA.Modelo.Aplicacion.Plugins.Validadores
{
   public class VersionPluginValidador : AbstractValidator<VersionPlugin>
    {
        public VersionPluginValidador(IStringLocalizer<VersionPlugin> localizer)
        {
            RuleFor(x => x.PluginId)
                .NotNull().WithMessage(x => localizer["El Plugin id es obligatorio"])
                .NotEmpty().WithMessage(x => localizer["El Plugin id es obligatorio"])
                .MinimumLength(1).WithMessage(x => localizer["El Plugin id debe tener entre {0} y {1} caracteres", 1, LongitudDatos.GUID])
                .MaximumLength(LongitudDatos.GUID).WithMessage(x => localizer["El Plugin id debe tener entre {0} y {1} caracteres", 1, LongitudDatos.GUID]);
            RuleFor(x => x.Version)
                .NotNull().WithMessage(x => localizer["La Version  es obligatorio"])
                .NotEmpty().WithMessage(x => localizer["La Version es obligatorio"])
                .MinimumLength(1).WithMessage(x => localizer["La Version debe tener entre {0} y {1} caracteres", 1, LongitudDatos.Nombre])
                .MaximumLength(LongitudDatos.Nombre).WithMessage(x => localizer["La Version  debe tener entre {0} y {1} caracteres", 1, LongitudDatos.Nombre]);
            RuleFor(x => x.URL)
       .NotNull().WithMessage(x => localizer["La URL es obligatorio"])
       .NotEmpty().WithMessage(x => localizer["La URL es obligatorio"])
       .MinimumLength(1).WithMessage(x => localizer["La URL debe tener entre {0} y {1} caracteres", 1, LongitudDatos.UICulture])
       .MaximumLength(LongitudDatos.UICulture).WithMessage(x => localizer["La URL debe tener entre {0} y {1} caracteres", 1, LongitudDatos.UICulture]);


            RuleFor(x => x.RequiereConfiguracion)
           .NotNull().WithMessage(x => localizer["El Eliminado es obligatorio"])
           .NotEmpty().WithMessage(x => localizer["El Eliminado es obligatorio"]);


        }
    }
}

