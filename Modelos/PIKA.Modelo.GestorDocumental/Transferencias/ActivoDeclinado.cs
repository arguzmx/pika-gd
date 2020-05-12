using System;
using System.Collections.Generic;
using System.Text;

namespace PIKA.Modelo.GestorDocumental
{

    /// <summary>
    /// Cuando un activo sea declinado para una trasnferencia debe indicarse utilizando esta entidad
    /// </summary>
    public class ActivoDeclinado
    {
        //LA CLAVE PRIMARIA PARA ESTA ENTIDAD SON LOS DOS IDS, ELIMINAR ESTE COMETARIO

        /// <summary>
        /// Identificador único del activo
        /// </summary>
        public string ActivoId { get; set; }

        /// <summary>
        /// Identificador único de la trasnfernecia
        /// </summary>
        public string TransferenciaId { get; set; }

        public string Motivo { get; set; }
        // Obligatoriuo 2048

        public Activo Activo { get; set; }
        public Transferencia Transferencia { get; set; }
    }
}
