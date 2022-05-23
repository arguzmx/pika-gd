using RepositorioEntidades;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;
using System.Xml.Serialization;

namespace PIKA.Modelo.GestorDocumental
{
    /// <summary>
    /// Define un contenedor para la guarda de los activos del acervo por ejemplo una caja de archivo
    /// </summary>
    public class ContenedorAlmacen : Entidad<string>, IEntidadNombrada
    {

        public ContenedorAlmacen()
        {
            EventosContenedor = new List<EventoContenedorAlmacen>();
        }

        /// <summary>
        /// Nomnbre del contenedor para refeencia humana puede ser el mismo que la clave
        /// </summary>
        public string Nombre { get; set; }

        /// <summary>
        /// Clave asociada al contenedor en la organización
        /// </summary>
        public string Clave { get; set; }

        /// <summary>
        /// Codigo de barras asociado
        /// </summary>
        public string CodigoBarras { get; set; }

        /// <summary>
        /// Codigo electrónico asociado
        /// </summary>
        public string CodigoElectronico { get; set; }

        /// <summary>
        /// Porcentaje de ocupación del contenedor
        /// </summary>
        public decimal Ocupacion { get; set; }

        /// <summary>
        /// Identificaodr único del archivo en el que se ubica el contenedor
        /// </summary>
        public string ArchivoId { get; set; }

        /// <summary>
        /// Identificador unico del almacen en el que se ubica el contenedor
        /// </summary>
        public string AlmacenArchivoId { get; set; }

        /// <summary>
        /// Identificador unico de la zona en el que se ubica el contenedor
        /// La zona del contenedor es opcional
        /// </summary>
        public string ZonaAlmacenId { get; set; }

        /// <summary>
        /// Identificador unico de la posición en la que se ubica el contenedor
        /// EL contenedor puede no estar asociado a una posición puede esta solamente en una zona por ejemplo FUMIGACION
        /// </summary>
        public string PosicionAlmacenId { get; set; }


        [XmlIgnore]
        [JsonIgnore]
        public PosicionAlmacen Posicion { get; set; }


        [XmlIgnore]
        [JsonIgnore]
        public ZonaAlmacen Zona { get; set; }

        [XmlIgnore]
        [JsonIgnore]
        public AlmacenArchivo Almacen { get; set; }


        [XmlIgnore]
        [JsonIgnore]
        public Archivo Archivo { get; set; }

        [XmlIgnore]
        [JsonIgnore]
        public List<EventoContenedorAlmacen> EventosContenedor { get; set; }
    }
}
