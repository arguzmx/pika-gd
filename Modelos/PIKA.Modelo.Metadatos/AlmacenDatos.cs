using RepositorioEntidades;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;
using System.Xml.Serialization;

namespace PIKA.Modelo.Metadatos
{
    /// <summary>
    /// Almacenes de metadatos disponibles
    /// </summary>
    public class AlmacenDatos: Entidad<string>, IEntidadNombrada
    {
        /// <summary>
        /// Identificador único del almacen
        /// </summary>
        public override string Id { get; set; }

        //Nombre del almacen de metadatos
        public string Nombre { get; set; }

        /// <summary>
        /// Protocolo para la conexión del almacen de datos
        /// </summary>
        public string Protocolo { get; set; }

        /// <summary>
        /// Direcion IP o URL para la conexión del almacen de datos
        /// </summary>
        public string Direccion { get; set; }

        /// <summary>
        /// Usuario utilizado para la conexión del almacen de datos
        /// </summary>
        public string Usuario { get; set; }

        /// <summary>
        /// Contraseña utlizada para la conexión del almacen de datos
        /// </summary>
        public string Contrasena { get; set; }

        /// <summary>
        /// Puerto utilziado para la conexión del almacen de datos
        /// </summary>
        public string Puerto { get; set; }

        /// <summary>
        /// Identificador del tipo de almacen
        /// </summary>
        public string TipoAlmacenMetadatosId  { get; set; }



        /// <summary>
        /// propiedad de navegacion
        /// </summary>
        [XmlIgnore]
        [JsonIgnore]
        public ICollection<Plantilla>  Plantillas { get; set; }
        [XmlIgnore]
        [JsonIgnore]
        public TipoAlmacenMetadatos TipoAlmacen { get; set; }
        
    }
}
