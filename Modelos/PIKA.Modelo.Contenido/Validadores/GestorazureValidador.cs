using System;
using System.Collections.Generic;
using System.Text;
using FluentValidation;
using Microsoft.Extensions.Localization;
using RepositorioEntidades;

namespace PIKA.Modelo.Contenido
{
    public class GestorazureValidador : AbstractValidator<GestorAzureConfig>
    {
        public GestorazureValidador()
        {

            RuleFor(x => x.Contrasena).NotNull().NotEmpty().MaximumLength(LongitudDatos.Nombre);

            RuleFor(x => x.Endpoint).NotNull().NotEmpty().MaximumLength(LongitudDatos.NombreLargo);

            RuleFor(x => x.Usuario).NotNull().MaximumLength(LongitudDatos.Nombre);

            RuleFor(x => x.VolumenId).NotNull().MaximumLength(LongitudDatos.Nombre);


        }
    }
}
