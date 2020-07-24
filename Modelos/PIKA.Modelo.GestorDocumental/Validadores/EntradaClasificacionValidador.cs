using System;
using System.Collections.Generic;
using System.Text;
using FluentValidation;
using Microsoft.Extensions.Localization;
using RepositorioEntidades;

namespace PIKA.Modelo.GestorDocumental.Validadores
{
    public class EntradaClasificacionValidador : AbstractValidator<EntradaClasificacion>
    {
        public EntradaClasificacionValidador(IStringLocalizer<EntradaClasificacion> localizer)
        {
            RuleFor(x => x.Nombre).NotNull().NotEmpty().MinimumLength(1).MaximumLength(LongitudDatos.Nombre);
            RuleFor(x => x.ElementoClasificacionId).NotNull().NotEmpty();
            RuleFor(x => x.TipoDisposicionDocumentalId).NotNull().NotEmpty().MaximumLength(LongitudDatos.GUID);
            RuleFor(x=>x.Clave).NotNull().NotEmpty().MinimumLength(1).MaximumLength(LongitudDatos.GUID);
            RuleFor(x=>x.Posicion).NotNull().NotEmpty();
            RuleFor(x=>x.MesesVigenciTramite).NotNull().NotEmpty();
            RuleFor(x=>x.MesesVigenciConcentracion).NotNull().NotEmpty();
            RuleFor(x=>x.MesesVigenciHistorico).NotNull().NotEmpty();
        }
    }
}
