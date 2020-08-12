using PIKA.Infraestructura.Comun;
using PIKA.Modelo.Metadatos;
using PIKA.Modelo.Metadatos.Atributos;
using RepositorioEntidades;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;
using System.Xml.Serialization;

namespace PIKA.Modelo.GestorDocumental
{
    [Entidad(PaginadoRelacional:false, EliminarLogico: true)]
    [EntidadVinculada(EntidadHijo: "ElementoClasificacion,EntradaClasificacion", 
        Cardinalidad: TipoCardinalidad.UnoVarios, PropiedadPadre: "Id", 
        PropiedadHijo: "CuadroClasifiacionId", TipoDespliegueVinculo: TipoDespliegueVinculo.Jerarquico) ]
    public class CuadroClasificacion : Entidad<string>, IEntidadNombrada, IEntidadEliminada, IEntidadRelacionada
    {

        [XmlIgnore]
        [JsonIgnore]
        public string TipoOrigenDefault => ConstantesModelo.IDORIGEN_DOMINIO;

        public CuadroClasificacion()
        {
            this.TipoOrigenId = this.TipoOrigenDefault;
            this.Elementos = new HashSet<ElementoClasificacion>();
        }


        /// <summary>
        /// Identificador únioc del cuadro de clasificación
        /// </summary>

        [Prop(Required: false, isId: true, Visible: false, OrderIndex: 0)]
        [VistaUI(ControlUI: ControlUI.HTML_HIDDEN, Accion: Acciones.update)]
        public override string Id { get => base.Id; set => base.Id = value; }

        /// <summary>
        /// Nombre único del cuadro de clasificación
        /// </summary>
        [Prop(Required: true, OrderIndex: 5)]
        [VistaUI(ControlUI: ControlUI.HTML_TEXT, Accion: Acciones.addupdate)]
        [ValidString(minlen: 2, maxlen: 200)]
        public string Nombre { get ; set ; }

        /// <summary>
        /// Identificaidor del estado del cuadro de clasificación
        /// </summary>
        [Prop(Required: false, OrderIndex: 10)]
        [VistaUI(ControlUI: ControlUI.HTML_SELECT, Accion: Acciones.update)]
        [List(Entidad: "EstadoCuadroClasificacion", DatosRemotos: true, TypeAhead: false, 
            Default: EstadoCuadroClasificacion.ESTADO_ACTIVO)]
        public string EstadoCuadroClasificacionId { get; set; }

        /// <summary>
        /// Especifica si el elemento ha sido marcado como eliminado
        /// </summary>
        [Prop(Required: false, OrderIndex: 15, DefaultValue: "false")]
        [VistaUI(ControlUI: ControlUI.HTML_TOGGLE, Accion: Acciones.none)]
        public bool Eliminada { get; set; }


        /// <summary>
        /// El tipo de orígen en para este modelo es el dominio a la que pertenece el cuadro de clasificacion
        /// </summary>
        [Prop(Required: true, OrderIndex: 1000, Contextual: true, ShowInTable: false, Searchable: false, DefaultValue: ConstantesModelo.IDORIGEN_DOMINIO)]
        [VistaUI(ControlUI: ControlUI.HTML_HIDDEN, Accion: Acciones.addupdate)]
        public string TipoOrigenId { get; set; }


        /// <summary>
        /// Identificador de la organización a la que pertenece el cuadro de clasificación
        /// </summary>
        [Prop(Required: true, Visible: false, OrderIndex: 1010, Contextual: true, IdContextual: ConstantesModelo.GLOBAL_DOMINIOID)]
        [VistaUI(ControlUI: ControlUI.HTML_HIDDEN, Accion: Acciones.addupdate)]
        public string OrigenId { get; set; }




        /// <summary>
        /// Esatdo del cuadro de clasificación
        /// </summary>
        public virtual EstadoCuadroClasificacion Estado { get; set; }

        [JsonIgnore]
        [XmlIgnore]
        public virtual ICollection<ElementoClasificacion> Elementos { get; set; }

    }
}
