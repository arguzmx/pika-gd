using System;
using System.Collections.Generic;
using System.Text;

namespace PIKA.Infraestrctura.Reportes
{
    public class ElementoReporte
    {
        /// <summary>
        /// Identificador único del reporte
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// IDentificador único de la entidad a la que pertenece el reporte
        /// </summary>
        public string Entidad { get; set; }
        // # INDEXAR tamaño NombreLargo requerido

        /// <summary>
        /// Nómbre del reporte
        /// </summary>
        public string Nombre { get; set; }
        // # tamaño Nombre requerido

        /// <summary>
        /// Descripción del reporte
        /// </summary>
        public string Descripcion { get; set; }
        // # tamaño Descripcion requerido

        /// <summary>
        /// DEtermina si es un subreporte
        /// </summary>
        public bool SubReporte { get; set; }

        /// <summary>
        /// Identificador del reporte padre en el caso de los subreportes
        /// </summary>
        public string GrupoReportes { get; set; }

        /// <summary>
        /// Archivo de plantilla para la generación del reporte
        /// Es un archivo de word serializado en base 64
        /// </summary>
        public string Plantilla { get; set; }

        /// <summary>
        /// Determina si es el reporte puede actualizarse
        /// </summary>
        public bool Bloqueado { get; set; }

        /// <summary>
        /// Formato electronico de salida del reporte en base a la plantilla: DOCX, CSV, PDF, etc.
        /// </summary>
        public string ExtensionSalida { get; set; }
    }
}
