using System;
using System.Collections.Generic;
using System.Text;
using FluentValidation;
using Microsoft.Extensions.Localization;
using RepositorioEntidades;

namespace PIKA.Modelo.GestorDocumental.Validadores
{
    public class TipoDisposicionDocumentalValidador : AbstractValidator<TipoDisposicionDocumental>
    {
        public TipoDisposicionDocumentalValidador(IStringLocalizer<TipoDisposicionDocumental> localizer)
        {
            RuleFor(x => x.Nombre)
                .NotEmpty()
                .MinimumLength(1)
                .MaximumLength(LongitudDatos.Nombre);
            RuleFor(x => x.Id)
              .NotEmpty()
              .MinimumLength(1)
              .MaximumLength(LongitudDatos.Nombre);


        }

    }
}
