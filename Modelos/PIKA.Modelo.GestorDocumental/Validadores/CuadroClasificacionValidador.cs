using FluentValidation;
using Microsoft.Extensions.Localization;
using RepositorioEntidades;
using System;
using System.Collections.Generic;
using System.Text;

namespace PIKA.Modelo.GestorDocumental.Validadores
{
    public class CuadroClasificacionValidador : AbstractValidator<CuadroClasificacion>
    {
        public CuadroClasificacionValidador(IStringLocalizer<CuadroClasificacion> localizer) {
            RuleFor(x => x.Nombre)
                .NotEmpty()
                .MinimumLength(1)
                .MaximumLength(LongitudDatos.Nombre);
            RuleFor(x => x.TipoOrigenId)
                .NotEmpty();

            RuleFor(x => x.OrigenId)
                .NotEmpty();
        }

    }
}
