using System;
using System.Collections.Generic;
using System.Text;
using FluentValidation;
using Microsoft.Extensions.Localization;
using RepositorioEntidades;

namespace PIKA.Modelo.GestorDocumental.Validadores
{
    public class HistorialArchivoActivoValidador : AbstractValidator<HistorialArchivoActivo>
    {
        public HistorialArchivoActivoValidador(IStringLocalizer<HistorialArchivoActivo> localizer)
        {
            RuleFor(x => x.ArchivoId).NotEmpty().MinimumLength(1).MaximumLength(LongitudDatos.GUID);
            RuleFor(x => x.Id).NotEmpty().MinimumLength(1).MaximumLength(LongitudDatos.GUID);
            RuleFor(x => x.ActivoId).NotEmpty().MinimumLength(1).MaximumLength(LongitudDatos.GUID);
            RuleFor(x => x.FechaIngreso).NotEmpty();
        }
    }
}
