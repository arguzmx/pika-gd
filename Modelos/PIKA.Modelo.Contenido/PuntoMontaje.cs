using Newtonsoft.Json;
using PIKA.Constantes.Aplicaciones.Contenido;
using PIKA.Infraestructura.Comun;
using PIKA.Modelo.Metadatos;
using PIKA.Modelo.Metadatos.Atributos;
using RepositorioEntidades;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace PIKA.Modelo.Contenido
{
    /// <summary>
    ///  Esta clase permite defiir puntos de montaje para una estructira jerarquica de carpetas
    /// </summary>
    [Entidad(PaginadoRelacional: false, EliminarLogico: true, 
    TokenApp: ConstantesAppContenido.APP_ID, TokenMod: ConstantesAppContenido.MODULO_ESTRUCTURA_CONTENIDO)]
    
    [EntidadVinculada(TokenSeguridad: ConstantesAppContenido.MODULO_ESTRUCTURA_CONTENIDO, 
        EntidadHijo: "Carpeta,Elemento",
        Cardinalidad: TipoCardinalidad.UnoVarios, PropiedadPadre: "Id",
        PropiedadHijo: "PuntoMontajeId", TipoDespliegueVinculo: TipoDespliegueVinculo.Jerarquico)]

    [EntidadVinculada(TokenSeguridad: ConstantesAppContenido.MODULO_ESTRUCTURA_CONTENIDO,
        EntidadHijo: "PermisosPuntoMontaje",
        Cardinalidad: TipoCardinalidad.UnoVarios, PropiedadPadre: "Id",
        PropiedadHijo: "PuntoMontajeId", TipoDespliegueVinculo: TipoDespliegueVinculo.Tabular)]
    public class PuntoMontaje : Entidad<string>, IEntidadRelacionada, 
        IEntidadNombrada, IEntidadEliminada, IEntidadRegistroCreacion
    {

        
        public string TipoOrigenDefault => ConstantesModelo.IDORIGEN_UNIDAD_ORGANIZACIONAL;

        public PuntoMontaje()
        {
            Carpetas = new HashSet<Carpeta>();
            VolumenesPuntoMontaje = new HashSet<VolumenPuntoMontaje>();
        }

        [Prop(Required: false, isId: true, Visible: false, OrderIndex: 0)]
        [VistaUI(ControlUI: ControlUI.HTML_HIDDEN, Accion: Acciones.update)]
        public override string Id { get => base.Id; set => base.Id = value; }


        /// <summary>
        /// Nomre único en el mismo nivel de loa jerarqu+ia para la carpeta
        /// </summary>
        [Prop(Required: true, OrderIndex: 10)]
        [VistaUI(ControlUI: ControlUI.HTML_TEXT, Accion: Acciones.addupdate)]
        [ValidString(minlen: 2, maxlen: 200)]
        public string Nombre { get; set; }

        /// <summary>
        /// Identifica si la carpeta ha sido eliminada
        /// </summary>
        [Prop(OrderIndex: 50, DefaultValue: "true")]
        [VistaUI(ControlUI: ControlUI.HTML_NONE, Accion: Acciones.none)]
        public bool Eliminada { get; set; }



        /// <summary>
        /// Identificador único del volumen por default para el punto de montaje
        /// </summary>
        [Prop(Required: false, OrderIndex: 70)]
        [VistaUI(ControlUI: ControlUI.HTML_SELECT, Accion: Acciones.addupdate)]
        [List(Entidad: "Volumen", DatosRemotos: true, TypeAhead: false)]
        public string VolumenDefaultId { get; set; }



        /// <summary>
        /// Identiicador único del creador del punto de montaje
        /// </summary>
        [Prop(Required: false, isId: true, Visible: false, OrderIndex: 1000)]
        [VistaUI(ControlUI: ControlUI.HTML_HIDDEN, Accion: Acciones.none)]
        [List(Entidad: "PropiedadesUsuario", DatosRemotos: true, TypeAhead: false)]
        public string CreadorId { get; set; }

        /// <summary>
        /// Fecha de creacón UTC del punto de montaje
        /// </summary>
        [Prop(Required: false, isId: true, Visible: false, OrderIndex: 1010)]
        [VistaUI(ControlUI: ControlUI.HTML_HIDDEN, Accion: Acciones.update)]
        public DateTime FechaCreacion { get; set; }



        /// <summary>
        /// Tipo de origen asociado al punto de montaje, en este caso es la unidad organizacional
        /// </summary>
        [Prop(Required: true, OrderIndex: 1000, Contextual: false, ShowInTable: false, Searchable: false, DefaultValue: ConstantesModelo.IDORIGEN_UNIDAD_ORGANIZACIONAL)]
        [VistaUI(ControlUI: ControlUI.HTML_HIDDEN, Accion: Acciones.addupdate)]
        public string TipoOrigenId { get; set; }

        /// <summary>
        /// Identificador único del origen, por ejemplo el Id de la Unidad Orgnizacional
        /// </summary>
        [Prop(Required: true, Visible: false, OrderIndex: 1010, ShowInTable: false, Contextual: true, IdContextual: ConstantesModelo.GLOBAL_UOID)]
        [VistaUI(ControlUI: ControlUI.HTML_HIDDEN, Accion: Acciones.addupdate)]
        public string OrigenId { get; set; }


        
        [XmlIgnore]
        [JsonIgnore]
        public ICollection<Carpeta> Carpetas { get; set; }

        [XmlIgnore]
        [JsonIgnore]
        public ICollection<Elemento> Elementos { get; set; }

        public Volumen VolumenDefault { get; set; }

        public ICollection<VolumenPuntoMontaje> VolumenesPuntoMontaje { get; set; }

        public ICollection<PermisosPuntoMontaje> PermisosPuntoMontaje { get; set; }

    }
}
