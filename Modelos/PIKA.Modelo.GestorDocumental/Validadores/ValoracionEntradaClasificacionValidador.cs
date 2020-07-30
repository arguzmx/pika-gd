using System;
using System.Collections.Generic;
using System.Text;
using FluentValidation;
using Microsoft.Extensions.Localization;
using RepositorioEntidades;

namespace PIKA.Modelo.GestorDocumental.Validadores
{
   public  class ValoracionEntradaClasificacionValidador : AbstractValidator<ValoracionEntradaClasificacion>
    {
        public ValoracionEntradaClasificacionValidador(IStringLocalizer<ValoracionEntradaClasificacion> localizer)
        {
            RuleFor(x => x.EntradaClasificacionId)
                .NotEmpty()
                .MinimumLength(1)
                .MaximumLength(LongitudDatos.GUID);

            RuleFor(x => x.TipoValoracionDocumentalId)
               .NotEmpty()
               .MinimumLength(1)
               .MaximumLength(LongitudDatos.GUID);
        }

    }
}
