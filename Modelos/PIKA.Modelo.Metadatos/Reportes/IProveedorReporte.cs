using System;
using System.Collections.Generic;
using System.Text;

namespace PIKA.Modelo.Metadatos
{
    public interface IProveedorReporte
    {
        /// <summary>
        /// Nombre del reporte
        /// </summary>
        string Nombre { get; set; }

        /// <summary>
        ///  Parametros de entrada para el reporte
        /// </summary>
        List<ParametroReporte> Parametros { get; set; }
        
        /// <summary>
        ///  Url de acceso al reporte
        /// </summary>
        string Url { get; set; }

        /// <summary>
        /// Lista de formatos disponibles del reporte
        /// </summary>
        List<FormatoReporte> FormatosDisponibles { get; set; }


    }
}
