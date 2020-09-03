using FluentValidation;
using Microsoft.Extensions.Localization;
using RepositorioEntidades;
using System;
using System.Collections.Generic;
using System.Text;

namespace PIKA.Infraestructura.Comun.Validadores
{
    public class ModuloAplicacionValidador : AbstractValidator<ModuloAplicacion>
    {
        public ModuloAplicacionValidador(IStringLocalizer<ModuloAplicacion> localizer)
        {
            RuleFor(x => x.Nombre).NotEmpty().MinimumLength(1).MaximumLength(LongitudDatos.Nombre);

            RuleFor(x => x.Descripcion)
                .NotEmpty().MinimumLength(1).MaximumLength(LongitudDatos.Descripcion);

            RuleFor(x => x.Icono)
                .NotEmpty()
                .MinimumLength(1)
                .MaximumLength(LongitudDatos.Icono);

            RuleFor(x => x.UICulture)
             .NotEmpty()
             .MinimumLength(1)
             .MaximumLength(LongitudDatos.Icono);

            RuleFor(x => x.Asegurable)
            .NotEmpty();

        }
    }
}
