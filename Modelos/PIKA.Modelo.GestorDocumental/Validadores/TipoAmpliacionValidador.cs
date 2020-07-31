using FluentValidation;
using Microsoft.Extensions.Localization;
using RepositorioEntidades;
using System;
using System.Collections.Generic;
using System.Text;

namespace PIKA.Modelo.GestorDocumental.Validadores
{
    public class TipoAmpliacionValidador : AbstractValidator<TipoAmpliacion>
    {
        public TipoAmpliacionValidador(IStringLocalizer<TipoAmpliacion> localizer) {

            RuleFor(x => x.Id).NotEmpty().MinimumLength(1).MaximumLength(LongitudDatos.GUID);
            RuleFor(x => x.Nombre).NotEmpty().MinimumLength(1).MaximumLength(LongitudDatos.Nombre);

        }
    }
}
