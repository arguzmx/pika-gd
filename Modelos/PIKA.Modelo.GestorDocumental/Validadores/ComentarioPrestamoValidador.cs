using FluentValidation;
using Microsoft.Extensions.Localization;
using RepositorioEntidades;
using System;
using System.Collections.Generic;
using System.Text;

namespace PIKA.Modelo.GestorDocumental.Validadores
{
    public class ComentarioPrestamoValidador : AbstractValidator<ComentarioPrestamo>
    {
        public ComentarioPrestamoValidador(IStringLocalizer<ComentarioPrestamo> localizer)
        {
            RuleFor(x => x.Comentario)
                .NotNull().WithMessage(x => localizer["El comentario es obligatorio"])
                .NotEmpty().WithMessage(x => localizer["El comentario es obligatorio"])
                .MinimumLength(1).WithMessage(x => localizer["El comentario debe tener entre {0} y {1} caracteres", 1, 2048])
                .MaximumLength(2048).WithMessage(x => localizer["El comentario debe tener entre {0} y {1} caracteres", 1, 2048]);
        }
    }
}
