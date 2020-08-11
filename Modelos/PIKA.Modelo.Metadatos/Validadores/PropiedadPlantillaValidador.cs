using FluentValidation;
using Microsoft.Extensions.Localization;
using RepositorioEntidades;
using System;
using System.Collections.Generic;
using System.Text;

namespace PIKA.Modelo.Metadatos.Validadores
{
    public class PropiedadPlantillaValidador : AbstractValidator<PropiedadPlantilla>
    {
        public PropiedadPlantillaValidador(IStringLocalizer<PropiedadPlantilla> localizer)
        {
            RuleFor(x => x.Nombre)
                .NotNull().WithMessage(x => localizer["El nombre es obligatorio"])
                .NotEmpty().WithMessage(x => localizer["El nombre es obligatorio"])
                .MinimumLength(1).WithMessage(x => localizer["El nombre debe tener entre {0} y {1} caracteres", 1, LongitudDatos.Nombre])
                .MaximumLength(LongitudDatos.Nombre).WithMessage(x => localizer["El nombre debe tener entre {0} y {1} caracteres", 1, LongitudDatos.Nombre]);

            RuleFor(x => x.TipoDatoId)
                 .NotNull().WithMessage(x => localizer["El tipo dato id es obligatorio"])
                 .NotEmpty().WithMessage(x => localizer["El tipo dato id  es obligatorio"])
                 .MinimumLength(1).WithMessage(x => localizer["El tipo dato id  debe tener entre {0} y {1} caracteres", 1, LongitudDatos.Nombre])
                 .MaximumLength(LongitudDatos.Nombre).WithMessage(x => localizer["El tipo dato id  debe tener entre {0} y {1} caracteres", 1, LongitudDatos.Nombre]);

           // RuleFor(x => x.ValorDefault)
           //.NotNull().WithMessage(x => localizer["El valor default es obligatoria"])
           //.NotEmpty().WithMessage(x => localizer["El valor default es obligatorio"]);

            RuleFor(x => x.IndiceOrdenamiento)
                .NotNull().WithMessage(x => localizer["El valor indice ordenamiento es obligatorio"])
                .NotEmpty().WithMessage(x => localizer["El indice ordenamiento es obligatorio"]);

            RuleFor(x => x.Buscable)
              .NotNull().WithMessage(x => localizer["El Buscable es obligatorio"])
              .NotEmpty().WithMessage(x => localizer["El Buscable es obligatorio"]);

            RuleFor(x => x.Ordenable)
         .NotNull().WithMessage(x => localizer["El Ordenable es obligatorio"])
         .NotEmpty().WithMessage(x => localizer["El Ordenable es obligatorio"]);


            RuleFor(x => x.Visible)
     .NotNull().WithMessage(x => localizer["El Visible es obligatorio"])
     .NotEmpty().WithMessage(x => localizer["El Visible es obligatorio"]);

            RuleFor(x => x.EsIdClaveExterna)
     .NotNull().WithMessage(x => localizer["El id clave externa es obligatorio"])
     .NotEmpty().WithMessage(x => localizer["El id clave externa es obligatorio"]);

            RuleFor(x => x.EsIdRegistro)
     .NotNull().WithMessage(x => localizer["El id registro es obligatorio"])
     .NotEmpty().WithMessage(x => localizer["El id registro es obligatorio"]);

            RuleFor(x => x.EsIdJerarquia)
     .NotNull().WithMessage(x => localizer["El id jerarquia es obligatorio"])
     .NotEmpty().WithMessage(x => localizer["El id jerarquia es obligatorio"]);

            RuleFor(x => x.EsIdRaizJerarquia)
.NotNull().WithMessage(x => localizer["El id padre de jerarquia es obligatorio"])
.NotEmpty().WithMessage(x => localizer["El id padre de jerarquia es obligatorio"]);

            RuleFor(x => x.EsFiltroJerarquia)
.NotNull().WithMessage(x => localizer["El filtro jerarquia es obligatorio"])
.NotEmpty().WithMessage(x => localizer["El filtro jerarquia es obligatorio"]);

            RuleFor(x => x.Requerido)
.NotNull().WithMessage(x => localizer["El campo requerido es obligatorio"])
.NotEmpty().WithMessage(x => localizer["El campo requerido es obligatorio"]);

            RuleFor(x => x.EsIndice)
.NotNull().WithMessage(x => localizer["El indice es obligatorio"])
.NotEmpty().WithMessage(x => localizer["El indice es obligatorio"]);

            RuleFor(x => x.ControlHTML)
                           .NotNull().WithMessage(x => localizer["El control HTML es obligatorio"])
                           .NotEmpty().WithMessage(x => localizer["El control HTML es obligatorio"])
                           .MinimumLength(1).WithMessage(x => localizer["El control HTML debe tener entre {0} y {1} caracteres", 1, LongitudDatos.Nombre])
                           .MaximumLength(LongitudDatos.Nombre).WithMessage(x => localizer["El control HTML debe tener entre {0} y {1} caracteres", 1, LongitudDatos.Nombre]);


            ;
        }
    }
}
