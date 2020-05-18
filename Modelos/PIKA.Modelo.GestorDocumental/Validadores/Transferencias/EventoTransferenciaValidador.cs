using FluentValidation;
using Microsoft.Extensions.Localization;
using RepositorioEntidades;
using System;
using System.Collections.Generic;
using System.Text;

namespace PIKA.Modelo.GestorDocumental.Validadores
{
    public class EventoTransferenciaValidador : AbstractValidator<EventoTransferencia>
    {
        public EventoTransferenciaValidador(IStringLocalizer<EventoTransferencia> localizer)
        {
            RuleFor(x => x.Fecha)
                .NotNull().WithMessage(x => localizer["La fecha es obligatoria"])
                .NotEmpty().WithMessage(x => localizer["La fecha es obligatoria"]);
               

            RuleFor(x => x.EstadoTransferenciaId)
                .NotNull().WithMessage(x => localizer["El estado de EventoTransferencia es obligatorio"])
                .NotEmpty().WithMessage(x => localizer["El estado de EventoTransferencia es obligatorio"]);


            RuleFor(x => x.TransferenciaId)
                .NotNull().WithMessage(x => localizer["El archivo origen es obligatorio"])
                .NotEmpty().WithMessage(x => localizer["El archivo  origen es obligatorio"])
                .MinimumLength(1).WithMessage(x => localizer["El archivo origen  debe tener entre {0} y {1} caracteres", 1, LongitudDatos.GUID])
                .MaximumLength(LongitudDatos.GUID).WithMessage(x => localizer["El archivo origen debe tener entre {0} y {1} caracteres", 1, LongitudDatos.GUID]);

            RuleFor(x => x.EstadoTransferenciaId)
                .NotNull().WithMessage(x => localizer["El archivo destino es obligatorio"])
                .NotEmpty().WithMessage(x => localizer["El archivo  destino es obligatorio"])
                .MinimumLength(1).WithMessage(x => localizer["El archivo destino  debe tener entre {0} y {1} caracteres", 1, LongitudDatos.GUID])
                .MaximumLength(LongitudDatos.GUID).WithMessage(x => localizer["El archivo destino debe tener entre {0} y {1} caracteres", 1, LongitudDatos.GUID]);

        }
    }
}
