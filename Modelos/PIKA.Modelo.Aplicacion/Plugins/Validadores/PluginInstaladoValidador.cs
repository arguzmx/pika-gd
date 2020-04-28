using System;
using System.Collections.Generic;
using System.Text;
using FluentValidation;
using Microsoft.Extensions.Localization;
using RepositorioEntidades;

namespace PIKA.Modelo.Aplicacion.Plugins.Validadores
{
  public  class PluginInstaladoValidador : AbstractValidator<PluginInstalado>
    {
        public PluginInstaladoValidador(IStringLocalizer<PluginInstalado> localizer)
        {
            RuleFor(x => x.PLuginId)
                .NotNull().WithMessage(x => localizer["El Plugin id es obligatorio"])
                .NotEmpty().WithMessage(x => localizer["El Plugin id es obligatorio"])
                .MinimumLength(1).WithMessage(x => localizer["El Plugin id debe tener entre {0} y {1} caracteres", 1, LongitudDatos.GUID])
                .MaximumLength(LongitudDatos.GUID).WithMessage(x => localizer["El Plugin id debe tener entre {0} y {1} caracteres", 1, LongitudDatos.GUID]);
            
            RuleFor(x => x.VersionPLuginId)
                .NotNull().WithMessage(x => localizer["La Version del Plugin Id es obligatorio"])
                .NotEmpty().WithMessage(x => localizer["La  Version del Plugin Id es obligatorio"])
                .MinimumLength(1).WithMessage(x => localizer["La  Version del Plugin Id debe tener entre {0} y {1} caracteres", 1, LongitudDatos.GUID])
                .MaximumLength(LongitudDatos.GUID).WithMessage(x => localizer["La  Version del Plugin Id  debe tener entre {0} y {1} caracteres", 1, LongitudDatos.GUID]);
       
        //     RuleFor(x => x.FechaInstalacion)
       //.NotNull().WithMessage(x => localizer["La URL es obligatorio"])
       //.NotEmpty().WithMessage(x => localizer["La URL es obligatorio"])
       //.MinimumLength(1).WithMessage(x => localizer["La URL debe tener entre {0} y {1} caracteres", 1, LongitudDatos.UICulture])
       //.MaximumLength(LongitudDatos.UICulture).WithMessage(x => localizer["La URL debe tener entre {0} y {1} caracteres", 1, LongitudDatos.UICulture]);


            RuleFor(x => x.Activo)
           .NotNull().WithMessage(x => localizer["El activo es obligatorio"])
           .NotEmpty().WithMessage(x => localizer["El activo es obligatorio"]);


        }
    }
}

