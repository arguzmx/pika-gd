using FluentValidation;
using Microsoft.Extensions.Localization;
using RepositorioEntidades;
using System;
using System.Collections.Generic;
using System.Text;

namespace PIKA.Modelo.GestorDocumental.Validadores
{
    public class PrestamoValidador : AbstractValidator<Prestamo>
    {
        public PrestamoValidador(IStringLocalizer<Prestamo> localizer)
        {
            RuleFor(x => x.Folio)
                .NotNull().When(x=>x.Id ==null).WithMessage(x => localizer["El folio es obligatorio"])
                .NotEmpty().When(x => x.Id == null).WithMessage(x => localizer["El folio es obligatorio"])
                .MinimumLength(1).When(x => x.Id == null).WithMessage(x => localizer["El folio debe tener entre {0} y {1} caracteres", 1, 100])
                .MaximumLength(100).When(x => x.Id == null).WithMessage(x => localizer["El folio debe tener entre {0} y {1} caracteres", 1, 100]);

            RuleFor(x => x.ArchivoId)
                .NotNull().WithMessage(x => localizer["El archivo es obligatorio"])
                .NotEmpty().WithMessage(x => localizer["El archivo es obligatorio"]);

            RuleFor(x => x.FechaProgramadaDevolucion)
                .NotNull().WithMessage(x => localizer["La fecha programada de devolución es obligatoria"])
                .NotEmpty().WithMessage(x => localizer["La fecha programada de devolución es obligatoria"]);
        }
    }
}
