using PIKA.Infraestructura.Comun;
using PIKA.Modelo.Metadatos;
using RepositorioEntidades;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;
using System.Xml.Serialization;

namespace PIKA.Modelo.Contenido
{
    public class Elemento : Entidad<string>, IEntidadRegistroCreacion, IEntidadEliminada, IEntidadRelacionada, IEntidadNombrada
    {
        [XmlIgnore]
        [JsonIgnore]
        public string TipoOrigenDefault => ConstantesModelo.IDORIGEN_PUNTO_MONTAJE;

        public Elemento() {
            this.TipoOrigenId = TipoOrigenDefault;
            this.Versiones = new HashSet<Version>();
        }

        /// <summary>
        /// Indetificador único del elemento de contenido
        /// </summary>
        public override string Id { get => base.Id; set => base.Id = value; }

      
        /// <summary>
        /// Sombre común dado al elemento de contenido
        /// </summary>
        public string Nombre { get; set; }
        //#Requerido, longitud nombre

        /// <summary>
        /// Indica si el elemento ha sido eliminado de manera lógica
        /// </summary>
        public bool Eliminada { get; set; }
        //#Requerido default=false

        /// <summary>
        /// Tipo de origen del contenido, por defecto el contenido será propiedad del punto de montaje
        /// </summary>
        public string TipoOrigenId { get; set; }
        //#Requerido, GUID

        /// <summary>
        /// Identificador único del origen por ejemplo el ID del punto de montaje
        /// </summary>
        public string OrigenId { get; set; }
        //#Requerido, GUID

        /// <summary>
        /// Identificador único del usuario que creó la entidad
        /// </summary>
        public string CreadorId { get; set; }
        //#Requerido, GUID


        /// <summary>
        /// Fecah de creación de la entidad en formato UTC
        /// </summary>
        public DateTime FechaCreacion { get; set; }
        //#Requerido

        ///// <summary>
        ///// Identificador único del volúmen para el contenido
        ///// </summary>
        public string VolumenId { get; set; }

        /// <summary>
        /// Identificador único de la carpeta donde se creó el contenido
        /// </summary>
        public string CarpetaId { get; set; }
        // Es opcional

        /// <summary>
        /// Identificador únido del permiso asociado al elemeento
        /// </summary>
        public string PermisoId { get; set; }
        // Es opcional


        /// <summary>
        /// Identifica si el elemento es administrado por versiones
        /// Todos los elementos tiene una versión inicial, 
        /// cuando se añaden nuevas versiones este campo toma el valor true
        /// </summary>
        public bool Versionado { get; set; }

        [XmlIgnore]
        [JsonIgnore]
        public Permiso Permiso { get; set; }
        [XmlIgnore]
        [JsonIgnore]
        public virtual Volumen Volumen { get; set; }
        [XmlIgnore]
        [JsonIgnore]
        public virtual ICollection<Version> Versiones { get; set; }
        [XmlIgnore]
        [JsonIgnore]
        public virtual ICollection<Parte> Partes { get; set; }

    }
}
