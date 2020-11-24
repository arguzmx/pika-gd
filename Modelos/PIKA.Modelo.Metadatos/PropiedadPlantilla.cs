using Newtonsoft.Json;
using PIKA.Constantes.Aplicaciones.Metadatos;
using PIKA.Infraestructura.Comun;
using PIKA.Modelo.Metadatos.Atributos;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Xml.Serialization;

namespace PIKA.Modelo.Metadatos
{

    /// <summary>
    /// Define las una propiedad asocida a una plantilla del repositoprio
    /// </summary>
    [Entidad(PaginadoRelacional: false, EliminarLogico: false,
        TokenApp: ConstantesAppMetadatos.APP_ID, TokenMod: ConstantesAppMetadatos.MODULO_PLANTILLAS)]

    [EntidadVinculada(TokenSeguridad: ConstantesAppMetadatos.MODULO_PLANTILLAS,
        EntidadHijo: "TipoDatoId", Cardinalidad: TipoCardinalidad.UnoUno,
        HijoDinamico: true, TipoDespliegueVinculo: TipoDespliegueVinculo.EntidadUnica,
        PropiedadPadre: "Id", PropiedadHijo: "PropiedadId",
        Diccionario: "string,ValidadorTexto|double,ValidadorNumero|int,ValidadorNumero|long,ValidadorNumero")]
    public class PropiedadPlantilla : Propiedad
    {

        public PropiedadPlantilla()
        {
            this.ValoresLista = new HashSet<ValorListaPlantilla>();
        }

        /// <summary>
        /// Identificador de la plantilla a la que pertenece la propiedad
        /// </summary>
        [Prop(Required: true, Visible: false, OrderIndex: 1000, Contextual: true, IdContextual: ConstantesModelo.IDORIGEN_GLOBAL)]
        [VistaUI(ControlUI: ControlUI.HTML_HIDDEN, Accion: Acciones.addupdate)]
        public string PlantillaId { get; set; }


        /// <summary>
        /// Plantilla a la que pertenece la propiedad
        /// </summary>
        /// 
        [XmlIgnore]
        [JsonIgnore]
        public Plantilla Plantilla { get; set; }

        [XmlIgnore]
        [JsonIgnore]
        public virtual ICollection<ValorListaPlantilla> ValoresLista { get; set; }


    }
}
