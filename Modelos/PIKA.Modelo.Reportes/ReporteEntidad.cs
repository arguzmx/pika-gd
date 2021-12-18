using PIKA.Infraestructura.Comun;
using RepositorioEntidades;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace PIKA.Modelo.Reportes
{
    public class ReporteEntidad : Entidad<String>, IEntidadNombrada, IEntidadRelacionada
    {
        /// <summary>
        /// Identificador único del reporte
        /// </summary>
        public override string Id { get => base.Id; set => base.Id = value; }

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


        [NotMapped]
        /// <summary>
        /// El tipo de origen por default es el dominio
        /// </summary>
        public string TipoOrigenDefault => ConstantesModelo.GLOBAL_DOMINIOID;

        /// <summary>
        /// Dominio del reporte, default = * significa cualquier dominio se utiliza en ausencia del específico
        /// </summary>
        public string TipoOrigenId { get; set; }

        /// <summary>
        /// Unidad organizacional del reporte, default = * significa cualquier UO se utiliza en ausencia del específico
        /// </summary>
        public string OrigenId { get ; set ; }

    }
}
