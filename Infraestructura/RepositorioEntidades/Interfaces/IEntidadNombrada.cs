using System;
using System.Collections.Generic;
using System.Text;

namespace RepositorioEntidades
{
    /// <summary>
    /// Caractereiza una entidad que tiene asociado un nombre que la hace reconocible
    /// </summary>
    public interface IEntidadNombrada
    {

        /// <summary>
        /// Nombre para la entidad
        /// </summary>
        string Nombre { get; set; }

    }
}
