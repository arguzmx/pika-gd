using FluentValidation;
using Microsoft.Extensions.Localization;
using RepositorioEntidades;
using System;
using System.Collections.Generic;
using System.Text;

namespace PIKA.Modelo.GestorDocumental.Validadores
{
    public class ActivoValidador : AbstractValidator<Activo>
    {
        public ActivoValidador(IStringLocalizer<Activo> localizer)
        {
            RuleFor(x => x.Nombre)
                .NotNull().WithMessage(x => localizer["El nombre es obligatorio"])
                .NotEmpty().WithMessage(x => localizer["El nombre es obligatorio"])
                .MinimumLength(1).WithMessage(x => localizer["El nombre debe tener entre {0} y {1} caracteres", 1, LongitudDatos.Nombre])
                .MaximumLength(LongitudDatos.Nombre).WithMessage(x => localizer["El nombre debe tener entre {0} y {1} caracteres", 1, LongitudDatos.Nombre]);

            RuleFor(x => x.Asunto)
                .MaximumLength(2048).WithMessage(x => localizer["El asunto no debe exceder de {0} caracteres", 2048]);

            RuleFor(x => x.FechaApertura)
                .NotNull().WithMessage(x => localizer["La fecha de apertura es obligatorio"])
                .NotEmpty().WithMessage(x => localizer["La fecha de apertura es obligatorio"]);

            RuleFor(x => x.CodigoOptico)
                .MaximumLength(1024).WithMessage(x => localizer["El código óptico no debe exceder de {0} caracteres", 1024]);

            RuleFor(x => x.CodigoElectronico)
                .MaximumLength(1024).WithMessage(x => localizer["El codigo electrónico no debe exceder de {0} caracteres", 1024]);

            RuleFor(x => x.ArchivoId)
                .NotNull().WithMessage(x => localizer["El archivo es obligatorio"])
                .NotEmpty().WithMessage(x => localizer["El archivo es obligatorio"]);

            RuleFor(x => x.ElementoClasificacionId)
                .NotNull().WithMessage(x => localizer["El elemento clasificación es obligatorio"])
                .NotEmpty().WithMessage(x => localizer["El elemento clasificación es obligatorio"]);

            RuleFor(x => x.TipoOrigenId)
                .NotNull().WithMessage(x => localizer["El tipo de origen es obligatorio"])
                .NotEmpty().WithMessage(x => localizer["El tipo de origen es obligatorio"]);

            RuleFor(x => x.OrigenId)
                .NotNull().WithMessage(x => localizer["El origen es obligatorio"])
                .NotEmpty().WithMessage(x => localizer["El origen es obligatorio"]);

        }
    }
}
