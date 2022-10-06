using PIKA.Constantes.Aplicaciones.GestorDocumental;
using PIKA.Infraestructura.Comun;
using PIKA.Modelo.Metadatos;
using PIKA.Modelo.Metadatos.Atributos;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;
using System.Xml.Serialization;

namespace PIKA.Modelo.GestorDocumental
{
    [Entidad(EliminarLogico: false, PaginadoRelacional: false, PermiteCambios: false,
    TokenApp: ConstantesAppGestionDocumental.MODULO_TRANSFERENCIA, 
        TokenMod: ConstantesAppGestionDocumental.APP_ID)]
    /// <summary>
    /// Mantien las relaciómn da activos incluidos en una transferenci
    /// </summary>
    /// 
    public class ActivoTransferencia
    {
        /// <summary>
        /// Identtificaor unico del reistro
        /// </summary>
        [Prop(Required: false, isId: true, Visible: false, OrderIndex: 0)]
        [VistaUI(ControlUI: ControlUI.HTML_HIDDEN, Accion: Acciones.update)]
        public string Id { get; set; }


        [Prop(Required: true, Visible: true, OrderIndex: 10)]
        [VistaUI(ControlUI: ControlUI.HTML_SELECT_MULTI, Accion: Acciones.addupdate)]
        [List(Entidad: "Activo", DatosRemotos: true, TypeAhead: true, FiltroBusqueda: true)]
        /// <summary>
        /// Identificador único del activo
        /// </summary>
        public string ActivoId { get; set; }

        [Prop(Searchable: false, Required: true, Visible: false, OrderIndex: 20, HieParent: false, ShowInTable: false,
        Contextual: true, IdContextual: ConstantesModelo.PREFIJO_CONEXTO + "Id")]
        [VistaUI(ControlUI: ControlUI.HTML_HIDDEN, Accion: Acciones.addupdate)]
        /// <summary>
        /// Identificador único de la trasnfernecia
        /// </summary>
        public string TransferenciaId { get; set; }


        /// <summary>
        /// Especifica si un activo es declinado en la transferencia
        /// </summary>
        [Prop(Required: false, OrderIndex: 30, DefaultValue: "false", Visible: true)]
        [VistaUI(ControlUI: ControlUI.HTML_HIDDEN, Accion: Acciones.none)]
        public bool Declinado { get; set; }


        /// <summary>
        /// Especifica si un activo es declinado en la transferencia
        /// </summary>
        [Prop(Required: false, OrderIndex: 40, DefaultValue: "false", Visible: true)]
        [VistaUI(ControlUI: ControlUI.HTML_HIDDEN, Accion: Acciones.none)]
        public string MotivoDeclinado { get; set; }


        /// <summary>
        /// Identificador único del usuario que la creó
        /// </summary>
        [Prop(Required: true, Visible: true, OrderIndex: 50)]
        [VistaUI(ControlUI: ControlUI.HTML_NONE, Accion: Acciones.none)]
        public string UsuarioIdDeclinadoId { get; set; }

        [XmlIgnore]
        [JsonIgnore]
        public Activo Activo { get; set; }
        [XmlIgnore]
        [JsonIgnore]
        public Transferencia Transferencia { get; set; }
    }
}
