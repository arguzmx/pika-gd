using Microsoft.AspNetCore.Razor.Language.Extensions;
using PIKA.Constantes.Aplicaciones.GestorDocumental;
using PIKA.Infraestructura.Comun;
using PIKA.Modelo.Metadatos;
using PIKA.Modelo.Metadatos.Atributos;
using RepositorioEntidades;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using System.Xml.Serialization;

namespace PIKA.Modelo.GestorDocumental
{

    /// <summary>
    /// Una trasferencia es el proceso para enviar actuivos de un archivo a otro
    /// a trvés de una lista de activos seleccionados para ser incluidos en dicha tarsnferencia
    /// </summary>

    [Entidad(PaginadoRelacional: false, EliminarLogico: false, BuscarPorTexto: true,
        TokenMod: ConstantesAppGestionDocumental.MODULO_TRANSFERENCIA,
        TokenApp: ConstantesAppGestionDocumental.APP_ID)]


    [EntidadVinculada(TokenSeguridad: ConstantesAppGestionDocumental.MODULO_TRANSFERENCIA,
        EntidadHijo: "ActivoTransferencia",
        Cardinalidad: TipoCardinalidad.UnoVarios, PropiedadPadre: "Id",
        PropiedadHijo: "TransferenciaId")]

    [LinkView(Titulo: "commandosweb.enviar-transferencia", Icono: "forward_to_inbox", Vista: "enviar-transferencia",
        RequireSeleccion: true, Tipo: TipoVista.WebCommand, Condicion: "entidad['EstadoTransferenciaId'] == 'nueva'")]

    [LinkView(Titulo: "commandosweb.aceptar-transferencia", Icono: "mark_email_read", Vista: "aceptar-transferencia",
        RequireSeleccion: true, Tipo: TipoVista.WebCommand, Condicion: "entidad['EstadoTransferenciaId'] == 'espera'")]

    [LinkView(Titulo: "commandosweb.declinar-transferencia", Icono: "unsubscribe", Vista: "declinar-transferencia",
        RequireSeleccion: true, Tipo: TipoVista.WebCommand, Condicion: "entidad['EstadoTransferenciaId'] == 'espera'")]

    public class Transferencia : Entidad<string>, IEntidadNombrada, IEntidadUsuario
    {

        public Transferencia()
        {
            Eventos = new HashSet<EventoTransferencia>();
            Comentarios = new HashSet<ComentarioTransferencia>();
            ActivosIncluidos = new HashSet<ActivoTransferencia>();
        }

        /// <summary>
        /// Identificador único interno para la trasnferencias
        /// </summary>
        [Prop(Required: false, isId: true, Visible: false, OrderIndex: 0, TableOrderIndex: 0)]
        [VistaUI(ControlUI: ControlUI.HTML_HIDDEN, Accion: Acciones.update)]
        [LinkViewParameter(Vista: "enviar-transferencia", Multiple: false)]
        [LinkViewParameter(Vista: "aceptar-transferencia", Multiple: false)]
        [LinkViewParameter(Vista: "declinar-transferencia", Multiple: false)]
        public override string Id { get; set; }


        /// <summary>
        /// Nombre asociado a la trasnferencia
        /// </summary>
        [Prop(Required: true, OrderIndex: 10, IsLabel: true, TableOrderIndex: 10)]
        [VistaUI(ControlUI: ControlUI.HTML_TEXT, Accion: Acciones.addupdate)]
        [ValidString(minlen: 2, maxlen: 200)]
        public string Nombre { get; set; }



        /// <summary>
        /// Número de fplio de la trasnferenci
        /// </summary>
        [Prop(Required: true, isId: false, Visible: true, OrderIndex: 15, TableOrderIndex: 15)]
        [VistaUI(ControlUI: ControlUI.HTML_TEXT, Accion: Acciones.addupdate)]
        public string Folio { get; set; }

        /// <summary>
        /// Identificador único del árchivo destino
        /// </summary>
        [Prop(Required: true, OrderIndex: 20, TableOrderIndex: 20)]
        [VistaUI(ControlUI: ControlUI.HTML_SELECT, Accion: Acciones.addupdate)]
        [List(Entidad: "Archivo", DatosRemotos: true, TypeAhead: false)]
        public string ArchivoDestinoId { get; set; }

        [Prop(Required: false, OrderIndex: 30, TableOrderIndex: 50)]
        [VistaUI(ControlUI: ControlUI.HTML_SELECT, Accion: Acciones.addupdate)]
        [List(Entidad: "CuadroClasificacion", DatosRemotos: true, TypeAhead: false, Default: "")]
        public string CuadroClasificacionId { get; set; }


        [Prop(Required: false, OrderIndex: 40, Orderable: false, Searchable: false, TableOrderIndex: 55)]
        [VistaUI(ControlUI: ControlUI.HTML_SELECT, Accion: Acciones.addupdate)]
        [List(Entidad: "EntradaClasificacion", DatosRemotos: true, TypeAhead: false)]
        [Event(Entidad: "CuadroClasificacionId", Evento: Metadatos.Atributos.Eventos.AlCambiar, Operacion: Operaciones.Actualizar, "CuadroClasificacionId")]
        public string EntradaClasificacionId { get; set; }


        
        [Prop(Required: true, Visible: true, OrderIndex: 25, TableOrderIndex: 30)]
        [VistaUI(ControlUI: ControlUI.HTML_SELECT, Accion: Acciones.addupdate)]
        [List("",Default: "0", ValoresCSV: "0,15,30,60,90,120,180")]
        public int RangoDias { get;  set; }


        [NotMapped]
        [Prop(Required: false, Visible: true, OrderIndex: 30,Orderable:false, Searchable: false, TableOrderIndex: 35)]
        [VistaUI(ControlUI: ControlUI.HTML_DATE, Accion: Acciones.none)]
        public DateTime FechaCorte { get { return FechaCreacion.AddDays(RangoDias); }  set { } }


        [NotMapped]
        [Prop(Required: false, Visible: true, OrderIndex: 60, ShowInTable:false)]
        [VistaUI(ControlUI: ControlUI.HTML_SELECT, Accion: Acciones.add)]
        [List(Entidad: "Activo", DatosRemotos: true, TypeAhead: false, EsListaTemas: true)]
        public string TemaId { get; set; }


        [NotMapped]
        [Prop(Required: false, Visible: true, OrderIndex: 70, ShowInTable: false)]
        [VistaUI(ControlUI: ControlUI.HTML_TOGGLE, Accion: Acciones.add)]
        public bool EliminarTema { get; set; }

        /// <summary>
        ///  Número de elementos involucrados en la trasnferencia
        /// </summary>
        [Prop(Required: false, isId: false, Visible: true, OrderIndex: 210, TableOrderIndex: 6)]
        [VistaUI(ControlUI: ControlUI.HTML_NUMBER, Accion: Acciones.none)]
        public int CantidadActivos { get; set; }

        /// <summary>
        /// Fechas de creación de la trasnfenrecia
        /// </summary>
        [Prop(Required: false, OrderIndex: 220, IsLabel: true, TableOrderIndex: 5)]
        [VistaUI(ControlUI: ControlUI.HTML_DATE, Accion: Acciones.none)]
        public DateTime FechaCreacion { get; set; }
        // Debe ser .Now en formato UTC

        /// <summary>
        /// Esatdo actual de la trasnferencia
        /// </summary>
        [Prop(Required: false, OrderIndex: 230, IsLabel: true)]
        [VistaUI(ControlUI: ControlUI.HTML_TEXT, Accion: Acciones.none)]
        [List(Entidad: "EstadoTransferencia", DatosRemotos: true, TypeAhead: false, EsListaTemas: false)]
        public string EstadoTransferenciaId { get; set; }
        //Obligatorios

        /// <summary>
        /// Identificador único del árchivo origen
        /// </summary>
        [Prop(Required: true, OrderIndex: 240, Visible: false, Contextual: true, ShowInTable: false,
        ToggleInTable: false, IdContextual: ConstantesModelo.PREFIJO_CONEXTO + "ArchivoId")]
        [VistaUI(ControlUI: ControlUI.HTML_HIDDEN, Accion: Acciones.addupdate)]
        public string ArchivoOrigenId { get; set; }


        /// <summary>
        /// Identificador único del usuario que la creó
        /// </summary>
        [Prop(Required: true, Visible: true, OrderIndex: 250)]
        [VistaUI(ControlUI: ControlUI.HTML_NONE, Accion: Acciones.none)]
        [List(Entidad: "PropiedadesUsuario", DatosRemotos: true)]
        public string UsuarioId { get; set; }



        /// <summary>
        /// Valor para filtrar en la UI entre amitidas y recibidas
        /// </summary>
        [Prop(Required: false, Visible: false, OrderIndex: 250, Searchable: true, ShowInTable: false)]
        [VistaUI(ControlUI: ControlUI.HTML_NONE, Accion: Acciones.none)]
        [NotMapped]
        public bool Recibidas { get; set; }

        [XmlIgnore]
        [JsonIgnore]
        public virtual ICollection<EventoTransferencia> Eventos { get; set; }

        [XmlIgnore]
        [JsonIgnore]
        public virtual ICollection<ActivoTransferencia> ActivosIncluidos { get; set; }

        [XmlIgnore]
        [JsonIgnore]
        public virtual  ICollection<ComentarioTransferencia> Comentarios { get; set; }


        [XmlIgnore]
        [JsonIgnore]
        public virtual EntradaClasificacion EntradaClasificacion { get; set; }

        [XmlIgnore]
        [JsonIgnore]
        public virtual CuadroClasificacion CuadroClasificacion { get; set; }

        [XmlIgnore]
        [JsonIgnore]
        public virtual EstadoTransferencia Estado { get; set; }
        [XmlIgnore]
        [JsonIgnore]
        public virtual Archivo ArchivoOrigen { get; set; }
        [XmlIgnore]
        [JsonIgnore]
        public virtual Archivo ArchivoDestino { get; set; }
        
    }
}
