using FluentValidation;
using Microsoft.Extensions.Localization;
using RepositorioEntidades;
using System;
using System.Collections.Generic;
using System.Text;

namespace PIKA.Modelo.GestorDocumental.Validadores
{
    public class AmpliacionValidador : AbstractValidator<Ampliacion>
    {
        public AmpliacionValidador(IStringLocalizer<Ampliacion> localizer)
        {
            RuleFor(x => x.ActivoId).NotEmpty().MinimumLength(1).MaximumLength(LongitudDatos.GUID);
            RuleFor(x => x.TipoAmpliacionId).NotEmpty().MinimumLength(1).MaximumLength(LongitudDatos.GUID);
            RuleFor(x => x.FundamentoLegal).NotEmpty().MinimumLength(1).MaximumLength(2000);
        }
    }
}
