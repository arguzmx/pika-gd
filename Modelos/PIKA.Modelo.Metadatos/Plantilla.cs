using PIKA.Constantes.Aplicaciones.Metadatos;
using PIKA.Infraestructura.Comun;
using PIKA.Modelo.Metadatos.Atributos;
using RepositorioEntidades;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;
using System.Xml.Serialization;

namespace PIKA.Modelo.Metadatos
{

    [Entidad(PaginadoRelacional: false, EliminarLogico: true, 
        TokenApp: ConstantesAppMetadatos.APP_ID, TokenMod: ConstantesAppMetadatos.MODULO_PLANTILLAS)]
    [EntidadVinculada(TokenSeguridad: ConstantesAppMetadatos.MODULO_PLANTILLAS, EntidadHijo: "PropiedadPlantilla",
        Cardinalidad: TipoCardinalidad.UnoVarios, PropiedadPadre: "Id", PropiedadHijo: "PlantillaId")]
    public class Plantilla : Entidad<string>, IEntidadNombrada, IEntidadRelacionada, IEntidadEliminada
    {

        public string TipoOrigenDefault => ConstantesModelo.IDORIGEN_DOMINIO;

        public Plantilla()
        {
            this.Propiedades = new HashSet<PropiedadPlantilla>();
            this.TipoOrigenId = TipoOrigenDefault;
        }


        /// <summary>
        /// Identificador único de la plantilla
        /// </summary>
        [Prop(Required: false, isId: true, Visible: false, OrderIndex: 0)]
        [VistaUI(ControlUI: ControlUI.HTML_HIDDEN, Accion: Acciones.update)]
        public override string Id { get => base.Id; set => base.Id = value; }

        /// <summary>
        /// Nombre de la plantilla
        /// </summary>
        [Prop(Required: true, OrderIndex: 5)]
        [VistaUI(ControlUI: ControlUI.HTML_TEXT, Accion: Acciones.addupdate)]
        [ValidString(minlen: 2, maxlen: 200)]
        public string Nombre { get; set; }

        /// <summary>
        /// Identificador de relación de origem, en este caso se utiliza
        /// para vincular la plantilla con su dominio 
        /// </summary>
        [Prop(Required: false, OrderIndex: 100, Contextual: true, ShowInTable: false, Searchable: false)]
        [VistaUI(ControlUI: ControlUI.HTML_HIDDEN, Accion: Acciones.addupdate)]
        public string TipoOrigenId { get; set; }

        /// <summary>
        /// Identficador único del dominio al que pertenece la plantilla
        /// El Id de ralción es el identificador de un dominio
        /// </summary>
        [Prop(Required: false, OrderIndex: 110, Contextual: true, ShowInTable: false, Searchable: false)]
        [VistaUI(ControlUI: ControlUI.HTML_HIDDEN, Accion: Acciones.addupdate)]
        public string OrigenId { get; set; }

        /// <summary>
        /// Establece si la plantilla ha sido eliminada
        /// </summary>
        public bool Eliminada { get; set; }

        /// <summary>
        /// Determina si la plantilla ha sido gnerada en esl sisteams de gestión de metadatos
        /// </summary>
        [Prop(Required: false, OrderIndex: 1000, ShowInTable: false, Searchable: false)]
        [VistaUI(ControlUI: ControlUI.HTML_NONE, Accion: Acciones.none)]
        public bool Generada { get; set; }


        /// <summary>
        /// Identificador único del almacen de datos
        /// </summary>
        public string AlmacenDatosId { get; set; }

        [XmlIgnore]
        [JsonIgnore]
        public AlmacenDatos Almacen { get; set; }
        
        public ICollection<PropiedadPlantilla> Propiedades { get; set; }
        
        [XmlIgnore]
        [JsonIgnore]
        public ICollection<AsociacionPlantilla> Asociaciones { get; set; }
       
    }
}
