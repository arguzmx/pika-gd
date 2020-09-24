using FluentValidation;
using Microsoft.Extensions.Localization;
using RepositorioEntidades;
using System;
using System.Collections.Generic;
using System.Text;

namespace PIKA.Modelo.Reportes.Validadores
{
 public  class ReporteEntidadValidador : AbstractValidator<ReporteEntidad>
    {
        public ReporteEntidadValidador(IStringLocalizer<ReporteEntidad> localizer)
        {
            RuleFor(x => x.Nombre).NotNull().NotEmpty().MinimumLength(1).MaximumLength(LongitudDatos.Nombre);
            RuleFor(x => x.OrigenId).NotNull().NotEmpty().MinimumLength(1).MaximumLength(LongitudDatos.GUID);
            RuleFor(x => x.TipoOrigenId).NotNull().NotEmpty().MinimumLength(1).MaximumLength(LongitudDatos.GUID);
            RuleFor(x => x.Descripcion).NotNull().NotEmpty().MinimumLength(1).MaximumLength(LongitudDatos.Descripcion);
            RuleFor(x => x.Entidad).NotNull().NotEmpty().MinimumLength(1).MaximumLength(LongitudDatos.NombreLargo);
            RuleFor(x => x.Plantilla).NotNull().NotEmpty().MinimumLength(1).MaximumLength(LongitudDatos.TEXTO_INDEXABLE_LARGO);
        }
    }
}
