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
            RuleFor(x => x.PropiedadId)
                .NotNull().WithMessage(x => localizer["El id propiedad es obligatorio"])
                .NotEmpty().WithMessage(x => localizer["El id propiedad es obligatorio"])
                .MinimumLength(1).WithMessage(x => localizer["El id propiedad debe tener entre {0} y {1} caracteres", 1, LongitudDatos.Nombre])
                .MaximumLength(LongitudDatos.Nombre).WithMessage(x => localizer["El id propiedad debe tener entre {0} y {1} caracteres", 1, LongitudDatos.Nombre]);
          
            RuleFor(x => x.IdTablaCliente)
                            .NotNull().WithMessage(x => localizer["El id tabla cliente es obligatorio"])
                            .NotEmpty().WithMessage(x => localizer["El id tabla cliente es obligatorio"])
                            .MinimumLength(1).WithMessage(x => localizer["El id tabla cliente debe tener entre {0} y {1} caracteres", 1, LongitudDatos.Nombre])
                            .MaximumLength(LongitudDatos.Nombre).WithMessage(x => localizer["El id tabla cliente debe tener entre {0} y {1} caracteres", 1, LongitudDatos.Nombre]);

            RuleFor(x => x.Incluir)
             .NotNull().WithMessage(x => localizer["El incluir es obligatoria"])
             .NotEmpty().WithMessage(x => localizer["El incluir es obligatorio"]);

            RuleFor(x => x.Visible)
           .NotNull().WithMessage(x => localizer["El visible es obligatoria"])
           .NotEmpty().WithMessage(x => localizer["El visible es obligatorio"]);

            RuleFor(x => x.Alternable)
           .NotNull().WithMessage(x => localizer["El alternable es obligatoria"])
           .NotEmpty().WithMessage(x => localizer["El alternable es obligatorio"]);

        }
    }

}
