using System;
using System.Collections.Generic;
using System.Text;
using FluentValidation;
using Microsoft.Extensions.Localization;
using RepositorioEntidades;

namespace PIKA.Modelo.Contenido
{
   public class ElementoValidador : AbstractValidator<Elemento>
    {
        public ElementoValidador()
        {
            RuleFor(x => x.Nombre)
             .NotNull().NotEmpty()
             .MinimumLength(1).MaximumLength(LongitudDatos.Nombre);

            RuleFor(x => x.PuntoMontajeId).NotNull().NotEmpty().MaximumLength(LongitudDatos.GUID);
                        
        }
    }
}
