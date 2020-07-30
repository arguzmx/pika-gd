using FluentValidation;
using Microsoft.Extensions.Localization;
using RepositorioEntidades;
using System;
using System.Collections.Generic;
using System.Text;

namespace PIKA.Modelo.GestorDocumental.Validadores
{
public   class EstadoCuadroClasificacionValidador : AbstractValidator<EstadoCuadroClasificacion>
    {
        public EstadoCuadroClasificacionValidador(IStringLocalizer<EstadoCuadroClasificacion> localizer)
        {
            RuleFor(x => x.Nombre)
                .NotEmpty()
                .MinimumLength(1)
                .MaximumLength(LongitudDatos.Nombre);
            RuleFor(x => x.Id)
              .NotEmpty()
              .MinimumLength(1)
              .MaximumLength(LongitudDatos.Nombre);

        }
    }
}
