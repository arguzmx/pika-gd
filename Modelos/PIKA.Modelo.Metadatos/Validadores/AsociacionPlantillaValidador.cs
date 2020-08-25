using System;
using System.Collections.Generic;
using System.Text;
using FluentValidation;
using Microsoft.Extensions.Localization;
using RepositorioEntidades;

namespace PIKA.Modelo.Metadatos.Validadores
{
 public   class AsociacionPlantillaValidador : AbstractValidator<AsociacionPlantilla>
    {
        public AsociacionPlantillaValidador(IStringLocalizer<AsociacionPlantilla> localizer)
        {
            RuleFor(x => x.TipoOrigenId).NotEmpty().MinimumLength(1).MaximumLength(LongitudDatos.GUID);
            RuleFor(x => x.OrigenId).NotNull().NotEmpty().MinimumLength(1).MaximumLength(LongitudDatos.GUID);
            RuleFor(x => x.PlantillaId).NotNull().NotEmpty().MinimumLength(1).MaximumLength(LongitudDatos.GUID);
            RuleFor(x => x.IdentificadorAlmacenamiento).NotNull().NotEmpty().MinimumLength(1).MaximumLength(LongitudDatos.GUID);
        }
    }
}
