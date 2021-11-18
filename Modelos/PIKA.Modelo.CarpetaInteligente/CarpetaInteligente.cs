using PIKA.Infraestructura.Comun;
using RepositorioEntidades;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;
using System.Xml.Serialization;

namespace PIKA.Modelo.CarpetaInteligente
{

    /// <summary>
    /// Entidad base para la definición de carpetas inteligentes, las carpeats inteligentes se declarana a nivel de dominio
    /// con la finalidad de que se encuentren disponibles para toda la organización
    /// </summary>
    public class CarpetaInteligente : Entidad<Guid>, IEntidadRegistroCreacion,
        IEntidadEliminada, IEntidadNombrada, IEntidadRelacionada
    {

        public CarpetaInteligente()
        {
            TipoOrigenId = this.TipoOrigenDefault;
            Contenido = new List<ContenidoBase>();
            Permisos = new List<PermisoAcceso>();
          }

        [XmlIgnore]
        [JsonIgnore]
        public string TipoOrigenDefault => ConstantesModelo.IDORIGEN_DOMINIO;

        /// <summary>
        /// Identificador único de la carpeta
        /// </summary>
        public override Guid Id { get; set; }

        /// <summary>
        /// Nombre de la carpeta
        /// </summary>
        public string Nombre { get; set; }


        /// <summary>
        /// Descripción del uso de la carpeta y contenido
        /// </summary>
        public string Descripción { get; set; }

        /// <summary>
        /// Identificador del creador de la carpeta
        /// </summary>
        public string CreadorId { get ; set ; }
        // Del sistema vía el valor en el JWT

        /// <summary>
        /// Fecha de creación de la carpeta
        /// </summary>
        public DateTime FechaCreacion { get ; set ; }
        // Del sistema en UTC


        /// <summary>
        /// Identificador único del volumen donde se almacenará el contenido electrónico
        /// </summary>
        public string VolumenId { get; set; }

        /// <summary>
        /// Identificador único del punto de montaje por defecto donde serán creados las instancias 
        /// para su despliegue en el explorador de contenido
        /// </summary>
        public string PuntoMontajeId { get; set; }

        /// <summary>
        /// Valor fijo siempre debe ser el identificador de dominio
        /// </summary>
        public string TipoOrigenId { get; set; }

        /// <summary>
        /// Identificador único del dominio al que pertenece la carpeta
        /// </summary>
        public string OrigenId { get ; set ; }

        /// <summary>
        /// DEtermina si la carpeta se enuentra eliminada
        /// </summary>
        public bool Eliminada { get ; set ; }


        /// <summary>
        /// Identificador de la plantilla de metadatos aplicable a la carpeta inteligente
        /// </summary>
        public string PlantillaMetadatosId { get; set; }

        /// <summary>
        /// Lista de elementos contenidos a nivel raíz de la carpeta
        /// </summary>
        public List<ContenidoBase> Contenido { get; set; }

        /// <summary>
        /// LIsta de permisos de acceso a la entidad
        /// </summary>
        public List<PermisoAcceso> Permisos { get; set; }

    }
}
