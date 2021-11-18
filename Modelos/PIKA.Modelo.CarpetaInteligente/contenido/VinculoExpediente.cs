using System;
using System.Collections.Generic;
using System.Text;

namespace PIKA.Modelo.CarpetaInteligente
{
    public class VinculoExpediente: ContenidoBase
    {
        /// <summary>
        /// Identificador único del expediente que se asocia en la liga
        /// </summary>
        public Guid ExpedienteId { get; set; }

        /// <summary>
        /// Mínimo de cardinalidad para completez, debe ser mayor o igual a cero.
        /// El mínimo de cardinalidad determina la condición de completez de la entidad
        /// 0 = Indica que elemento es opcional
        /// </summary>
        public int MinimoCardinalidad { get; set; }

        /// <summary>
        /// Máximo de cardinalidad para completez, debe ser mayor al MinimoCardinalidad.
        /// Establece un límite para la cantidad de expedientes de este tipo en la carpeta
        /// 0 = Ilimitado
        /// </summary>
        public int MaximoCardinalidad { get; set; }

        /// <summary>
        /// Especifica si los datos del expediente deben almacenarse en la carpeta inteligente.
        /// El valor true determina que los datos será tomados desde la instancia del expediente 
        /// </summary>
        public bool EsLigaSimbolica { get; set; }
    }
}
