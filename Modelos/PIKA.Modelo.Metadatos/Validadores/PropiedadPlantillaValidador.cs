using FluentValidation;
using Microsoft.Extensions.Localization;
using RepositorioEntidades;
using System;
using System.Collections.Generic;
using System.Text;

namespace PIKA.Modelo.Metadatos.Validadores
{
    public class PropiedadPlantillaValidador : AbstractValidator<PropiedadPlantilla>
    {
        public PropiedadPlantillaValidador(IStringLocalizer<PropiedadPlantilla> localizer)
        {
            RuleFor(x => x.Nombre).NotEmpty().MinimumLength(1).MaximumLength(LongitudDatos.Nombre);
            RuleFor(x => x.TipoDatoId).NotEmpty().MinimumLength(1).MaximumLength(LongitudDatos.Nombre)
                .When(x => x.Id == "" || x.Id == null);


        }
    }
}
