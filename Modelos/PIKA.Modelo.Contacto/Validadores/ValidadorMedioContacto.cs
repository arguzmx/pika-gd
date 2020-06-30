using FluentValidation;
using Microsoft.Extensions.Localization;
using RepositorioEntidades;
using System;
using System.Collections.Generic;
using System.Text;

namespace PIKA.Modelo.Contacto
{

    
    public class ValidadorMedioContacto : AbstractValidator<MedioContacto>
    {
        public ValidadorMedioContacto(IStringLocalizer<Pais> localizer) {
            RuleFor(x => x.OrigenId).NotEmpty().NotNull().MaximumLength(LongitudDatos.GUID);
            RuleFor(x => x.TipoOrigenId).NotEmpty().NotNull().MaximumLength(LongitudDatos.GUID);
            RuleFor(x => x.Medio).NotEmpty().NotNull().MinimumLength(1).MaximumLength(500);
            RuleFor(x => x.TipoMedioId).NotEmpty().NotNull();
            RuleFor(x => x.Prefijo).NotEmpty().NotNull().MaximumLength(100);
            RuleFor(x => x.Sufijo).NotEmpty().NotNull().MaximumLength(100);
            RuleFor(x => x.Notas).NotEmpty().NotNull().MaximumLength(500);
        }
    }
}
