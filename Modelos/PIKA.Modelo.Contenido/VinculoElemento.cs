using System;
using System.Collections.Generic;
using System.Text;

namespace PIKA.Modelo.Contenido
{
    /// <summary>
    /// Permite vincular un elemento con un módulo o sistema externo
    /// </summary>
    public class VinculoElemento
    {

        /// Las dos propeidades forman la llave primaria, de este modo sólo
        /// se puede vinvular una sola vez un elemento a un vinculante

        /// <summary>
        /// Identificador único del elemento de contendo
        /// </summary>
        public string ElementoId { get; set; }

        /// <summary>
        /// Identificador único del elemento vinculante (módulo o sistema)
        /// </summary>
        public string VinculanteId { get; set; }


    }
}
