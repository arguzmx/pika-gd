using FluentValidation;
using Microsoft.Extensions.Localization;
using RepositorioEntidades;
using System;
using System.Collections.Generic;
using System.Text;

namespace PIKA.Modelo.Metadatos.Validadores
{
    public class AtributoTablaValidador : AbstractValidator<AtributoTabla>
    {
        public AtributoTablaValidador(IStringLocalizer<AtributoTabla> localizer)
        {
            RuleFor(x => x.PropiedadId).NotEmpty().MinimumLength(1).MaximumLength(LongitudDatos.Nombre);
            RuleFor(x => x.IdTablaCliente).NotEmpty().MinimumLength(1).MaximumLength(LongitudDatos.Nombre);
            RuleFor(x => x.Incluir).NotEmpty();
            RuleFor(x => x.Visible).NotEmpty();
            RuleFor(x => x.Alternable).NotEmpty();

        }
    }

}
