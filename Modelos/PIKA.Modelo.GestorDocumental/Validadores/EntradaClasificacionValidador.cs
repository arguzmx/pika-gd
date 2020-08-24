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
            RuleFor(x => x.Nombre).MinimumLength(1).MaximumLength(LongitudDatos.Nombre);
            RuleFor(x => x.ElementoClasificacionId).NotEmpty();
            RuleFor(x => x.TipoDisposicionDocumentalId).MaximumLength(LongitudDatos.GUID);
            RuleFor(x=>x.Clave).NotEmpty().MinimumLength(1).MaximumLength(LongitudDatos.GUID);
            RuleFor(x=>x.Posicion).GreaterThanOrEqualTo(0);
            RuleFor(x=>x.VigenciaTramite).GreaterThanOrEqualTo(0);
            RuleFor(x=>x.VigenciaConcentracion).GreaterThanOrEqualTo(0);
        }
    }
}
