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
            RuleFor(x => x.Nombre).NotEmpty().MinimumLength(1).MaximumLength(LongitudDatos.Nombre);
            RuleFor(x => x.TipoDatoId).NotEmpty().MinimumLength(1).MaximumLength(LongitudDatos.Nombre);
            RuleFor(x => x.Requerido).NotEmpty();
            //RuleFor(x => x.Buscable).NotEmpty();
            //RuleFor(x => x.Ordenable).NotEmpty();
            //RuleFor(x => x.Visible).NotEmpty();
            //RuleFor(x => x.EsIndice).NotEmpty();
            // RuleFor(x => x.IndiceOrdenamiento).NotEmpty();
            //RuleFor(x => x.EsIdClaveExterna).NotEmpty();
            //RuleFor(x => x.EsIdRegistro).NotEmpty();
            //RuleFor(x => x.EsIdJerarquia).NotEmpty();
            //RuleFor(x => x.EsIdRaizJerarquia).NotEmpty();
            //RuleFor(x => x.EsFiltroJerarquia).NotEmpty();
            //RuleFor(x => x.ControlHTML).NotEmpty().MinimumLength(1).MaximumLength(LongitudDatos.Nombre);

        }
    }
}
