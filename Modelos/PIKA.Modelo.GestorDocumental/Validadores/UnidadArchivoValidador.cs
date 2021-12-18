using FluentValidation;
using Microsoft.Extensions.Localization;
using RepositorioEntidades;
using System;
using System.Collections.Generic;
using System.Text;

namespace PIKA.Modelo.GestorDocumental.Validadores
{
    public class UnidadArchivoValidador :  AbstractValidator<UnidadAdministrativaArchivo>
    {
        public UnidadArchivoValidador(IStringLocalizer<UnidadAdministrativaArchivo> localizer)
        {
            RuleFor(x => x.UnidadAdministrativa).NotEmpty().MinimumLength(2).MaximumLength(LongitudDatos.Nombre);
            RuleFor(x => x.ArchivoTramiteId).NotEmpty();
        }
    }
}
