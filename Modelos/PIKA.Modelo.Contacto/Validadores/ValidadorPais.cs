﻿using FluentValidation;
using Microsoft.Extensions.Localization;
using RepositorioEntidades;
using System;
using System.Collections.Generic;
using System.Text;

namespace PIKA.Modelo.Contacto
{

    
    public class ValidadorPais: AbstractValidator<Pais>
    {
        public ValidadorPais(IStringLocalizer<Pais> localizer) {
            RuleFor(x => x.Nombre).NotNull().NotEmpty().MinimumLength(1).MaximumLength(LongitudDatos.Nombre);
            RuleFor(x => x.Id).NotNull().NotEmpty().MinimumLength(1).MaximumLength(LongitudDatos.GUID);
        }
    }
}
