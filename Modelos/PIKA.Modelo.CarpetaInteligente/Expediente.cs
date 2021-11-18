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
    /// Modelo que sirve como plantilla para la creación de instancias de carpetas inteligentes
    /// </summary>
    public class Expediente: Entidad<Guid>, IEntidadRegistroCreacion,
         IEntidadNombrada, IEntidadRelacionada, ICaducible
    {

        [XmlIgnore]
        [JsonIgnore]
        public string TipoOrigenDefault => ConstantesModelo.IDORIGEN_DOMINIO;

        public Expediente()
        {
            Contenido = new List<ContenidoBase>();
            Permisos = new List<PermisoAcceso>();
        }

        /// <summary>
        /// Identificador único del expediente
        /// </summary>
        public override Guid Id { get; set; }

        /// <summary>
        /// Nombre del expediente
        /// </summary>
        public string Nombre { get; set; }

        /// <summary>
        /// Valor fijo siempre debe ser el identificador de dominio
        /// </summary>
        public string TipoOrigenId { get; set; }

        /// <summary>
        /// Identificador único del dominio al que pertenece la carpeta
        /// </summary>
        public string OrigenId { get; set; }
        /// <summary>
        /// Descripción del uso de la carpeta y contenido
        /// </summary>
        public string Descripción { get; set; }

        /// <summary>
        /// Identificador del creador de la carpeta
        /// </summary>
        public string CreadorId { get; set; }
        // Del sistema vía el valor en el JWT

        /// <summary>
        /// Fecha de creación de la carpeta
        /// </summary>
        public DateTime FechaCreacion { get; set; }
        // Del sistema en UTC

        /// <summary>
        /// Especifica el tipo de caducidad
        /// </summary>
        public TipoCaducidad TipoCaducidad { get; set; }

        /// <summary>
        /// Unidad de medida para el intevalo de caducidad
        /// </summary>
        public UnidadesCaducidad UnidadesCaducidad { get; set; }

        /// <summary>
        /// Número de unidades del intervalo para calcular la caducidad
        /// </summary>
        public int IntervaloCaducidad { get; set; }

        /// <summary>
        /// Especifica que en cada aactualización de caducidad, el intervalo comienza de nuevo
        /// </summary>
        public bool CaducidadCiclica { get; set; }

        /// <summary>
        /// Especifica si el expediente debe tener un registro de completez
        /// </summary>
        public bool TieneCompletez { get; set; }

        /// <summary>
        /// Identificador de la plantilla de metadatos aplicable al expediente
        /// </summary>
        public string PlantillaMetadatosId { get; set; }

        /// <summary>
        /// Contenido del expediente
        /// </summary>
        public List<ContenidoBase> Contenido { get; set; }


        /// <summary>
        /// LIsta de permisos de acceso a la entidad
        /// </summary>
        public List<PermisoAcceso> Permisos { get; set; }
    }
}
