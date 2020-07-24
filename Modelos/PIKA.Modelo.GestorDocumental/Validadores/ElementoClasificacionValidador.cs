using FluentValidation;
using Microsoft.Extensions.Localization;
using RepositorioEntidades;
using System;
using System.Collections.Generic;
using System.Text;

namespace PIKA.Modelo.GestorDocumental.Validadores
{
    class ElementoClasificacionValidador : AbstractValidator<ElementoClasificacion>
    {
        public ElementoClasificacionValidador(IStringLocalizer<ElementoClasificacion> localizer)
        {
            RuleFor(x => x.Nombre).NotNull().NotEmpty().MinimumLength(1).MaximumLength(LongitudDatos.Nombre);
            RuleFor(x => x.Clave).NotNull().NotEmpty().MinimumLength(1).MaximumLength(LongitudDatos.Nombre);
            RuleFor(x => x.Posicion).NotNull().NotEmpty().LessThanOrEqualTo(0);
            RuleFor(x => x.CuadroClasifiacionId).NotNull().NotEmpty().MaximumLength(LongitudDatos.GUID);
            RuleFor(x=>x.ElementoClasificacionId).MaximumLength(LongitudDatos.GUID);
            


        }
    }
}
