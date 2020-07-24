using System;
using System.Collections.Generic;
using System.Text;
using FluentValidation;
using Microsoft.Extensions.Localization;
using RepositorioEntidades;

namespace PIKA.Modelo.GestorDocumental.Validadores
{
  public  class TipoValoracionDocumentalValidador : AbstractValidator<TipoValoracionDocumental>
    {
        public TipoValoracionDocumentalValidador(IStringLocalizer<TipoValoracionDocumental> localizer)
        {
            RuleFor(x => x.Nombre)
                .NotNull()
                .NotEmpty()
                .MinimumLength(1)
                .MaximumLength(LongitudDatos.Nombre);


        }

    }
}
