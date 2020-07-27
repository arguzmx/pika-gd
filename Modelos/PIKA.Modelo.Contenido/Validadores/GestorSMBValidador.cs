using System;
using System.Collections.Generic;
using System.Text;
using FluentValidation;
using Microsoft.Extensions.Localization;
using RepositorioEntidades;

namespace PIKA.Modelo.Contenido
{
    public class GestorSMBValidador : AbstractValidator<GestorSMBConfig>
    {
        public GestorSMBValidador()
        {

            RuleFor(x => x.Contrasena).NotNull().NotEmpty().MaximumLength(LongitudDatos.Nombre);

            RuleFor(x => x.Dominio).NotNull().NotEmpty().MaximumLength(LongitudDatos.Nombre);

            RuleFor(x => x.Ruta).NotNull().NotEmpty().MaximumLength(LongitudDatos.NombreLargo);

            RuleFor(x => x.Usuario).NotNull().MaximumLength(LongitudDatos.Nombre);

            RuleFor(x => x.VolumenId).NotNull().MaximumLength(LongitudDatos.Nombre);


        }
    }
}
