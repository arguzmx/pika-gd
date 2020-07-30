using RepositorioEntidades;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using System.Xml.Serialization;
using PIKA.Infraestructura.Comun;
using PIKA.Modelo.Metadatos;
using PIKA.Modelo.Metadatos.Atributos;
using System.Reflection.Emit;

namespace PIKA.Modelo.Contenido
{
    /// <summary>
    /// Define el espacio físico al que serán anexados las partes de un contenido
    /// </summary>
    [Entidad(EliminarLogico: true)]
    public class Volumen : Entidad<string>, IEntidadNombrada, IEntidadEliminada, IEntidadRelacionada
    {

        public string TipoOrigenDefault => ConstantesModelo.IDORIGEN_DOMINIO;

        public Volumen() {
            PuntosMontajeVolumen = new HashSet<VolumenPuntoMontaje>();
        }

        /// <summary>
        ///  Identificdor únio del volumen
        /// </summary>
        [Prop(Required: false, isId: true, Visible: false, OrderIndex: 0)]
        [VistaUI(ControlUI: ControlUI.HTML_HIDDEN, Accion: Acciones.update)]
        public override string Id { get => base.Id; set => base.Id = value; }


        /// <summary>
        /// Nombre únicop del volumen
        /// </summary>
        [Prop(Required: true, OrderIndex: 10)]
        [VistaUI(ControlUI: ControlUI.HTML_TEXT, Accion: Acciones.addupdate)]
        [ValidString(minlen: 2, maxlen: 200)]
        public string Nombre { get; set; }
        

        /// <summary>
        /// Identificador único del  tipo de gestor, es necesario para la configuración
        /// </summary>
        [Prop(Required: true, OrderIndex: 20)]
        [VistaUI(ControlUI: ControlUI.HTML_SELECT, Accion: Acciones.addupdate)]
        [List(Entidad: "TipoGestorES", DatosRemotos: true, TypeAhead: false)]
        public string TipoGestorESId { get; set; }

        /// <summary>
        /// Tamaño maximo del volumen en bytes, 0 indidica ilimitado
        /// </summary>
        [Prop(Required: true, OrderIndex: 25)]
        [VistaUI(ControlUI: ControlUI.HTML_NUMBER, Accion: Acciones.addupdate)]
        [ValidNumeric(min: 0, max: 0, defaulvalue: 0, usemin: true, usemax: false)]
        public long TamanoMaximo { get; set; }

        /// <summary>
        /// Indica si el volumen se encuentra activo 
        /// </summary>
        [Prop(Required: true, OrderIndex: 30, DefaultValue: "true")]
        [VistaUI(ControlUI: ControlUI.HTML_TOGGLE, Accion: Acciones.update)]
        public bool Activo { get; set; }

        /// <summary>
        /// Especifica si el volumen se encuntra habilitado para escritura
        /// </summary>
        [Prop(Required: true, OrderIndex: 40, DefaultValue: "true")]
        [VistaUI(ControlUI: ControlUI.HTML_TOGGLE, Accion: Acciones.update)]
        public bool EscrituraHabilitada { get; set; }

 

        /// <summary>
        /// Identifica sie l volumen ha sido marcado como eliminadp
        /// </summary>
        [Prop(OrderIndex: 50, DefaultValue: "true")]
        [VistaUI(ControlUI: ControlUI.HTML_NONE, Accion: Acciones.none)]
        public bool Eliminada { get; set; }



        /// <summary>
        /// Consecutivo del elemento para el alamcenamiento, esta propieda tambié existe en la Parte del contenido 
        /// para poder asociar un Id de tipo String, con uno númerio si es necesario
        /// </summary>
        [Prop(OrderIndex: 60)]
        [VistaUI(ControlUI: ControlUI.HTML_NONE, Accion: Acciones.none)]
        public long ConsecutivoVolumen { get; set; }

        /// <summary>
        /// Número de partess contenidas en el volumen
        /// </summary>
        [Prop(OrderIndex: 70)]
        [VistaUI(ControlUI: ControlUI.HTML_NONE, Accion: Acciones.none)]
        public long CanidadPartes { get; set; }


        /// <summary>
        /// Número de elementos contenidas en el volumen
        /// </summary>
        [Prop(OrderIndex: 80)]
        [VistaUI(ControlUI: ControlUI.HTML_NONE, Accion: Acciones.none)]
        public long CanidadElementos { get; set; }

        /// <summary>
        /// Tamaño de volumen en bytes
        /// </summary>
        [Prop(OrderIndex: 90)]
        [VistaUI(ControlUI: ControlUI.HTML_NONE, Accion: Acciones.none)]
        public long Tamano { get; set; }

        /// <summary>
        /// Atributo de uso interno queindica se la configuración del volumen es válida
        /// </summary>
        [Prop(OrderIndex: 100)]
        [VistaUI(ControlUI: ControlUI.HTML_NONE, Accion: Acciones.none)]
        public bool ConfiguracionValida { get; set; }



        /// <summary>
        /// Tipo de origen para el volumen puede ser la Unidaorganizacionla o el Dominio dependiendo del grado de 
        /// visibilidad del volumen
        /// </summary>
        [Prop(Required: true, OrderIndex: 1000, Contextual: true, ShowInTable: false, Searchable: false, DefaultValue: ConstantesModelo.IDORIGEN_DOMINIO)]
        [VistaUI(ControlUI: ControlUI.HTML_HIDDEN, Accion: Acciones.addupdate)]
        public string TipoOrigenId { get; set; }

        /// <summary>
        /// Identificador único del origen, por ejempl el ID de la UO o el dominio
        /// </summary>
        [Prop(Required: true, OrderIndex: 1010, Contextual: true, ShowInTable: false, Searchable: false)]
        [VistaUI(ControlUI: ControlUI.HTML_HIDDEN, Accion: Acciones.addupdate)]
        public string OrigenId { get; set; }

        

        /// <summary>
        /// Esta propedad eno debe serializarse en la base de datos
        /// </summary>
        [JsonIgnore]
        [XmlIgnore]
        public TipoGestorES TipoGestorES { get; set; }

        [XmlIgnore]
        [JsonIgnore]
        public ICollection<VolumenPuntoMontaje> PuntosMontajeVolumen { get; set; }

        [XmlIgnore]
        [JsonIgnore]
        public ICollection<PuntoMontaje> PuntosMontaje { get; set; }

        [XmlIgnore]
        [JsonIgnore]
        public ICollection<Elemento> Elementos { get; set; }

        [XmlIgnore]
        [JsonIgnore]
        public GestorAzureConfig AxureConfig { get; set; }

        [XmlIgnore]
        [JsonIgnore]
        public GestorLocalConfig LocalConfig { get; set; }

        [XmlIgnore]
        [JsonIgnore]
        public GestorSMBConfig SMBConfig { get; set; }
    }

}
