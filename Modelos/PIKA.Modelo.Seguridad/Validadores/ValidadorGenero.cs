using FluentValidation;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using RepositorioEntidades;
using System;
using System.Linq;

namespace PIKA.Modelo.Seguridad.Validadores
{

    public class ValidadorPais : AbstractValidator<Genero>
    {
        public ValidadorPais(IStringLocalizer<Genero> localizer)
        {
            RuleFor(x => x.Nombre).NotEmpty().MinimumLength(1).MaximumLength(LongitudDatos.Nombre);
            RuleFor(x => x.Id).NotEmpty().MinimumLength(1).MaximumLength(LongitudDatos.GUID);
        }
    }
}
