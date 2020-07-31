using FluentValidation;
using Microsoft.Extensions.Localization;
using RepositorioEntidades;
using System;
using System.Collections.Generic;
using System.Text;

namespace PIKA.Modelo.GestorDocumental.Validadores
{
    public class AsuntoValidador : AbstractValidator<Asunto>
    {
        public AsuntoValidador(IStringLocalizer<Asunto> localizer)
        {
            RuleFor(x => x.Id).NotEmpty().MinimumLength(1).MaximumLength(LongitudDatos.GUID);
            RuleFor(x => x.Contenido).NotEmpty().MinimumLength(1).MaximumLength(LongitudDatos.ControlHTML);
            RuleFor(x => x.ActivoId).NotEmpty().MinimumLength(1);
        }
    }
}
