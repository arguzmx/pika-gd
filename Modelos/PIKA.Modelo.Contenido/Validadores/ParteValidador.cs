using System;
using System.Collections.Generic;
using System.Text;
using FluentValidation;
using Microsoft.Extensions.Localization;
using RepositorioEntidades;

namespace PIKA.Modelo.Contenido
{
    public class ParteValidador : AbstractValidator<Parte>
    {
        public ParteValidador(IStringLocalizer<Parte> localizer)
        {
            RuleFor(x => x.VersionId).NotEmpty().MaximumLength(LongitudDatos.GUID)
                .When(x => string.IsNullOrEmpty(x.Id));
            RuleFor(x => x.ElementoId).NotEmpty().MaximumLength(LongitudDatos.GUID)
                .When(x => string.IsNullOrEmpty(x.Id));
            RuleFor(x => x.NombreOriginal).NotEmpty().MinimumLength(1).MaximumLength(LongitudDatos.NombreLargo)
                .When(x => string.IsNullOrEmpty(x.Id));
            RuleFor(x => x.TipoMime).NotEmpty().MinimumLength(1).MaximumLength(LongitudDatos.MIME)
                .When(x => string.IsNullOrEmpty(x.Id));
            RuleFor(x => x.LongitudBytes).NotEmpty().GreaterThanOrEqualTo(0);

        }
    }
}