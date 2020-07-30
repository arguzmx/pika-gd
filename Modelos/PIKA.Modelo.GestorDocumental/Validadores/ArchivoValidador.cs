using FluentValidation;
using Microsoft.Extensions.Localization;
using RepositorioEntidades;
using System;
using System.Collections.Generic;
using System.Text;

namespace PIKA.Modelo.GestorDocumental.Validadores
{
    public class ArchivoValidador : AbstractValidator<Archivo>
    {
        public ArchivoValidador(IStringLocalizer<Archivo> localizer) {
            RuleFor(x => x.Nombre).NotEmpty().MinimumLength(1).MaximumLength(LongitudDatos.Nombre);
            RuleFor(x => x.TipoArchivoId).NotEmpty().MinimumLength(1).MaximumLength(LongitudDatos.GUID);
            RuleFor(x => x.TipoOrigenId).NotEmpty().MinimumLength(1).MaximumLength(LongitudDatos.GUID);
            RuleFor(x => x.OrigenId).NotEmpty().MinimumLength(1).MaximumLength(LongitudDatos.GUID);

        }
    }
}
