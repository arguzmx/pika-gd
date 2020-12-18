using PIKA.Constantes.Aplicaciones.Metadatos;
using PIKA.Infraestructura.Comun;
using PIKA.Modelo.Metadatos.Atributos;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;
using System.Xml.Serialization;

namespace PIKA.Modelo.Metadatos
{
    [Entidad(PaginadoRelacional: false, EliminarLogico: false,
    TokenApp: ConstantesAppMetadatos.APP_ID, TokenMod: ConstantesAppMetadatos.MODULO_PLANTILLAS)]

    public class ValorListaPlantilla: ValorLista
    {
        /// <summary>
        ///  Identificadro único de la propiead a la que pertenece el elemento
        /// </summary>
        [Prop(Required: true, Visible: false, OrderIndex: 1000, Contextual: true, IdContextual: ConstantesModelo.PREFIJO_CONEXTO + "PropiedadId")]
        [VistaUI(ControlUI: ControlUI.HTML_HIDDEN, Accion: Acciones.addupdate)]
        public string PropiedadId { get; set; }
        [XmlIgnore]
        [JsonIgnore]
        public PropiedadPlantilla Propiedad { get; set; }

    }
}
