using System;
using System.Collections.Generic;
using System.Text;

namespace RepositorioEntidades
{

    /// <summary>
    /// Esta interfaz permite relacionar la entidad con una entidad padre a través de su identificador
    /// </summary>
    public interface IEntidadRelacionada
    {

        /// <summary>
        /// Identificador de tipo de origen por default
        /// </summary>
        string TipoOrigenDefault { get; }

        /// <summary>
        /// Especifica el tipo de entidad origen al que pertenece la relación
        /// </summary>
        string TipoOrigenId { get; set; }

        /// <summary>
        /// Identificador del registro orígen de la relación
        /// </summary>
        string OrigenId { get; set; }

    }
}
