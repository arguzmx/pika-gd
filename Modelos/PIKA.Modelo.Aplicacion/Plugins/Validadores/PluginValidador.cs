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
                .NotEmpty().MinimumLength(1).MaximumLength(LongitudDatos.Nombre);

            RuleFor(x => x.Gratuito).NotEmpty();


        }
    }
}
