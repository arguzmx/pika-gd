using System;
using System.Collections.Generic;
using System.Text;

namespace RepositorioEntidades
{

    /// <summary>
    /// Define las propedades necesarias para la identificación electrónica de un elemento físico
    /// </summary>
    public interface IEntidadIdElectronico
    {
        /// <summary>
        /// Codigo de barras lineal o 2D
        /// </summary>
        string CodigoOptico { get; set; }

        /// <summary>
        /// Identificador electrónico tal como RFID
        /// </summary>
        string CodigoElectronico { get; set; }
    }
}
