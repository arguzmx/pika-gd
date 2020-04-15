using PIKA.Infraestructura.Comun;
using RepositorioEntidades;
using System;
using System.Collections.Generic;
using System.Text;

namespace PIKA.Modelo.Metadatos
{

    /// <summary>
    /// Este objeto nos permite asociar matadatos con un objeto existente en el sistema
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
        /// Ientificador del tipo de enlace 
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
        //#Tamano GUID es un campo obligatorio

        /// <summary>
        /// En caso de que el almacenmiento no pueda incluir los Ids de TipoOrigenId y OrigenId
        /// en esste campo debe almacenarse el Id generado por el almacén para el elemento asociado
        /// </summary>
        public string IdentificadorAlmacenamiento { get; set; }
        //#Tamano GUID es un campo opciona

        /// <summary>
        /// Identificador del tipo de almacén de metadatos asociado al objeto
        /// </summary>
        public string TipoAlmacenMetadatosId { get; set; }
        
        public virtual TipoAlmacenMetadatos TipoAlmacen { get; set; }

    }
}
