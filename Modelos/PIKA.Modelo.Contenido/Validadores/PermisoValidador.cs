using System;
using System.Collections.Generic;
using System.Text;
using FluentValidation;
using Microsoft.Extensions.Localization;
using RepositorioEntidades;


namespace PIKA.Modelo.Contenido
{
  public  class PermisoValidador : AbstractValidator<Permiso>
    {
        public PermisoValidador()
        {
            RuleFor(x => x.Leer).NotNull();
            RuleFor(x => x.Escribir).NotNull();
            RuleFor(x => x.Crear).NotNull();
            RuleFor(x => x.Eliminar).NotNull();
        }
    }
}
