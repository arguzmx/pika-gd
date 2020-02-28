using System;
using System.Collections.Generic;
using System.Text;

namespace RepositorioEntidades
{

    /// <summary>
    /// Clase base para entidades de un repositorio
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Entidad<T>
    {

        public virtual T Id { get; set; }

    }
}
