using PIKA.Infraestructura.Comun;
using PIKA.Modelo.Metadatos;
using PIKA.Modelo.Metadatos.Atributos;
using RepositorioEntidades;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;
using System.Xml.Serialization;
using static PIKA.Modelo.Metadatos.EventAttribute;

namespace PIKA.Modelo.Contacto
{

    /// <summary>
    /// Detalla la ubicació espacial de un elemento en forma de una dirección postal
    /// Las direcciones postales se  enlazan a los objeto a através de una entidad de relación
    /// con la finalidad de establecer relaciones varios a varios
    /// </summary>

    [Entidad(PaginadoRelacional: true, EliminarLogico: false)]
    public class DireccionPostal : Entidad<string>, IEntidadNombrada, IEntidadRelacionada
    {
     
        [XmlIgnore]
        [JsonIgnore]
        public string TipoOrigenDefault => ConstantesModelo.IDORIGEN_NULO;


        public DireccionPostal()
        {
            this.TipoOrigenId = this.TipoOrigenDefault;
        }


        /// <summary>
        /// Identificador únioc de la dirección postal
        /// </summary>

        [Prop(Required: false, isId: true, Visible: false, OrderIndex: 0)]
        [VistaUI(ControlUI: ControlUI.HTML_HIDDEN, Accion: Acciones.update)]
        public override string Id { get => base.Id; set => base.Id = value; }


        /// <summary>
        /// NOmbre corto para reconocer la dirección por ejemplo: Casa, Oficina, etc
        /// </summary>

        [Prop(Required: true, OrderIndex: 5)]
        [VistaUI(ControlUI: ControlUI.HTML_TEXT, Accion: Acciones.addupdate)]
        [ValidString(minlen: 2, maxlen: 200)]
        public string Nombre { get; set; }


        /// <summary>
        /// Calle de la dirección
        /// </summary>
        [Prop(Required: true, OrderIndex: 10)]
        [VistaUI(ControlUI: ControlUI.HTML_TEXTAREA, Accion: Acciones.addupdate)]
        [ValidString(minlen: 1, maxlen: 200)]
        public string Calle { get; set; }

        /// <summary>
        /// No. interior de la dirección
        /// </summary>
        [Prop(Required: false, OrderIndex: 20)]
        [VistaUI(ControlUI: ControlUI.HTML_TEXT, Accion: Acciones.addupdate)]
        [ValidString(minlen: 0, maxlen: 200)]
        public string NoInterno { get; set; }


        /// <summary>
        /// No. exterior de la dirección
        /// </summary>
        [Prop(Required: false, OrderIndex: 30)]
        [VistaUI(ControlUI: ControlUI.HTML_TEXT, Accion: Acciones.addupdate)]
        [ValidString(minlen: 0, maxlen: 200)]
        public string NoExterno { get; set; }


        /// <summary>
        /// Colonia o barrio de la dirección
        /// </summary>
        [Prop(Required: false, OrderIndex: 40)]
        [VistaUI(ControlUI: ControlUI.HTML_TEXT, Accion: Acciones.addupdate)]
        [ValidString(minlen: 1, maxlen: 200)]
        public string Colonia { get; set; }


        /// <summary>
        /// Código postal de la dirección
        /// </summary>
        [Prop(Required: false, OrderIndex: 50)]
        [VistaUI(ControlUI: ControlUI.HTML_TEXT, Accion: Acciones.addupdate)]
        [ValidString(minlen: 5, maxlen: 10, regexp: ValidStringAttribute.REGEXP_NUMBERS)]
        public string CP { get; set; }


        /// <summary>
        /// Municipio o alcaldía de la dirección
        /// </summary>
        [Prop(Required: false, OrderIndex: 60)]
        [VistaUI(ControlUI: ControlUI.HTML_TEXT, Accion: Acciones.addupdate)]
        [ValidString(minlen: 1, maxlen: 200)]
        public string Municipio { get; set; }



        /// <summary>
        /// País de la dirección
        /// </summary>
        [Prop(Required: false, OrderIndex: 70)]
        [VistaUI(ControlUI: ControlUI.HTML_SELECT, Accion: Acciones.addupdate)]
        [List(Entidad: "Pais", DatosRemotos: true, TypeAhead: false, Default: "MEX")]
        public string PaisId { get; set; }



        /// <summary>
        /// Estado del país de la dirección
        /// </summary>
        [Prop(Required: false, OrderIndex: 80)]
        [VistaUI(ControlUI: ControlUI.HTML_SELECT, Accion: Acciones.addupdate)]
        [List(Entidad: "Estado", DatosRemotos: true, TypeAhead: false )]
        [Event(Entidad: "PaisId", Evento: Eventos.AlCambiar, Operacion: Operaciones.Actualizar, "PaisId")]
        public string EstadoId { get; set; }


        /// <summary>
        /// Identifica si la dirección postal es la dirección por defecto para el origen
        /// </summary>
        [Prop(Required: false, OrderIndex: 90, DefaultValue: "false")]
        [VistaUI(ControlUI: ControlUI.HTML_TOGGLE, Accion: Acciones.addupdate)]
        public bool EsDefault { get; set; }

        /// <summary>
        /// Tipo de ojeto origen asociado a la dirección, pr ejemplo UO
        /// </summary>
        [Prop(Required: true, OrderIndex: 100, Contextual: true, ShowInTable: false, Searchable: false)]
        [VistaUI(ControlUI: ControlUI.HTML_HIDDEN, Accion: Acciones.addupdate)]
        public string TipoOrigenId { get; set; }


        /// <summary>
        /// Identificador único del objeto origen, por ejemplo el id de una UO
        /// </summary>
        [Prop(Required: true, OrderIndex: 110, Contextual: true, ShowInTable: false,  Searchable: false)]
        [VistaUI(ControlUI: ControlUI.HTML_HIDDEN, Accion: Acciones.addupdate)]
        public string OrigenId { get; set; }


        public virtual Pais Pais { get; set; }

        public virtual Estado Estado { get; set; }
    }
}
