using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using FluentValidation;
using Microsoft.Extensions.Localization;
using RepositorioEntidades;

namespace PIKA.Modelo.Contenido
{
  public  class CarpetaValidador : AbstractValidator<Carpeta>
    {
        public CarpetaValidador()
        {
            RuleFor(x => x.PuntoMontajeId).NotEmpty().MaximumLength(LongitudDatos.GUID);
            RuleFor(x => x.CreadorId).NotEmpty().MaximumLength(LongitudDatos.GUID);
            RuleFor(x => x.Nombre).NotEmpty().MaximumLength(LongitudDatos.Nombre );
            RuleFor(x => x.CarpetaPadreId).NotEmpty().When(x => x.EsRaiz == false);
            RuleFor(x => x.EsRaiz).NotNull();
        }  
    }
}
