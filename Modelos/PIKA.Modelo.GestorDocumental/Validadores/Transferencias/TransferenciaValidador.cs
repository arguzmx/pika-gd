using FluentValidation;
using Microsoft.Extensions.Localization;
using RepositorioEntidades;
using System;
using System.Collections.Generic;
using System.Text;

namespace PIKA.Modelo.GestorDocumental.Validadores
{
    public class TransferenciaValidador : AbstractValidator<Transferencia>
    {
        public TransferenciaValidador(IStringLocalizer<Transferencia> localizer)
        {
            RuleFor(x => x.Nombre)
                .NotNull().WithMessage(x => localizer["El nombre es obligatorio"])
                .NotEmpty().WithMessage(x => localizer["El nombre es obligatorio"])
                .MinimumLength(1).WithMessage(x => localizer["El nombre debe tener entre {0} y {1} caracteres", 1, LongitudDatos.Nombre])
                .MaximumLength(LongitudDatos.Nombre).WithMessage(x => localizer["El nombre debe tener entre {0} y {1} caracteres", 1, LongitudDatos.Nombre]);

            RuleFor(x => x.EstadoTransferenciaId)
                .NotNull().WithMessage(x => localizer["El estado de Transferencia es obligatorio"])
                .NotEmpty().WithMessage(x => localizer["El estado de Transferencia es obligatorio"]);

            RuleFor(x => x.ArchivoOrigenId)
                .NotNull().WithMessage(x => localizer["El archivo origen es obligatorio"])
                .NotEmpty().WithMessage(x => localizer["El archivo  origen es obligatorio"])
                .MinimumLength(1).WithMessage(x => localizer["El archivo origen  debe tener entre {0} y {1} caracteres", 1, LongitudDatos.GUID])
                .MaximumLength(LongitudDatos.GUID).WithMessage(x => localizer["El archivo origen debe tener entre {0} y {1} caracteres", 1, LongitudDatos.GUID]);

            RuleFor(x => x.ArchivoDestinoId)
                .NotNull().WithMessage(x => localizer["El archivo destino es obligatorio"])
                .NotEmpty().WithMessage(x => localizer["El archivo  destino es obligatorio"])
                .MinimumLength(1).WithMessage(x => localizer["El archivo destino  debe tener entre {0} y {1} caracteres", 1, LongitudDatos.GUID])
                .MaximumLength(LongitudDatos.GUID).WithMessage(x => localizer["El archivo destino debe tener entre {0} y {1} caracteres", 1, LongitudDatos.GUID]);

            RuleFor(x => x.UsuarioId)
               .NotNull().WithMessage(x => localizer["El usuario es obligatorio"])
               .NotEmpty().WithMessage(x => localizer["El usuario es obligatorio"])
               .MinimumLength(1).WithMessage(x => localizer["El usuario debe tener entre {0} y {1} caracteres", 1, LongitudDatos.GUID])
               .MaximumLength(LongitudDatos.GUID).WithMessage(x => localizer["El usuario debe tener entre {0} y {1} caracteres", 1, LongitudDatos.GUID]);

        }
    }
}
