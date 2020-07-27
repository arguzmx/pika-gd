using System;
using System.Collections.Generic;
using System.Text;
using FluentValidation;
using Microsoft.Extensions.Localization;
using RepositorioEntidades;

namespace PIKA.Modelo.Contenido
{
  public  class VolumenPuntosMontajeValidador : AbstractValidator<VolumenPuntoMontaje>
    {
        public VolumenPuntosMontajeValidador()
        {
            RuleFor(x => x.PuntoMontajeId).NotEmpty().NotNull().MaximumLength(LongitudDatos.GUID);
            RuleFor(x => x.VolumenId).NotEmpty().NotNull().MaximumLength(LongitudDatos.GUID);
        }
    }
}
