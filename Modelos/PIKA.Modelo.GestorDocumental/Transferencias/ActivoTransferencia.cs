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
    [Entidad(EliminarLogico: false, PaginadoRelacional: false, PermiteCambios: true, PermiteEliminarTodo: true, BuscarPorTexto: true,
    TokenApp: ConstantesAppGestionDocumental.MODULO_TRANSFERENCIA, 
        TokenMod: ConstantesAppGestionDocumental.APP_ID)]


    [LinkView(Titulo: "commandosweb.activotransferencias-aceptar", Icono: "thumb_up", Vista: "aceptar-activos-tx",
        RequireSeleccion: true, Tipo: TipoVista.WebCommand, Condicion: "")]
    [LinkView(Titulo: "commandosweb.activotransferencias-declinar", Icono: "thumb_down", Vista: "declinar-activos-tx",
        RequireSeleccion: true, Tipo: TipoVista.WebCommand, Condicion: "")]
    /// <summary>
    /// Mantien las relaciómn da activos incluidos en una transferenci
    /// </summary>
    /// 
    public class ActivoTransferencia
    {
        /// <summary>
        /// Identtificaor unico del reistro
        /// </summary>
        [Prop(Required: false, isId: true, Visible: false, OrderIndex: 0, TableOrderIndex: 0)]
        [VistaUI(ControlUI: ControlUI.HTML_HIDDEN, Accion: Acciones.update)]
        [LinkViewParameter(Vista: "aceptar-activos-tx", Multiple: true)]
        [LinkViewParameter(Vista: "declinar-activos-tx", Multiple: true)]
        public string Id { get; set; }


        [Prop(Required: true, Visible: true, OrderIndex: 10, TableOrderIndex: 10)]
        [VistaUI(ControlUI: ControlUI.HTML_SELECT_MULTI, Accion: Acciones.add)]
        [List(Entidad: "Activo", DatosRemotos: true, TypeAhead: true, FiltroBusqueda: true)]
        /// <summary>
        /// Identificador único del activo
        /// </summary>
        public string ActivoId { get; set; }


        [Prop(Required: false, Visible: true, OrderIndex: 15, TableOrderIndex: 20)]
        [VistaUI(ControlUI: ControlUI.HTML_TEXTAREA, Accion: Acciones.addupdate)]
        /// <summary>
        /// Identificador único del activo
        /// </summary>
        public string Notas { get; set; }


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
        [Prop(Required: false, OrderIndex: 40, DefaultValue: "false", Visible: true, TableOrderIndex: 40)]
        [VistaUI(ControlUI: ControlUI.HTML_TOGGLE, Accion: Acciones.none)]
        public bool Declinado { get; set; }


        /// <summary>
        /// Especifica si un activo es aceptado por el archivo receptor 
        /// </summary>
        [Prop(Required: false, OrderIndex: 30, DefaultValue: "false", Visible: true, TableOrderIndex: 30)]
        [VistaUI(ControlUI: ControlUI.HTML_TOGGLE, Accion: Acciones.none)]
        public bool Aceptado { get; set; }


        /// <summary>
        /// Fecha en la que se emitió el voto para aceptar o declinar
        /// </summary>
        [Prop(Required: false, Visible: true, OrderIndex: 50, TableOrderIndex: 50)]
        [VistaUI(ControlUI: ControlUI.HTML_DATETIME, Accion: Acciones.none)]
        public DateTime? FechaVoto { get; set; }

        /// <summary>
        /// Fecha en la que se emitió el voto para aceptar o declinar
        /// </summary>
        [Prop(Required: false, Visible: true, OrderIndex: 12, TableOrderIndex: 12)]
        [VistaUI(ControlUI: ControlUI.HTML_DATE, Accion: Acciones.none)]
        public DateTime FechaRetencion { get; set; }


        [Prop(Required: false, OrderIndex: 30, TableOrderIndex: 50, Searchable: false)]
        [VistaUI(ControlUI: ControlUI.HTML_SELECT, Accion: Acciones.none)]
        [List(Entidad: "CuadroClasificacion", DatosRemotos: true, TypeAhead: false, Default: "")]
        public string CuadroClasificacionId { get; set; }


        [Prop(Required: false, OrderIndex: 40, Orderable: false, Searchable: false, TableOrderIndex: 55)]
        [VistaUI(ControlUI: ControlUI.HTML_SELECT, Accion: Acciones.none)]
        [List(Entidad: "EntradaClasificacion", DatosRemotos: true, TypeAhead: false)]
        public string EntradaClasificacionId { get; set; }

        /// <summary>
        /// Especifica si un activo es declinado en la transferencia
        /// </summary>
        [Prop(Required: false, OrderIndex: 60, DefaultValue: "false", Visible: false, TableOrderIndex: 60)]
        [VistaUI(ControlUI: ControlUI.HTML_TEXT, Accion: Acciones.none)]
        public string MotivoDeclinado { get; set; }


        /// <summary>
        /// Identificador único del usuario que añadió el activo
        /// </summary>
        [Prop(Required: false, Visible: false, OrderIndex: 50)]
        [VistaUI(ControlUI: ControlUI.HTML_NONE, Accion: Acciones.none)]
        [List(Entidad: "PropiedadesUsuario", DatosRemotos: true)]
        public string UsuarioId{ get; set; }

        /// <summary>
        /// Identificador único del usuario que acepta o declina el activos
        /// </summary>
        [Prop(Required: false, Visible: false, OrderIndex: 50)]
        [VistaUI(ControlUI: ControlUI.HTML_NONE, Accion: Acciones.none)]
        [List(Entidad: "PropiedadesUsuario", DatosRemotos: true)]
        public string UsuarioReceptorId { get; set; }





        [XmlIgnore]
        [JsonIgnore]
        public Activo Activo { get; set; }
        [XmlIgnore]
        [JsonIgnore]
        public Transferencia Transferencia { get; set; }
    }
}
