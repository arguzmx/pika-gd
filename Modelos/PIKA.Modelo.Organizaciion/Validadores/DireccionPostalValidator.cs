using FluentValidation;
using Microsoft.Extensions.Localization;
using RepositorioEntidades;
using System;
using System.Collections.Generic;
using System.Text;

namespace PIKA.Modelo.Organizacion.Validadores
{
    public class DireccionPostalValidator : AbstractValidator<DireccionPostal>
    {
        public DireccionPostalValidator(IStringLocalizer<DireccionPostal> localizer)
        {
            RuleFor(x => x.Nombre)
                .NotNull().WithMessage(x => localizer["El nombre es obligatorio"])
                .NotEmpty().WithMessage(x => localizer["El nombre es obligatorio"])
                .MinimumLength(1).WithMessage(x => localizer["El nombre debe tener entre {0} y {1} caracteres", 1, LongitudDatos.Nombre])
                .MaximumLength(LongitudDatos.Nombre).WithMessage(x => localizer["El nombre debe tener entre {0} y {1} caracteres", 1, LongitudDatos.Nombre]);

            RuleFor(x => x.Calle)
               .NotNull().WithMessage(x => localizer["La calle es obligatoria"])
               .NotEmpty().WithMessage(x => localizer["La calle es obligatoria"])
               .MinimumLength(1).WithMessage(x => localizer["La calle debe tener entre {0} y {1} caracteres", 1, LongitudDatos.Nombre])
               .MaximumLength(LongitudDatos.Nombre).WithMessage(x => localizer["La calle debe tener entre {0} y {1} caracteres", 1, LongitudDatos.Nombre]);

            RuleFor(x => x.NoInterno)
               .NotNull().WithMessage(x => localizer["El número interior es obligatorio"])
               .NotEmpty().WithMessage(x => localizer["El número interior es obligatorio"])
               .MinimumLength(1).WithMessage(x => localizer["El número interior debe tener entre {0} y {1} caracteres", 1, LongitudDatos.Nombre])
               .MaximumLength(LongitudDatos.Nombre).WithMessage(x => localizer["El número interior debe tener entre {0} y {1} caracteres", 1, LongitudDatos.Nombre]);

            RuleFor(x => x.Colonia)
               .NotNull().WithMessage(x => localizer["La colonia es obligatoria"])
               .NotEmpty().WithMessage(x => localizer["La colonia es obligatoria"])
               .MinimumLength(1).WithMessage(x => localizer["La colonia debe tener entre {0} y {1} caracteres", 1, LongitudDatos.Nombre])
               .MaximumLength(LongitudDatos.Nombre).WithMessage(x => localizer["La colonia debe tener entre {0} y {1} caracteres", 1, LongitudDatos.Nombre]);

            RuleFor(x => x.CP)
               .NotNull().WithMessage(x => localizer["El código postal es obligatorio"])
               .NotEmpty().WithMessage(x => localizer["El código postal es obligatorio"])
               .MinimumLength(1).WithMessage(x => localizer["El código postal debe tener entre {0} y {1} caracteres", 1, LongitudDatos.CodigoPostal])
               .MaximumLength(LongitudDatos.CodigoPostal).WithMessage(x => localizer["El código postal debe tener entre {0} y {1} caracteres", 1, LongitudDatos.CodigoPostal]);

            RuleFor(x => x.Municipio)
               .NotNull().WithMessage(x => localizer["El municipio es obligatorio"])
               .NotEmpty().WithMessage(x => localizer["El municipio es obligatorio"])
               .MinimumLength(1).WithMessage(x => localizer["El municipio debe tener entre {0} y {1} caracteres", 1, LongitudDatos.Nombre])
               .MaximumLength(LongitudDatos.Nombre).WithMessage(x => localizer["El municipio debe tener entre {0} y {1} caracteres", 1, LongitudDatos.Nombre]);

            RuleFor(x => x.EstadoId)
               .NotNull().WithMessage(x => localizer["El estado es obligatorio"])
               .NotEmpty().WithMessage(x => localizer["El estado es obligatorio"])
               .MinimumLength(1).WithMessage(x => localizer["El estado debe tener entre {0} y {1} caracteres", 1, LongitudDatos.Nombre])
               .MaximumLength(LongitudDatos.GUID).WithMessage(x => localizer["El estado debe tener entre {0} y {1} caracteres", 1, LongitudDatos.Nombre]);

            RuleFor(x => x.PaisId)
               .NotNull().WithMessage(x => localizer["El país es obligatorio"])
               .NotEmpty().WithMessage(x => localizer["El país es obligatorio"])
               .MinimumLength(1).WithMessage(x => localizer["El país debe tener entre {0} y {1} caracteres", 1, LongitudDatos.Nombre])
               .MaximumLength(LongitudDatos.GUID).WithMessage(x => localizer["El país debe tener entre {0} y {1} caracteres", 1, LongitudDatos.Nombre]);

        }
    }
}
