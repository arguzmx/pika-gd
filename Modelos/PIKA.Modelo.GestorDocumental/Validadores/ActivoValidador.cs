using FluentValidation;
using Microsoft.Extensions.Localization;
using RepositorioEntidades;
using System;
using System.Collections.Generic;
using System.Text;

namespace PIKA.Modelo.GestorDocumental.Validadores
{
    public class ActivoValidador : AbstractValidator<Activo>
    {
        public ActivoValidador(IStringLocalizer<Activo> localizer)
        {
            RuleFor(x => x.Nombre).NotEmpty().MinimumLength(1).MaximumLength(LongitudDatos.Nombre);

            RuleFor(x => x.Asunto).MaximumLength(2048);

            RuleFor(x => x.FechaApertura).NotEmpty();

            RuleFor(x => x.CodigoOptico).MaximumLength(1024);

            RuleFor(x => x.CodigoElectronico).MaximumLength(1024);

            RuleFor(x => x.ArchivoId).NotEmpty().MaximumLength(LongitudDatos.GUID);

            RuleFor(x => x.ElementoClasificacionId).NotEmpty().MaximumLength(LongitudDatos.GUID);

            RuleFor(x => x.TipoOrigenId).NotEmpty().MaximumLength(LongitudDatos.GUID);

            RuleFor(x => x.OrigenId).NotEmpty().MaximumLength(LongitudDatos.GUID);

        }
    }
}
