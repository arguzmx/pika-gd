using PIKA.Infraestructura.Comun;
using RepositorioEntidades;
using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using System.Xml.Serialization;
using PIKA.Modelo.Metadatos;
using PIKA.Modelo.Metadatos.Atributos;
using System.ComponentModel.DataAnnotations.Schema;
using PIKA.Constantes.Aplicaciones.GestorDocumental;
using PIKA.Modelo.GestorDocumental.Reportes;

namespace PIKA.Modelo.GestorDocumental
{

    [Entidad(PaginadoRelacional: false, EliminarLogico: true, HabilitarSeleccion: true,
        TokenMod: ConstantesAppGestionDocumental.MODULO_PRESTAMO,
        TokenApp: ConstantesAppGestionDocumental.APP_ID)]

    [EntidadVinculada(TokenSeguridad: ConstantesAppGestionDocumental.MODULO_PRESTAMO,
        EntidadHijo: "ActivoPrestamo",
        Cardinalidad: TipoCardinalidad.UnoVarios, PropiedadPadre: "Id",
        PropiedadHijo: "PrestamoId")]

    [LinkView(Titulo: "commandosweb.gd-prestamo-entregar", Icono: "assignment_ind", Vista: "gd-prestamo-entregar", 
        RequireSeleccion: true, Tipo: TipoVista.WebCommand, Condicion: "['Entregado']==0")]
    [LinkView(Titulo: "commandosweb.gd-prestamo-devolver", Icono: "assignment_turned_in", Vista: "gd-prestamo-devolver", 
        RequireSeleccion: true, Tipo: TipoVista.WebCommand, Condicion: "['Entregado']==1 && ['Devuelto']==0")]
    public class Prestamo: Entidad<string>, IEntidadEliminada, IEntidadReportes
    {
        [NotMapped]
        [JsonIgnore]
        [XmlIgnore]
        public List<IProveedorReporte> Reportes { get; set; }

        public Prestamo() {
            ActivosRelacionados = new HashSet<ActivoPrestamo>();
            Comentarios = new HashSet<ComentarioPrestamo>();
            this.Reportes = new List<IProveedorReporte>();
            this.Reportes.Add(new ReportePrestamo());
        }

        [Prop(Required: false, isId: true, Visible: false, OrderIndex: 0)]
        [VistaUI(ControlUI: ControlUI.HTML_HIDDEN, Accion: Acciones.update)]
        [LinkViewParameter(Vista: "gd-prestamo-entregar")]
        [LinkViewParameter(Vista: "gd-prestamo-devolver")]
        public override string Id { get; set; }

        /// <summary>
        /// FEcha de creación del regiustro debe ser la fecha del sistema UTC, sin intervención del usuario
        /// </summary>
        [Prop(Required: false, isId: false, Visible: true, OrderIndex: 10)]
        [VistaUI(ControlUI: ControlUI.HTML_DATETIME, Accion: Acciones.none)]
        public DateTime FechaCreacion { get; set; }

        /// <summary>
        /// Número de fplio del préstamo
        /// </summary>
        [Prop(Required: true, isId: false, Visible: true, OrderIndex: 20)]
        [VistaUI(ControlUI: ControlUI.HTML_TEXT, Accion: Acciones.add)]
        public string Folio { get; set; }

        /// <summary>
        ///  Número de elementos involucrados en el préstamo
        /// </summary>
        [Prop(Required: false, isId: false, Visible: true, OrderIndex: 25)]
        [VistaUI(ControlUI: ControlUI.HTML_NONE, Accion: Acciones.none)]
        public int CantidadActivos { get; set; }


        /// <summary>
        /// Identificador del usuario destinatario del préstamo
        /// </summary>
        [Prop(Required: true, Visible: true, OrderIndex: 30)]
        [VistaUI(ControlUI: ControlUI.HTML_SELECT_MULTI, Accion: Acciones.addupdate)]
        [List(Entidad: "PropiedadesUsuario", DatosRemotos: true, TypeAhead: true)]
        public string UsuarioDestinoId { get; set; }


        /// <summary>
        /// Fecha programda parala devolucion del preátsmo en formato UTC
        /// </summary>
        [Prop(Required: false, isId: false, Visible: true, OrderIndex: 40)]
        [VistaUI(ControlUI: ControlUI.HTML_DATE, Accion: Acciones.addupdate)]
        public DateTime FechaProgramadaDevolucion { get; set; }

        /// <summary>
        /// Fecha de devolución real del préstamo, este valor se establece sin intervención del  usuario 
        /// </summary>
        [Prop(Required: false, isId: false, Visible: true, OrderIndex: 50)]
        [VistaUI(ControlUI: ControlUI.HTML_NONE, Accion: Acciones.none)]
        public DateTime? FechaDevolucion { get; set; }

        [Prop(Required: true, isId: false, Visible: false, OrderIndex: 60)]
        [VistaUI(ControlUI: ControlUI.HTML_TEXTAREA, Accion: Acciones.addupdate)]
        public string Descripcion { get; set; }


        /// <summary>
        /// ESTOS VALORES SE CALCULAR POR SISTEMA  EN BASE AL CONTROL DE PRESTAMO
        /// </summary>
        [Prop(Required: false, OrderIndex: 65, DefaultValue: "false", Visible: true)]
        [VistaUI(ControlUI: ControlUI.HTML_HIDDEN, Accion: Acciones.none)]
        public bool Entregado { get; set; }

        /// <summary>
        /// Estupila si el préstamo ha sico devuelto en su totalidad
        /// </summary>
        [Prop(Required: false, isId: false, Visible: true, OrderIndex: 70)]
        [VistaUI(ControlUI: ControlUI.HTML_NONE, Accion: Acciones.none)]
        public bool Devuelto { get; set; }

        /// <summary>
        /// Indica si ha habido devoluciones parciales del préstamo, sin intervención del usuario
        /// </summary>
        [Prop(Required: false, isId: false, Visible: true, OrderIndex: 80)]
        [VistaUI(ControlUI: ControlUI.HTML_NONE, Accion: Acciones.none)]
        public bool TieneDevolucionesParciales { get; set; }


        /// <summary>
        /// Identificador único del usuario origen del prestamo
        /// </summary>
        [Prop(Required: true, Visible: true, OrderIndex: 100)]
        [VistaUI(ControlUI: ControlUI.HTML_NONE, Accion: Acciones.none)]
        [List(Entidad: "PropiedadesUsuario", DatosRemotos: true, TypeAhead: true)]
        public string UsuarioOrigenId { get; set; }


        [NotMapped]
        [Prop(Required: true, Visible: true, OrderIndex: 100)]
        [VistaUI(ControlUI: ControlUI.HTML_SELECT, Accion: Acciones.add)]
        [List(Entidad: "Activo", DatosRemotos: true, TypeAhead: false, EsListaTemas: true)]
        public string TemaId { get; set; }


        /// <summary>
        /// Identificador único del archivo actual del activo
        /// ESTOS VALORES SE CALCULAR POR SISTEMA  EN BASE AL LOS PROCESOS DE TRASNFENRENCIA
        /// </summary>
        [Prop(Required: true, OrderIndex: 600, Visible: false, Contextual: true, ShowInTable: false, 
            ToggleInTable: false, IdContextual: ConstantesModelo.PREFIJO_CONEXTO + "ArchivoId")]
        [VistaUI(ControlUI: ControlUI.HTML_HIDDEN, Accion: Acciones.addupdate)]
        public string ArchivoId { get; set; }


        [Prop(Required: false, OrderIndex: 100, DefaultValue: "false", Visible: false)]
        [VistaUI(ControlUI: ControlUI.HTML_HIDDEN, Accion: Acciones.none)]
        public bool Eliminada { get; set; }


        public Archivo Archivo { get; set; }
        [XmlIgnore]
        [JsonIgnore]
        public virtual ICollection<ActivoPrestamo> ActivosRelacionados { get; set; }

        [XmlIgnore]
        [JsonIgnore]
        public virtual ICollection<ComentarioPrestamo> Comentarios { get; set; }

    }
}
