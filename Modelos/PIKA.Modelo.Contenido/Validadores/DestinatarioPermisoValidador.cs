using System;
using System.Collections.Generic;
using System.Text;
using FluentValidation;
using Microsoft.Extensions.Localization;
using RepositorioEntidades;

namespace PIKA.Modelo.Contenido
{
  public  class DestinatarioPermisoValidador : AbstractValidator<DestinatarioPermiso>
    {
        public DestinatarioPermisoValidador()
        {
            RuleFor(x => x.PermisoId).NotEmpty().NotNull().MaximumLength(LongitudDatos.GUID);
            RuleFor(x => x.DestinatarioId).NotEmpty().NotNull().MaximumLength(LongitudDatos.GUID);
            RuleFor(x => x.EsGrupo).NotNull();
        }
    }
}
