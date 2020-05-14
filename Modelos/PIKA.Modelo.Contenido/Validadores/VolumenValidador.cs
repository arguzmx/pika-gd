using System;
using System.Collections.Generic;
using System.Text;
using FluentValidation;
using Microsoft.Extensions.Localization;
using RepositorioEntidades;

namespace PIKA.Modelo.Contenido.Validadores
{
    public class VolumenValidador : AbstractValidator<Volumen>
    {
        public VolumenValidador(IStringLocalizer<Volumen> localizer)
        {
            RuleFor(x => x.Nombre)
              .NotNull().WithMessage(x => localizer["El nombre es obligatorio"])
              .NotEmpty().WithMessage(x => localizer["El nombre es obligatorio"])
              .MinimumLength(1).WithMessage(x => localizer["El nombre debe tener entre {0} y {1} caracteres", 1, LongitudDatos.Nombre])
              .MaximumLength(LongitudDatos.Nombre).WithMessage(x => localizer["El nombre debe tener entre {0} y {1} caracteres", 1, LongitudDatos.Nombre]);

            RuleFor(x => x.TipoOrigenId)
                .NotNull().WithMessage(x => localizer["El tipo origen id es obligatorio"])
                .NotEmpty().WithMessage(x => localizer["El tipo origen id es obligatorio"])
                .MinimumLength(1).WithMessage(x => localizer["El tipo origen id debe tener entre {0} y {1} caracteres", 1, LongitudDatos.GUID])
                .MaximumLength(LongitudDatos.GUID).WithMessage(x => localizer["El tipo origen id debe tener entre {0} y {1} caracteres", 1, LongitudDatos.GUID]);

            RuleFor(x => x.OrigenId)
             .NotNull().WithMessage(x => localizer["El origen id es obligatorio"])
             .NotEmpty().WithMessage(x => localizer["El origen id es obligatorio"])
             .MinimumLength(1).WithMessage(x => localizer["El origen id debe tener entre {0} y {1} caracteres", 1, LongitudDatos.GUID])
             .MaximumLength(LongitudDatos.GUID).WithMessage(x => localizer["El origen id debe tener entre {0} y {1} caracteres", 1, LongitudDatos.GUID]);

            RuleFor(x => x.TipoGestorESId)
           .NotNull().WithMessage(x => localizer["El tipo gestor id es obligatorio"])
           .NotEmpty().WithMessage(x => localizer["El tipo gestor id es obligatorio"])
           .MinimumLength(1).WithMessage(x => localizer["El tipo gestor id debe tener entre {0} y {1} caracteres", 1, LongitudDatos.GUID])
           .MaximumLength(LongitudDatos.GUID).WithMessage(x => localizer["El tipo gestor id debe tener entre {0} y {1} caracteres", 1, LongitudDatos.GUID]);

           // RuleFor(x => x.Eliminada)
           //.NotNull().WithMessage(x => localizer["El Eliminado es obligatorio"])
           //.NotEmpty().WithMessage(x => localizer["El Eliminado es obligatorio"]);

            RuleFor(x => x.Tamano)
                 .NotNull().WithMessage(x => localizer["El tamaño es obligatorio"])
                 .NotEmpty().WithMessage(x => localizer["El tamaño de creaciòn es obligatorio"]);
          
            RuleFor(x => x.CanidadPartes)
            .NotNull().WithMessage(x => localizer["La cantidad parte es obligatorio"])
            .NotEmpty().WithMessage(x => localizer["La cantidad parte es obligatorio"]);


            RuleFor(x => x.ConsecutivoVolumen)
            .NotNull().WithMessage(x => localizer["El consecutivo del volumen es obligatorio"])
            .NotEmpty().WithMessage(x => localizer["El consecutivo del volumen es obligatorio"]);

            RuleFor(x => x.CadenaConexion)
           .NotNull().WithMessage(x => localizer["La cadena de conexion es obligatorio"])
           .NotEmpty().WithMessage(x => localizer["La cadena de conexion es obligatorio"])
           .MinimumLength(1).WithMessage(x => localizer["La cadena de conexion debe tener entre {0} y {1} caracteres", 1, LongitudDatos.GUID])
           .MaximumLength(LongitudDatos.GUID).WithMessage(x => localizer["La cadena de conexion debe tener entre {0} y {1} caracteres", 1, LongitudDatos.GUID]);

            RuleFor(x => x.Activo)
                  .NotNull().WithMessage(x => localizer["El activo es obligatorio"])
                  .NotEmpty().WithMessage(x => localizer["El activo es obligatorio"]);
            RuleFor(x => x.EscrituraHabilitada)
                  .NotNull().WithMessage(x => localizer["La escritura habilitada es obligatorio"])
                  .NotEmpty().WithMessage(x => localizer["La escritura habilitada es obligatorio"]);

        }
    }
}
