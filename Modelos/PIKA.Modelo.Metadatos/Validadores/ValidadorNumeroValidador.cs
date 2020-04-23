using FluentValidation;
using Microsoft.Extensions.Localization;
using RepositorioEntidades;
using System;
using System.Collections.Generic;
using System.Text;

namespace PIKA.Modelo.Metadatos.Validadores
{
  public class ValidadorNumeroValidador : AbstractValidator<ValidadorNumero>
    {
        public ValidadorNumeroValidador(IStringLocalizer<ValidadorNumero> localizer)
        {
                  
            RuleFor(x => x.valordefault)
                    .NotNull().WithMessage(x => localizer["El valor default es obligatoria"])
                    .NotEmpty().WithMessage(x => localizer["El valor default es obligatorio"]);


        }
    }
}
