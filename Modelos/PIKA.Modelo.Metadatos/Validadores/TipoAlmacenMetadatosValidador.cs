using System;
using System.Collections.Generic;
using System.Text;
using FluentValidation;
using Microsoft.Extensions.Localization;
using RepositorioEntidades;

namespace PIKA.Modelo.Metadatos.Validadores
{
  public  class TipoAlmacenMetadatosValidador : AbstractValidator<TipoAlmacenMetadatos>
    {
        public TipoAlmacenMetadatosValidador(IStringLocalizer<TipoAlmacenMetadatos> localizer)
        {
            RuleFor(x => x.Nombre).NotEmpty().MinimumLength(1).MaximumLength(LongitudDatos.Nombre);
        }
    }
}
