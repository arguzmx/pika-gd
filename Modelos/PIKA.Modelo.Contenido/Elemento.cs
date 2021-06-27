using PIKA.Constantes.Aplicaciones.Contenido;
using PIKA.Infraestructura.Comun;
using PIKA.Modelo.Metadatos;
using PIKA.Modelo.Metadatos.Atributos;
using RepositorioEntidades;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using System.Text.Json.Serialization;
using System.Xml.Serialization;

namespace PIKA.Modelo.Contenido
{
    [Entidad(PaginadoRelacional: false, EliminarLogico: true, AsociadoMetadatos: true,
    TokenApp: ConstantesAppContenido.APP_ID, TokenMod: ConstantesAppContenido.MODULO_ESTRUCTURA_CONTENIDO)]
    [LinkView(Titulo: "visor", Icono: "preview", Vista: "visorcontenido", Tipo: TipoVista.EventoApp)]
    [LinkView(Titulo: "buscarcontenido", Icono: "manage_search", Vista: "buscarcontenido", RequireSeleccion: false)]
    public class Elemento : Entidad<string>, IEntidadRegistroCreacion, 
        IEntidadEliminada, IEntidadNombrada, IEntidadRelacionada
    {
       
        public Elemento() {
            this.Versiones = new HashSet<Version>();
        }

        /// <summary>
        /// Indetificador único del elemento de contenido
        /// </summary>
        [Prop(Required: false, isId: true, Visible: false, OrderIndex: 0)]
        [VistaUI(ControlUI: ControlUI.HTML_HIDDEN, Accion: Acciones.update)]
        [LinkViewParameter(Vista: "visorcontenido")]
        public override string Id { get => base.Id; set => base.Id = value; }


        /// <summary>
        /// Sombre común dado al elemento de contenido
        /// </summary>
        [Prop(Required: true, OrderIndex: 10)]
        [VistaUI(ControlUI: ControlUI.HTML_TEXT, Accion: Acciones.addupdate)]
        [ValidString(minlen: 2, maxlen: 200)]
        [LinkViewParameter(Vista: "visorcontenido")]
        public string Nombre { get; set; }
        //#Requerido, longitud nombre

        /// <summary>
        /// Indica si el elemento ha sido eliminado de manera lógica
        /// </summary>
        [Prop(OrderIndex: 50, DefaultValue: "true")]
        [VistaUI(ControlUI: ControlUI.HTML_NONE, Accion: Acciones.none)]
        public bool Eliminada { get; set; }
        //#Requerido default=false

        /// <summary>
        /// Identificador del punto de montaje asociado a la carpeta
        /// </summary>
        [Prop(Required: true, Visible: false, HieRoot: true, OrderIndex: 1010, Contextual: true, IdContextual: ConstantesModelo.PREFIJO_CONEXTO + "PuntoMontaje")]
        [VistaUI(ControlUI: ControlUI.HTML_HIDDEN, Accion: Acciones.addupdate)]
        [LinkViewParameter(Vista: "visorcontenido")]
        public string PuntoMontajeId { get; set; }

        /// <summary>
        /// Identificador único del usuario que creó la entidad
        /// </summary>
        [Prop(Required: false, isId: true, Visible: false, OrderIndex: 1000)]
        [VistaUI(ControlUI: ControlUI.HTML_HIDDEN, Accion: Acciones.none)]
        [List(Entidad: "PropiedadesUsuario", DatosRemotos: true, TypeAhead: false)]
        public string CreadorId { get; set; }
        //#Requerido, GUID


        /// <summary>
        /// Fecah de creación de la entidad en formato UTC
        /// </summary>
        [Prop(Required: false, isId: true, Visible: false, OrderIndex: 1010)]
        [VistaUI(ControlUI: ControlUI.HTML_HIDDEN, Accion: Acciones.update)]
        public DateTime FechaCreacion { get; set; }
        //#Requerido

        ///// <summary>
        ///// Identificador único del volúmen para el contenido
        ///// </summary>
        [Prop(Required: false, OrderIndex: 70)]
        [VistaUI(ControlUI: ControlUI.HTML_SELECT, Accion: Acciones.add)]
        [List(Entidad: "Volumen", DatosRemotos: true, TypeAhead: false)]
        [LinkViewParameter(Vista: "visorcontenido")]
        [LinkMetadatos(CampoMetadatos: LinkMetadatosAttribute.IndiceFiltrado)]
        public string VolumenId { get; set; }

        /// <summary>
        /// Identificador único de la carpeta donde se creó el contenido
        /// </summary>
        [Prop(Required: false, Visible: false, OrderIndex: 1000, ShowInTable: false, Contextual: true, IdContextual: ConstantesModelo.PREFIJO_CONEXTO + "PadreId")]
        [VistaUI(ControlUI: ControlUI.HTML_HIDDEN, Accion: Acciones.addupdate)]
        [LinkViewParameter(Vista: "visorcontenido")]
        [LinkMetadatos(CampoMetadatos: LinkMetadatosAttribute.IndiceJerarquico)]
        public string CarpetaId { get; set; }
        

        /// <summary>
        /// Identificador únido del permiso asociado al elemeento
        /// </summary>
        [Prop(Required: false, isId: false, Visible: false, ShowInTable: false, OrderIndex: 1010)]
        [VistaUI(ControlUI: ControlUI.HTML_HIDDEN, Accion: Acciones.none)]
        public string PermisoId { get; set; }



        /// <summary>
        /// Determina el orien vinculado al elemento por ejemplo Carpeta de conteido, Activo de acervo etc
        /// </summary>
        [Prop(Required: false, OrderIndex: 1020, Visible: false, ShowInTable: false)]
        [VistaUI(ControlUI: ControlUI.HTML_NONE, Accion: Acciones.none)]
        [LinkViewParameter(Vista: "visorcontenido")]
        public string TipoOrigenId { get; set; }

        /// <summary>
        /// Identifiacdor del registor padre, en el caso de contenido se encuentra vacio, en otro tipo de catos 
        /// Puede ser por ejemplo Acervo
        /// </summary>
        [Prop(Required: true, OrderIndex: 1021, Visible: false, ShowInTable: false)]
        [VistaUI(ControlUI: ControlUI.HTML_NONE, Accion: Acciones.none)]
        [LinkViewParameter(Vista: "visorcontenido")]
        public string OrigenId { get; set; }

        /// <summary>
        /// Identifica si el elemento es administrado por versiones
        /// Todos los elementos tiene una versión inicial, 
        /// cuando se añaden nuevas versiones este campo toma el valor true
        /// </summary>
        [Prop(Required: false, isId: false, Visible: false, ShowInTable: false, OrderIndex: 1010, DefaultValue: "true" )]
        [VistaUI(ControlUI: ControlUI.HTML_HIDDEN, Accion: Acciones.addupdate)]
        public bool Versionado { get; set; }


        [Prop(Required: false, isId: false, Visible: false, ToggleInTable: false, OrderIndex: 1010)]
        [VistaUI(ControlUI: ControlUI.HTML_HIDDEN, Accion: Acciones.update)]
        [LinkViewParameter(Vista: "visorcontenido")]
        public string VersionId { get; set; }


        public string TipoOrigenDefault =>  "";


        [JsonIgnore, XmlIgnore]
        public Permiso Permiso { get; set; }
        
        [JsonIgnore, XmlIgnore]
        public virtual Volumen Volumen { get; set; }

        [JsonIgnore, XmlIgnore, NotMapped]
        public virtual ICollection<Version> Versiones { get; set; }
        
        [JsonIgnore, XmlIgnore, NotMapped]
        public virtual ICollection<Parte> Partes { get; set; }

        [JsonIgnore, XmlIgnore]
        public PuntoMontaje PuntoMontaje { get; set; }

    }
}
