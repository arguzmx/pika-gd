using RepositorioEntidades;
using System;
using System.Collections.Generic;
using System.Text;

namespace PIKA.Modelo.Contenido
{
    /// <summary>
    /// Proporciona una estructura jerárquica de folders para situar elementos del contenido
    /// </summary>
    public class Carpeta : Entidad<string>, IEntidadNombrada, IEntidadEliminada, IEntidadRegistroCreacion
    {
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

        public Permiso Permiso { get; set; }

        public Carpeta CarpetaPadre { get; set; }

        public virtual ICollection<Carpeta> Subcarpetas { get; set; }
        
    }
}
