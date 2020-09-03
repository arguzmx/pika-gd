using System;
using System.Collections.Generic;
using System.Text;
using FluentValidation;
using Microsoft.Extensions.Localization;
using RepositorioEntidades;

namespace PIKA.Modelo.Metadatos.Validadores
{
 public class ValorListaValidador : AbstractValidator<ValorLista>
    {
        public ValorListaValidador(IStringLocalizer<ValorLista> localizer)
        {
            RuleFor(x => x.Texto).NotEmpty().MinimumLength(1).MaximumLength(LongitudDatos.Nombre);
            RuleFor(x => x.Indice).NotEmpty().GreaterThanOrEqualTo(0);
        }
    }
}