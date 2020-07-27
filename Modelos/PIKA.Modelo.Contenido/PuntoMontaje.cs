using Newtonsoft.Json;
using PIKA.Infraestructura.Comun;
using PIKA.Modelo.Metadatos;
using RepositorioEntidades;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace PIKA.Modelo.Contenido
{
    /// <summary>
    ///  Esta clase permite defiir puntos de montaje para una estructira jerarquica de carpetas
    /// </summary>
    public class PuntoMontaje : Entidad<string>, IEntidadRelacionada, 
        IEntidadNombrada, IEntidadEliminada, IEntidadRegistroCreacion
    {

        
        public string TipoOrigenDefault => ConstantesModelo.IDORIGEN_UNIDAD_ORGANIZACIONAL;

        public PuntoMontaje()
        {
            Carpetas = new HashSet<Carpeta>();
            VolumenesPuntoMontaje = new HashSet<VolumenPuntoMontaje>();
        }


       /// <summary>
       /// Tipo de origen asociado al punto de montaje, por ejemplo Unidad Orgnizacional
       /// </summary>
        public string TipoOrigenId { get; set; }

        /// <summary>
        /// Identificador único del origen, por ejemplo el Id de la Unidad Orgnizacional
        /// </summary>
        public string OrigenId { get; set; }

        /// <summary>
        /// Nomre único en el mismo nivel de loa jerarqu+ia para la carpeta
        /// </summary>
        public string Nombre { get; set; }

        /// <summary>
        /// Identifica si la carpeta ha sido eliminada
        /// </summary>
        public bool Eliminada { get; set; }
        
        
        /// <summary>
        /// Identiicador único del creador del punto de montaje
        /// </summary>
        public string CreadorId { get; set; }

        /// <summary>
        /// Fecha de creacón UTC del punto de montaje
        /// </summary>
        public DateTime FechaCreacion { get; set; }

        /// <summary>
        /// Identificador único del volumen por default para el punto de montaje
        /// </summary>
        public string VolumenDefaultId { get; set; }


        [XmlIgnore]
        [JsonIgnore]
        public ICollection<Carpeta> Carpetas { get; set; }

        public Volumen VolumenDefault { get; set; }

        public ICollection<VolumenPuntoMontaje> VolumenesPuntoMontaje { get; set; }

    }
}
