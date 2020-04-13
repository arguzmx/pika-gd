using RepositorioEntidades;
using System;
using System.Collections.Generic;
using System.Text;

namespace PIKA.Modelo.GestorDocumental
{

    public class HistorialArchivoActivo: Entidad<string>
    {
        public override string Id { get; set; }

        /// <summary>
        /// Indetificador único del archivo
        /// </summary>
        public string ActivoId { get; set; }

        /// <summary>
        /// Identificador del archivo donde se licaliza el activo
        /// </summary>
        public string ArchivoId { get; set; }

        /// <summary>
        /// Fecha de ingreso del activo al archiv
        /// </summary>
        public DateTime FechaIngreso { get; set; }

        /// <summary>
        /// FEcha de egreso del activo del archivo
        /// </summary>
        public DateTime? FechaEgreso { get; set; }


        public Archivo Archivo { get; set; }

        public Activo Activo { get; set; }

    }
}
