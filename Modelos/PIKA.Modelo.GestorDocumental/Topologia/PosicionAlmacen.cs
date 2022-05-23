using RepositorioEntidades;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;
using System.Xml.Serialization;

namespace PIKA.Modelo.GestorDocumental
{
    public class PosicionAlmacen : Entidad<string>, IEntidadNombrada
    {
        public PosicionAlmacen()
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
        /// Indice de la posicion en la osición padre
        /// </summary>
        public int Indice { get; set; }

        /// <summary>
        /// Codigo de barras asociado
        /// </summary>
        public string CodigoBarras { get; set; }

        /// <summary>
        /// Codigo electrónico asociado
        /// </summary>
        public string CodigoElectronico { get; set; }

        /// <summary>
        /// Porcentaje de ocupación de la posición
        /// </summary>
        public decimal Ocupacion { get; set; }

        /// <summary>
        /// Identificaodr único del archivo al qu pertenece la posición
        /// </summary>
        public string ArchivoId { get; set; }

        /// <summary>
        /// Identificador unico del almacen al que pertenece la posición
        /// </summary>
        public string AlmacenArchivoId { get; set; }

        /// <summary>
        /// Identificador unico de la zona a la que pertenece la posición
        /// </summary>
        public string ZonaAlmacenId { get; set; }


        /// <summary>
        /// Identificador unico de la posición padre para el elemento
        /// </summary>
        public string PosicionPadreId { get; set; }


        [XmlIgnore]
        [JsonIgnore]
        public PosicionAlmacen PosicionPadre { get; set; }


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
        public List<PosicionAlmacen> Posiciones { get; set; }

        [XmlIgnore]
        [JsonIgnore]
        public List<ContenedorAlmacen> Contenedores { get; set; }
    }
}
