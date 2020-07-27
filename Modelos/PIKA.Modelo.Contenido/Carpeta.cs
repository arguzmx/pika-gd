using RepositorioEntidades;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;
using System.Xml.Serialization;

namespace PIKA.Modelo.Contenido
{
    /// <summary>
    /// Proporciona una estructura jerárquica de folders para situar elementos del contenido
    /// </summary>
    public class Carpeta : Entidad<string>, IEntidadNombrada, IEntidadEliminada, IEntidadRegistroCreacion
    {
        public Carpeta() {
            Subcarpetas = new HashSet<Carpeta>();
        }

        /// <summary>
        /// Identificador del punto de montaje asociado a la carpeta
        /// </summary>
        public string PuntoMontajeId { get; set; }

        /// <summary>
        /// Identificador único de la carptea
        /// </summary>
        public override string Id { get; set; }

        /// <summary>
        /// Identificador único  del creador de la carpeta
        /// </summary>
        public string CreadorId { get; set; }

        /// <summary>
        /// FEcha de creación en formato UTC
        /// </summary>
        public DateTime FechaCreacion { get; set; }

        /// <summary>
        /// Nombre para la carpeta 
        /// </summary>
        public string Nombre { get; set; }
        // #Longitud 512

        /// <summary>
        /// Identifica si la carpeta ha sido eliminada
        /// </summary>
        public bool Eliminada { get; set; }
        /// Default=false

        /// <summary>
        /// Identificador de la carpeta padre de la actual 
        /// </summary>
        public string CarpetaPadreId { get; set; }
        // Es opcional 

        /// <summary>
        /// Identificador único del permiso a la carpeta
        /// </summary>
        public string PermisoId { get; set; }
        // Es opcional


        /// <summary>
        /// Determina si la carpeta es un nodo raíz, en esta caso la propiedad CarpetaPadreId debe ser nula
        /// </summary>
        public bool? EsRaiz { get; set; }


        public Permiso Permiso { get; set; }

        [XmlIgnore]
        [JsonIgnore]
        public Carpeta CarpetaPadre { get; set; }
       
        public virtual ICollection<Carpeta> Subcarpetas { get; set; }

        [XmlIgnore]
        [JsonIgnore]
        public PuntoMontaje PuntoMontaje { get; set; }

    }
}
