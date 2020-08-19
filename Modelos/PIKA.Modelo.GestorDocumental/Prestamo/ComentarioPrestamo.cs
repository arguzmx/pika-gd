using RepositorioEntidades;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;
using System.Xml.Serialization;

namespace PIKA.Modelo.GestorDocumental
{
    public class ComentarioPrestamo : Entidad<string>
    {


        /// <summary>
        /// FEcha de creación del comentario
        /// </summary>
        public DateTime Fecha { get; set; }

        /// <summary>
        /// TExto del Comentario
        /// </summary>
        public string  Comentario { get; set; }
        //# lungitod 2048


        /// <summary>
        /// Identificador del préstamo al que pertenece el comentario
        /// </summary>
        public string PrestamoId { get; set; }


        /// <summary>
        /// Préstamo al que pertenece el comentario
        /// </summary>
        [XmlIgnore]
        [JsonIgnore]
        public Prestamo Prestamo { get; set; }


    }
}
