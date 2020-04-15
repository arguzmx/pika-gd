using FluentValidation;
using Microsoft.Extensions.Localization;
using RepositorioEntidades;
using System;
using System.Collections.Generic;
using System.Text;

namespace PIKA.Modelo.Metadatos.Validadores
{
   public class PlantillaValidador : AbstractValidator<Plantilla>
    {
        public PlantillaValidador(IStringLocalizer<Plantilla> localizer)
        {
            RuleFor(x => x.Nombre)
                .NotNull().WithMessage(x => localizer["El nombre es obligatorio"])
                .NotEmpty().WithMessage(x => localizer["El nombre es obligatorio"])
                .MinimumLength(1).WithMessage(x => localizer["El nombre debe tener entre {0} y {1} caracteres", 1, LongitudDatos.Nombre])
                .MaximumLength(LongitudDatos.Nombre).WithMessage(x => localizer["El nombre debe tener entre {0} y {1} caracteres", 1, LongitudDatos.Nombre]);
            RuleFor(x => x.TipoOrigenId)
                .NotNull().WithMessage(x => localizer["El tipo origen id es obligatorio"])
                .NotEmpty().WithMessage(x => localizer["El tipo origen id es obligatorio"])
                .MinimumLength(1).WithMessage(x => localizer["El tipo origen id debe tener entre {0} y {1} caracteres", 1, LongitudDatos.Nombre])
                .MaximumLength(LongitudDatos.Nombre).WithMessage(x => localizer["El tipo origen id debe tener entre {0} y {1} caracteres", 1, LongitudDatos.Nombre]);
            
            RuleFor(x => x.OrigenId)
             .NotNull().WithMessage(x => localizer["El origen id es obligatorio"])
             .NotEmpty().WithMessage(x => localizer["El origen id es obligatorio"])
             .MinimumLength(1).WithMessage(x => localizer["El origen id debe tener entre {0} y {1} caracteres", 1, LongitudDatos.Nombre])
             .MaximumLength(LongitudDatos.Nombre).WithMessage(x => localizer["El origen id debe tener entre {0} y {1} caracteres", 1, LongitudDatos.Nombre]);

            RuleFor(x => x.Eliminada)
           .NotNull().WithMessage(x => localizer["El Eliminado es obligatorio"])
           .NotEmpty().WithMessage(x => localizer["El Eliminado es obligatorio"]);

            
        }
    }
}
