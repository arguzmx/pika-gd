using PIKA.Infraestructura.Comun;
using RepositorioEntidades;
using System;
using System.Collections.Generic;
using System.Text;

namespace PIKA.Modelo.Contenido
{
    public class Elemento : Entidad<string>, IEntidadRegistroCreacion, IEntidadEliminada, IEntidadRelacionada, IEntidadNombrada
    {
        public string TipoOrigenDefault => ConstantesModelo.IDORIGEN_UNIDAD_ORGANIZACIONAL;
        public Elemento() {
            this.TipoOrigenId = TipoOrigenDefault;
            this.Versiones = new HashSet<Version>();
        }

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
        /// Tipo de origen del contenido, por defecto el contenido será propiedad de la Unidad Organizacional que gestiona 
        /// el reposotorio. El orign tambi´´ien puede ser ConstantesModelo.IDORIGEN_DOMINIO 
        /// </summary>
        public string TipoOrigenId { get; set; }
        //#Requerido, GUID

        /// <summary>
        /// Identificador único del origen por ejemplo el ID de la unidad organizaciónal
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

        /// <summary>
        /// Identificador único del volúmen para el contenido
        /// </summary>
        public string VolumenId { get; set; }

        public virtual Volumen Volumen { get; set; }

        public virtual ICollection<Version> Versiones { get; set; }

    }
}
