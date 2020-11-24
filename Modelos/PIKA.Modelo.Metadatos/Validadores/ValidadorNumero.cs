using PIKA.Constantes.Aplicaciones.Metadatos;
using PIKA.Infraestructura.Comun;
using PIKA.Modelo.Metadatos.Atributos;
using RepositorioEntidades;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using System.Text.Json.Serialization;
using System.Xml.Serialization;

namespace PIKA.Modelo.Metadatos
{
    [Entidad(PaginadoRelacional: false, EliminarLogico: false,
   TokenApp: ConstantesAppMetadatos.APP_ID, TokenMod: ConstantesAppMetadatos.MODULO_PLANTILLAS)]
    public class ValidadorNumero: Entidad<string>
    {
        [Prop(Required: false, isId: true, Visible: false, OrderIndex: 0,
           Contextual: true, IdContextual: ConstantesModelo.PREFIJO_CONEXTO + "PropiedadId")]
        [VistaUI(ControlUI: ControlUI.HTML_HIDDEN, Accion: Acciones.addupdate)]
        public string PropiedadId { get; set; }

        [Prop(Required: true, OrderIndex: 10, DefaultValue: "0")]
        [VistaUI(ControlUI: ControlUI.HTML_NUMBER, Accion: Acciones.addupdate)]
        public float min { get; set; }

        [Prop(Required: true, OrderIndex: 20, DefaultValue: "0")]
        [VistaUI(ControlUI: ControlUI.HTML_NUMBER, Accion: Acciones.addupdate)]
        public float max { get; set; }

        [Prop(Required: true, OrderIndex: 30, DefaultValue: "false")]
        [VistaUI(ControlUI: ControlUI.HTML_TOGGLE, Accion: Acciones.addupdate)]
        public bool UtilizarMax { get; set; }


        [Prop(Required: true, OrderIndex: 40, DefaultValue: "false")]
        [VistaUI(ControlUI: ControlUI.HTML_TOGGLE, Accion: Acciones.addupdate)]
        public bool UtilizarMin { get; set; }

        [Prop(Required: false, OrderIndex: 1000, ShowInTable: false)]
        [VistaUI(ControlUI: ControlUI.HTML_NONE, Accion: Acciones.none)]
        public float valordefault { get; set; }

        /// <summary>
        /// Propeidad de navegación
        /// </summary>
        [NotMapped]
        [XmlIgnore]
        [JsonIgnore]
        public Propiedad Propiedad { get; set; }

        /// <summary>
        /// Propeidad de navegación
        /// </summary>
        [NotMapped]
        [XmlIgnore]
        [JsonIgnore]
        public PropiedadPlantilla PropiedadPlantilla { get; set; }
    }
}
