using PIKA.Modelo.Metadatos.Atributos;
using System;
using System.Collections.Generic;
using System.Text;

namespace PIKA.Modelo.Metadatos
{
    public class AtributoVistaUI
    {

        public string PropiedadId { get; set; }

        /// <summary>
        ///  Tipo de control en base a las contastntes de COntrolI
        /// </summary>
        public string Control { get; set; }

        /// <summary>es _Accion;
        /// Accion para la cual se encuentra destiando el desplieguie de UI
        /// </summary>
        public Acciones Accion { get; set; }
    }
}
