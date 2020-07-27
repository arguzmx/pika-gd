using System;
using System.Collections.Generic;
using System.Text;
using FluentValidation;
using Microsoft.Extensions.Localization;
using RepositorioEntidades;

namespace PIKA.Modelo.Contenido
{
   public class VersionValidador : AbstractValidator<PIKA.Modelo.Contenido.Version>
    {
        public VersionValidador()
        {
            RuleFor(x => x.ElementoId).NotNull().NotEmpty().MaximumLength(LongitudDatos.GUID);
            RuleFor(x => x.CreadorId).NotNull().NotEmpty().MaximumLength(LongitudDatos.GUID)
                .When(x => string.IsNullOrEmpty(x.Id));
           
        }
    }
}
