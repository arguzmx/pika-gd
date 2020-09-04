using FluentValidation;
using Microsoft.Extensions.Localization;
using RepositorioEntidades;
using System;
using System.Collections.Generic;
using System.Text;

namespace PIKA.Infraestructura.Comun.Validadores
{
    public class TipoAdministradorModuloValidador : AbstractValidator<TipoAdministradorModulo>
    {
        public TipoAdministradorModuloValidador(IStringLocalizer<TipoAdministradorModulo> localizer)
        {
       
            RuleFor(x => x.AplicacionId)
                .NotEmpty()
                .MinimumLength(1)
                .MaximumLength(LongitudDatos.GUID);

            RuleFor(x => x.ModuloId)
              .NotNull()
              .NotEmpty()
              .MinimumLength(1)
              .MaximumLength(LongitudDatos.GUID);

        }
    }
}
