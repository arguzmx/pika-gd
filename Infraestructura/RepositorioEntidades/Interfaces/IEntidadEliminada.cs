using System;
using System.Collections.Generic;
using System.Text;

namespace RepositorioEntidades
{

    /// <summary>
    /// Identifica entidades del repositorio marcadas para soft-delete
    /// </summary>
    public interface IEntidadEliminada
    {

        /// <summary>
        /// Indica si la entidad ha sido marcada como eliminada 
        /// </summary>
        bool Eliminada { get; set; }

    }
}
