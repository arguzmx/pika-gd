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
        /// Archivo de plantilla para la generación del reporte
        /// Es un archivo de word serializado en base 64
        /// </summary>
        public string Plantilla { get; set; }
        // # tamaño LongText de MySQL  requerido


        [NotMapped]
        /// <summary>
        /// El tipo de origen por default es el dominio
        /// </summary>
        public string TipoOrigenDefault => ConstantesModelo.GLOBAL_DOMINIOID;

        /// <summary>
        /// Identificador único del origen por defecto el dominio
        /// </summary>
        public string TipoOrigenId { get; set; }
        // # tamaño GUID requerido INDEXAR con OrigenId

        /// <summary>
        /// Identificador único del objeto contenedor del reporte por default el Id del dominio en sesión
        /// </summary>
        public string OrigenId { get ; set ; }
        // # tamaño GUID requerido INDEXAR con TipoOrigenId

    }
}
