using RepositorioEntidades;
using System;
using System.Collections.Generic;
using System.Text;

namespace PIKA.Modelo.Contenido
{
    public class Version: Entidad<string>, IEntidadEliminada
    {
        /// <summary>
        /// Identificador único del elemento al que pertenece la versión
        /// </summary>
        public string ElementoId { get; set; }

        /// <summary>
        /// Indica si la versión es la activa, sólo pude existir una versión activa por elemento
        /// </summary>
        public bool Activa { get; set; }
        // este campo debe estar indexado

        /// <summary>
        /// Fecha de ceración de la versión
        /// </summary>
        public DateTime FechaCreacion { get; set; }

        /// <summary>
        /// Fecha de actualización es identica a la de creación al momento de crear el regitrso
        /// </summary>
        public DateTime FechaActualizacion { get; set; }
       

        public virtual Elemento Elemento { get; set; }
        public virtual ICollection<Parte> Partes { get; set; }
        public bool Eliminada { get; set; }
    }
}
