using System;
using System.Collections.Generic;
using System.Text;
using FluentValidation;
using Microsoft.Extensions.Localization;
using RepositorioEntidades;

namespace PIKA.Modelo.Contenido
{
    public class GestorLocalValidador : AbstractValidator<GestorLocalConfig>
    {
        public GestorLocalValidador()
        {

            RuleFor(x => x.Ruta).NotNull().NotEmpty().MaximumLength(LongitudDatos.NombreLargo);
            RuleFor(x => x.VolumenId).NotNull().MaximumLength(LongitudDatos.Nombre);
        }
    }
}
