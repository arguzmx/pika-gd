using PIKA.Infraestructura.Comun;
using RepositorioEntidades;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;
using System.Xml.Serialization;

namespace PIKA.Modelo.Metadatos
{

    /// <summary>
    /// Este objeto nos permite asociar matadatos con 
    /// un objeto existente en el sistema
    /// </summary>
    public class AsociacionPlantilla: Entidad<string>,  IEntidadRelacionada
    {
                
        public string TipoOrigenDefault => ConstantesModelo.IDORIGEN_NULO;

        public AsociacionPlantilla()
        {
            this.TipoOrigenId = TipoOrigenDefault;
        }

        public override string Id { get; set; }

        /// <summary>
        /// Identificador del tipo de enlace 
        /// por ejemplo aqui se espera el nombre del mnódulo que asocia los metadatos
        /// </summary>
        public string TipoOrigenId { get; set; }
        
        /// <summary>
        /// Identificador único del objeto al que se asocian los metadatos
        /// </summary>
        public string OrigenId { get; set; }

        /// <summary>
        /// Identificador único de la plantilla 
        /// </summary>
        public string PlantillaId { get; set; }
        
        /// <summary>
        /// Identificador único proporcionado por el almacenamiento para el registro
        /// </summary>
        public string IdentificadorAlmacenamiento { get; set; }
        

        [XmlIgnore]
        [JsonIgnore]
        public virtual Plantilla Plantilla { get; set; }
    }
}
