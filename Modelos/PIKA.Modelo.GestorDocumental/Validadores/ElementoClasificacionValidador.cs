using FluentValidation;
using Microsoft.Extensions.Localization;
using RepositorioEntidades;
using System;
using System.Collections.Generic;
using System.Text;

namespace PIKA.Modelo.GestorDocumental.Validadores
{
  public  class ElementoClasificacionValidador : AbstractValidator<ElementoClasificacion>
    {
        public ElementoClasificacionValidador(IStringLocalizer<ElementoClasificacion> localizer)
        {
            RuleFor(x => x.Nombre).NotEmpty().MinimumLength(1).MaximumLength(LongitudDatos.Nombre);
            RuleFor(x => x.Clave).NotEmpty().MinimumLength(1).MaximumLength(LongitudDatos.Nombre);
            RuleFor(x => x.Posicion).GreaterThanOrEqualTo(0);
            RuleFor(x => x.CuadroClasifiacionId).NotEmpty().MaximumLength(LongitudDatos.GUID);
            RuleFor(x=>x.ElementoClasificacionId).MaximumLength(LongitudDatos.GUID);
            


        }
    }
}
