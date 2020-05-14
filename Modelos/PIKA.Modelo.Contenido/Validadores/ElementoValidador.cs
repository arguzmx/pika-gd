using System;
using System.Collections.Generic;
using System.Text;
using FluentValidation;
using Microsoft.Extensions.Localization;
using RepositorioEntidades;

namespace PIKA.Modelo.Contenido.Validadores
{
   public class ElementoValidador : AbstractValidator<Elemento>
    {
        public ElementoValidador(IStringLocalizer<Elemento> localizer)
        {
            RuleFor(x => x.Nombre)
                .NotNull().WithMessage(x => localizer["El nombre es obligatorio"])
                .NotEmpty().WithMessage(x => localizer["El nombre es obligatorio"])
                .MinimumLength(1).WithMessage(x => localizer["El nombre debe tener entre {0} y {1} caracteres", 1, LongitudDatos.Nombre])
                .MaximumLength(LongitudDatos.Nombre).WithMessage(x => localizer["El nombre debe tener entre {0} y {1} caracteres", 1, LongitudDatos.Nombre]);
           
            RuleFor(x => x.TipoOrigenId)
                .NotNull().WithMessage(x => localizer["El tipo origen id es obligatorio"])
                .NotEmpty().WithMessage(x => localizer["El tipo origen id es obligatorio"])
                .MinimumLength(1).WithMessage(x => localizer["El tipo origen id debe tener entre {0} y {1} caracteres", 1, LongitudDatos.GUID])
                .MaximumLength(LongitudDatos.GUID).WithMessage(x => localizer["El tipo origen id debe tener entre {0} y {1} caracteres", 1, LongitudDatos.GUID]);

            RuleFor(x => x.OrigenId)
             .NotNull().WithMessage(x => localizer["El origen id es obligatorio"])
             .NotEmpty().WithMessage(x => localizer["El origen id es obligatorio"])
             .MinimumLength(1).WithMessage(x => localizer["El origen id debe tener entre {0} y {1} caracteres", 1, LongitudDatos.GUID])
             .MaximumLength(LongitudDatos.GUID).WithMessage(x => localizer["El origen id debe tener entre {0} y {1} caracteres", 1, LongitudDatos.GUID]);

            RuleFor(x => x.CreadorId)
           .NotNull().WithMessage(x => localizer["El Creador id es obligatorio"])
           .NotEmpty().WithMessage(x => localizer["El Creador id es obligatorio"])
           .MinimumLength(1).WithMessage(x => localizer["El Creador id debe tener entre {0} y {1} caracteres", 1, LongitudDatos.GUID])
           .MaximumLength(LongitudDatos.GUID).WithMessage(x => localizer["El Creador id debe tener entre {0} y {1} caracteres", 1, LongitudDatos.GUID]);

           // RuleFor(x => x.Eliminada)
           //.NotNull().WithMessage(x => localizer["El Eliminado es obligatorio"])
           //.NotEmpty().WithMessage(x => localizer["El Eliminado es obligatorio"]);

           RuleFor(x => x.FechaCreacion)
                .NotNull().WithMessage(x => localizer["La fecha de creaciòn es obligatorio"])
                .NotEmpty().WithMessage(x => localizer["La fecha de creaciòn es obligatorio"]);

        }
    }
}
