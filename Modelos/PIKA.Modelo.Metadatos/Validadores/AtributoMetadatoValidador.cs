using FluentValidation;
using Microsoft.Extensions.Localization;
using RepositorioEntidades;
using System;
using System.Collections.Generic;
using System.Text;

namespace PIKA.Modelo.Metadatos.Validadores
{
  public  class AtributoMetadatoValidador : AbstractValidator<AtributoMetadato>
    {
        public AtributoMetadatoValidador(IStringLocalizer<AtributoMetadato> localizer)
        {
            RuleFor(x => x.PropiedadId)
                .NotNull().WithMessage(x => localizer["El id propiedad es obligatorio"])
                .NotEmpty().WithMessage(x => localizer["El id propiedad es obligatorio"])
                .MinimumLength(1).WithMessage(x => localizer["El id propiedad debe tener entre {0} y {1} caracteres", 1, LongitudDatos.Nombre])
                .MaximumLength(LongitudDatos.Nombre).WithMessage(x => localizer["El id propiedad debe tener entre {0} y {1} caracteres", 1, LongitudDatos.Nombre]);
            
            RuleFor(x => x.Id)
                  .NotNull().WithMessage(x => localizer["El id es obligatorio"])
                  .NotEmpty().WithMessage(x => localizer["El id es obligatorio"])
                  .MinimumLength(1).WithMessage(x => localizer["El id debe tener entre {0} y {1} caracteres", 1, LongitudDatos.Nombre])
                  .MaximumLength(LongitudDatos.Nombre).WithMessage(x => localizer["El id debe tener entre {0} y {1} caracteres", 1, LongitudDatos.Nombre]);


        }
    }
}
