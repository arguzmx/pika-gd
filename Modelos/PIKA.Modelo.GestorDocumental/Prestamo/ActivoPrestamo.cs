using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;
using System.Xml.Serialization;

namespace PIKA.Modelo.GestorDocumental
{
    public class ActivoPrestamo
    {

        /// <summary>
        /// Clave principal
        /// </summary>
        public string PrestamoId { get; set; }

        /// <summary>
        /// Clave principal
        /// </summary>
        public string ActivoId { get; set; }

        /// <summary>
        /// Indica si el elemento de prestamo ha sido devuelto
        /// </summary>
        public bool Devuelto { get; set; }

        /// <summary>
        /// Fecha en la que el elemento fue devuelto en formato UTC
        /// </summary>
        public DateTime? FechaDevolucion { get; set; }

        [XmlIgnore]
        [JsonIgnore]
        public Activo Activo { get; set; }
        [XmlIgnore]
        [JsonIgnore]
        public Prestamo Prestamo { get; set; }

    }
}
