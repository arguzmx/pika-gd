using System;
using System.Collections.Generic;
using System.Text;
using FluentValidation;
using Microsoft.Extensions.Localization;
using RepositorioEntidades;

namespace PIKA.Modelo.Contenido.Validadores
{
  public  class TipoGestorESValidador : AbstractValidator<TipoGestorES>
    {
        public TipoGestorESValidador(IStringLocalizer<TipoGestorES> localizer)
        {
            RuleFor(x => x.Nombre)
                .NotNull().WithMessage(x => localizer["El nombre es obligatorio"])
                .NotEmpty().WithMessage(x => localizer["El nombre es obligatorio"])
                .MinimumLength(1).WithMessage(x => localizer["El nombre debe tener entre {0} y {1} caracteres", 1, LongitudDatos.Nombre])
                .MaximumLength(LongitudDatos.Nombre).WithMessage(x => localizer["El nombre debe tener entre {0} y {1} caracteres", 1, LongitudDatos.Nombre]);

            //RuleFor(x => x.Volumenid)
            //    .NotNull().WithMessage(x => localizer["El volumen id es obligatorio"])
            //    .NotEmpty().WithMessage(x => localizer["El volumen id es obligatorio"])
            //    .MinimumLength(1).WithMessage(x => localizer["El volumen id debe tener entre {0} y {1} caracteres", 1, LongitudDatos.GUID])
            //    .MaximumLength(LongitudDatos.GUID).WithMessage(x => localizer["El volumen id debe tener entre {0} y {1} caracteres", 1, LongitudDatos.GUID]);

          

        }
    }
}
