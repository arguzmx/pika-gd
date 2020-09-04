using FluentValidation;
using Microsoft.Extensions.Localization;
using RepositorioEntidades;
using System;
using System.Collections.Generic;
using System.Text;

namespace PIKA.Infraestructura.Comun.Validadores
{
    public class TraduccionAplicacionModuloValidador : AbstractValidator<TraduccionAplicacionModulo>
    {
        public TraduccionAplicacionModuloValidador(IStringLocalizer<TraduccionAplicacionModulo> localizer)
        {
            RuleFor(x => x.Id)
              .NotEmpty()
              .MinimumLength(1)
              .MaximumLength(LongitudDatos.GUID);

            RuleFor(x => x.AplicacionId)
                .NotEmpty()
                .MinimumLength(1)
                .MaximumLength(LongitudDatos.GUID);

            RuleFor(x => x.ModuloId)
              .NotEmpty()
              .MinimumLength(1)
              .MaximumLength(LongitudDatos.GUID);

            RuleFor(x => x.Nombre)
                .NotEmpty()
                .MinimumLength(1)
                .MaximumLength(LongitudDatos.Nombre);

            RuleFor(x => x.Descripcion)
                .NotEmpty()
                .MinimumLength(1)
                .MaximumLength(LongitudDatos.Descripcion);

            RuleFor(x => x.UICulture)
                 .NotEmpty()
                 .MinimumLength(1)
                 .MaximumLength(LongitudDatos.Icono);

        }
    }
}
