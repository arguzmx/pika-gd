using RepositorioEntidades;
using System;
using System.Collections.Generic;
using System.Text;

namespace PIKA.Modelo.Contenido
{
    public class Version: Entidad<string>, IEntidadRegistroCreacion ,IEntidadEliminada
    {

        /// <summary>
        /// Identificador único de la version
        /// </summary>
        public override string Id { get => base.Id; set => base.Id = value; }

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
        /// Identificadro único del creador de la versión
        /// </summary>
        public string CreadorId { get; set; }

        /// <summary>
        /// Especifica si la versión ha sido eliminada
        /// </summary>
        public bool Eliminada { get; set; }

        /// <summary>
        /// Mantiene la cuenta del número de partes asociadas a la versión
        /// </summary>
        public int ConteoPartes { get; set; }


        /// <summary>
        /// Mantiene el tamaño en bytes de las partes de la versión
        /// </summary>
        public long TamanoBytes { get; set; }


        /// <summary>
        /// Mantiene el indice del número de partes de la versión
        /// </summary>
        public int MaxIndicePartes { get; set; }

        public virtual Elemento Elemento { get; set; }
        public virtual ICollection<Parte> Partes { get; set; }
        
    }
}
