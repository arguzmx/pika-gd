using System;
using System.Collections.Generic;
using System.Text;

namespace PIKA.Modelo.CarpetaInteligente
{
    public interface ICaducible
    {
        /// <summary>
        /// Especifica el tipo de caducidad
        /// </summary>
        TipoPeriodicidad TipoCaducidad { get; set; }

        /// <summary>
        /// Unidad de medida para el intevalo de caducidad
        /// </summary>
        UnidadesTiempo UnidadesTiempo { get; set; }

        /// <summary>
        /// Número de unidades del intervalo para calcular la caducidad
        /// </summary>
        int IntervaloCaducidad { get; set; }

        /// <summary>
        /// Especifica que en cada aactualización de caducidad, el intervalo comienza de nuevo
        /// </summary>
        bool CaducidadCiclica { get; set; }
    }
}
