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
    public class ValidadorTexto: Entidad<string>
    {
        [Prop(Required: false, isId: true, Visible: false, OrderIndex: 0,
            Contextual: true, IdContextual: ConstantesModelo.PREFIJO_CONEXTO + "PropiedadId")]
        [VistaUI(ControlUI: ControlUI.HTML_HIDDEN, Accion: Acciones.addupdate)]
        public string PropiedadId { get; set; }

        [Prop(Required: true, OrderIndex: 10, DefaultValue: "")]
        [VistaUI(ControlUI: ControlUI.HTML_NUMBER, Accion: Acciones.addupdate)]
        public int longmin { get; set; }

        [Prop(Required: true, OrderIndex: 20, DefaultValue: "")]
        [VistaUI(ControlUI: ControlUI.HTML_NUMBER, Accion: Acciones.addupdate)]
        public int longmax { get; set; }

        [Prop(Required: false, OrderIndex: 30,  DefaultValue: "")]
        [VistaUI(ControlUI: ControlUI.HTML_TEXTAREA, Accion: Acciones.addupdate)]
        public string regexp { get; set; }


        [Prop(Required: false, OrderIndex: 1000, ShowInTable: false)]
        [VistaUI(ControlUI: ControlUI.HTML_NONE, Accion: Acciones.none)]
        public string valordefault { get; set; }

        /// <summary>
        /// Propieudad de navegación
        /// </summary>
        [NotMapped]
        [XmlIgnore]
        [JsonIgnore]
        public Propiedad Propiedad { get; set; }

        /// <summary>
        /// Propieudad de navegación
        /// </summary>
        [XmlIgnore]
        [JsonIgnore]
        public PropiedadPlantilla PropiedadPlantilla { get; set; }
    }
}
