using FluentValidation;
using Microsoft.Extensions.Localization;
using RepositorioEntidades;
using System;
using System.Collections.Generic;
using System.Text;

namespace PIKA.Modelo.GestorDocumental.Validadores
{
    public class ComentarioTransferenciaValidador : AbstractValidator<ComentarioTransferencia>
    {
        public ComentarioTransferenciaValidador(IStringLocalizer<ComentarioTransferencia> localizer)
        {
            RuleFor(x => x.Comentario)
                .NotNull().WithMessage(x => localizer["El comentario es obligatorio"])
                .NotEmpty().WithMessage(x => localizer["El comentario es obligatorio"])
                .MinimumLength(1).WithMessage(x => localizer["El nombre debe tener entre {0} y {1} caracteres", 1, LongitudDatos.Nombre])
                .MaximumLength(LongitudDatos.Nombre).WithMessage(x => localizer["El nombre debe tener entre {0} y {1} caracteres", 1, LongitudDatos.Nombre]);

            RuleFor(x => x.TransferenciaId)
                .NotNull().WithMessage(x => localizer["La transferencia es obligatoria"])
                .NotEmpty().WithMessage(x => localizer["La transferencia es obligatoria"])
                .MinimumLength(1).WithMessage(x => localizer["La transferencia  debe tener entre {0} y {1} caracteres", 1, LongitudDatos.GUID])
                .MaximumLength(LongitudDatos.GUID).WithMessage(x => localizer["La transferencia debe tener entre {0} y {1} caracteres", 1, LongitudDatos.GUID]);


            RuleFor(x => x.Fecha)
                .NotNull().WithMessage(x => localizer["El archivo origen es obligatorio"])
                .NotEmpty().WithMessage(x => localizer["El archivo  origen es obligatorio"]);
               

            RuleFor(x => x.Comentario)
                .NotNull().WithMessage(x => localizer["El comentario es obligatorio"])
                .NotEmpty().WithMessage(x => localizer["El comentario es obligatorio"])
                .MinimumLength(1).WithMessage(x => localizer["El comentario debe tener entre {0} y {1} caracteres", 1, 2048])
                .MaximumLength(2048).WithMessage(x => localizer["El comentario debe tener entre {0} y {1} caracteres", 1, 2048]);

            RuleFor(x => x.UsuarioId)
               .NotNull().WithMessage(x => localizer["El usuario es obligatorio"])
               .NotEmpty().WithMessage(x => localizer["El usuario es obligatorio"])
               .MinimumLength(1).WithMessage(x => localizer["El usuario debe tener entre {0} y {1} caracteres", 1, LongitudDatos.GUID])
               .MaximumLength(LongitudDatos.GUID).WithMessage(x => localizer["El usuario debe tener entre {0} y {1} caracteres", 1, LongitudDatos.GUID]);

        }
    }
}
