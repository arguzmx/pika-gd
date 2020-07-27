using System;
using System.Collections.Generic;
using System.Text;
using FluentValidation;
using Microsoft.Extensions.Localization;
using RepositorioEntidades;

namespace PIKA.Modelo.Contenido
{
  public  class TipoGestorESValidador : AbstractValidator<TipoGestorES>
    {
        public TipoGestorESValidador()
        {
            RuleFor(x => x.Nombre)
                .NotNull()
                .NotEmpty()
                .MinimumLength(1)
                .MaximumLength(LongitudDatos.Nombre);

        }
    }
}
