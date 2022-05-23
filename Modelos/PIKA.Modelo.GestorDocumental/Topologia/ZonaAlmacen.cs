using RepositorioEntidades;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;
using System.Xml.Serialization;

namespace PIKA.Modelo.GestorDocumental
{
    public class ZonaAlmacen : Entidad<string>, IEntidadNombrada
    {

        public ZonaAlmacen()
        {
            Posiciones = new List<PosicionAlmacen>();
            Contenedores = new List<ContenedorAlmacen>();
        }

        /// <summary>
        /// Nomnbre de la zona
        /// </summary>
        public string Nombre { get; set; }

        /// <summary>
        /// Clave asociada a la zona en la organización
        /// </summary>
        public string Clave { get; set; }

        /// <summary>
        /// Identificaodr único del archivo al qu pertenece la zona
        /// </summary>
        public string ArchivoId { get; set; }

        /// <summary>
        /// Identificador unico del almacen al que pertenece la zona
        /// </summary>
        public string AlmacenArchivoId { get; set; }


        [XmlIgnore]
        [JsonIgnore]
        public AlmacenArchivo Almacen { get; set; }


        [XmlIgnore]
        [JsonIgnore]
        public Archivo Archivo { get; set; }

        [XmlIgnore]
        [JsonIgnore]
        public List<PosicionAlmacen> Posiciones { get; set; }

        [XmlIgnore]
        [JsonIgnore]
        public List<ContenedorAlmacen> Contenedores { get; set; }
    }
}
