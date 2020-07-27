using System;
using System.Collections.Generic;
using System.Text;
using FluentValidation;
using Microsoft.Extensions.Localization;
using RepositorioEntidades;

namespace PIKA.Modelo.Contenido
{
   public class PuntoMontajeValidador : AbstractValidator<PuntoMontaje>
    {
        public PuntoMontajeValidador()
        {
            RuleFor(x => x.Nombre)
                .NotNull().NotEmpty()
                .MinimumLength(1).MaximumLength(LongitudDatos.Nombre);

            RuleFor(x => x.TipoOrigenId).NotNull().NotEmpty().MaximumLength(LongitudDatos.GUID);
            RuleFor(x => x.OrigenId).NotNull().NotEmpty().MaximumLength(LongitudDatos.GUID);
            RuleFor(x => x.CreadorId).NotNull().NotEmpty().MaximumLength(LongitudDatos.GUID)
                .When(x => string.IsNullOrEmpty(x.Id));
 
        }
    }
}
