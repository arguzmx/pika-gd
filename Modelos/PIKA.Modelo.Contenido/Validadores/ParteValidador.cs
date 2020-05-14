using System;
using System.Collections.Generic;
using System.Text;
using FluentValidation;
using Microsoft.Extensions.Localization;
using RepositorioEntidades;

namespace PIKA.Modelo.Contenido.Validadores
{
    public class ParteValidador : AbstractValidator<Parte>
    {
        public ParteValidador(IStringLocalizer<Parte> localizer)
        {
            RuleFor(x => x.VersionId)
                .NotNull().WithMessage(x => localizer["El id de la versiòn es obligatorio"])
                .NotEmpty().WithMessage(x => localizer["El id de la versiòn es obligatorio"])
                .MinimumLength(1).WithMessage(x => localizer["El id de la versiòn debe tener entre {0} y {1} caracteres", 1, LongitudDatos.GUID])
                .MaximumLength(LongitudDatos.GUID).WithMessage(x => localizer["El id de la versiòn  debe tener entre {0} y {1} caracteres", 1, LongitudDatos.GUID]);

            RuleFor(x => x.ElementoId)
                .NotNull().WithMessage(x => localizer["El elemento id es obligatorio"])
                .NotEmpty().WithMessage(x => localizer["El elemento id es obligatorio"])
                .MinimumLength(1).WithMessage(x => localizer["El elemento id debe tener entre {0} y {1} caracteres", 1, LongitudDatos.GUID])
                .MaximumLength(LongitudDatos.GUID).WithMessage(x => localizer["El elemento id debe tener entre {0} y {1} caracteres", 1, LongitudDatos.GUID]);

            RuleFor(x => x.NombreOriginal)
             .NotNull().WithMessage(x => localizer["El nombre original es obligatorio"])
             .NotEmpty().WithMessage(x => localizer["El  nombre original es obligatorio"])
             .MinimumLength(1).WithMessage(x => localizer["El  nombre original debe tener entre {0} y {1} caracteres", 1, LongitudDatos.Nombre])
             .MaximumLength(LongitudDatos.Nombre).WithMessage(x => localizer["El  nombre original debe tener entre {0} y {1} caracteres", 1, LongitudDatos.Nombre]);

            RuleFor(x => x.TipoMime)
           .NotNull().WithMessage(x => localizer["El tipo mime es obligatorio"])
           .NotEmpty().WithMessage(x => localizer["El tipo mime es obligatorio"])
           .MinimumLength(1).WithMessage(x => localizer["El tipo mime debe tener entre {0} y {1} caracteres", 1, LongitudDatos.MIME])
           .MaximumLength(LongitudDatos.MIME).WithMessage(x => localizer["El tipo mime debe tener entre {0} y {1} caracteres", 1, LongitudDatos.MIME]);

           // RuleFor(x => x.Eliminada)
           //.NotNull().WithMessage(x => localizer["El Eliminado es obligatorio"])
           //.NotEmpty().WithMessage(x => localizer["El Eliminado es obligatorio"]);

            RuleFor(x => x.Indice)
                 .NotNull().WithMessage(x => localizer["El indice es obligatorio"])
                 .NotEmpty().WithMessage(x => localizer["El indice es obligatorio"]);

            RuleFor(x => x.ConsecutivoVolumen)
                .NotNull().WithMessage(x => localizer["El Consecutivo Volumen es obligatorio"])
                .NotEmpty().WithMessage(x => localizer["El Consecutivo Volumen es obligatorio"]);

            RuleFor(x => x.LongitudBytes)
            .NotNull().WithMessage(x => localizer["El Longitud Bytes es obligatorio"])
            .NotEmpty().WithMessage(x => localizer["El Longitud Bytes es obligatorio"]);

        }
    }
}