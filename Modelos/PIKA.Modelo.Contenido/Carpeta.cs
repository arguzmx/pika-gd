using RepositorioEntidades;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;
using System.Xml.Serialization;
using PIKA.Infraestructura.Comun;
using PIKA.Modelo.Metadatos;
using PIKA.Modelo.Metadatos.Atributos;

namespace PIKA.Modelo.Contenido
{
    /// <summary>
    /// Proporciona una estructura jerárquica de folders para situar elementos del contenido
    /// </summary>
    [Entidad(EliminarLogico: true)]
    [EntidadVinculada(EntidadHijo: "Elemento", Cardinalidad: TipoCardinalidad.UnoVarios, PropiedadPadre: "Id", PropiedadHijo: "CarpetaId")]
    public class Carpeta : Entidad<string>, IEntidadNombrada, IEntidadEliminada, IEntidadRegistroCreacion
    {
        public Carpeta() {
            Subcarpetas = new HashSet<Carpeta>();
        }

        /// <summary>
        /// Identificador del punto de montaje asociado a la carpeta
        /// </summary>
        [Prop(Required: false, Visible: false, OrderIndex: 0 )]
        [VistaUI(ControlUI: ControlUI.HTML_HIDDEN, Accion: Acciones.addupdate)]
        public string PuntoMontajeId { get; set; }


        /// <summary>
        /// Identificador único de la carptea
        /// </summary>
        [Prop(Required: false, isId: true, Visible: false, OrderIndex: 10)]
        [VistaUI(ControlUI: ControlUI.HTML_HIDDEN, Accion: Acciones.update)]
        public override string Id { get; set; }

        /// <summary>
        /// Identificador único  del creador de la carpeta
        /// </summary>
        [Prop(Required: false, Visible: false, OrderIndex: 20)]
        [VistaUI(ControlUI: ControlUI.HTML_HIDDEN, Accion: Acciones.none)]
        public string CreadorId { get; set; }

        /// <summary>
        /// FEcha de creación en formato UTC
        /// </summary>
        [Prop(Required: false, Visible: false, OrderIndex: 30)]
        [VistaUI(ControlUI: ControlUI.HTML_HIDDEN, Accion: Acciones.none)]
        public DateTime FechaCreacion { get; set; }

        /// <summary>
        /// Nombre para la carpeta 
        /// </summary>
        [Prop(Required: true, OrderIndex: 50)]
        [VistaUI(ControlUI: ControlUI.HTML_TEXT, Accion: Acciones.addupdate)]
        [ValidString(minlen: 2, maxlen: 200)]
        public string Nombre { get; set; }
        // #Longitud 512

        /// <summary>
        /// Identifica si la carpeta ha sido eliminada
        /// </summary>
        [Prop(OrderIndex: 60, DefaultValue: "false")]
        [VistaUI(ControlUI: ControlUI.HTML_TOGGLE, Accion: Acciones.update)]
        public bool Eliminada { get; set; }

        /// <summary>
        /// Identificador de la carpeta padre de la actual 
        /// </summary>
        [Prop(Required: true, OrderIndex: 70)]
        [VistaUI(ControlUI: ControlUI.HTML_HIDDEN, Accion: Acciones.addupdate)]
        [ValidString(minlen: 2, maxlen: 200)]
        public string CarpetaPadreId { get; set; }
        // Es opcional 

        /// <summary>
        /// Identificador único del permiso a la carpeta
        /// </summary>
        [Prop(OrderIndex: 80, DefaultValue: "false")]
        [VistaUI(ControlUI: ControlUI.HTML_HIDDEN, Accion: Acciones.none)]
        public string PermisoId { get; set; }
        // Es opcional


        /// <summary>
        /// Determina si la carpeta es un nodo raíz, en esta caso la propiedad CarpetaPadreId debe ser nula
        /// </summary>
        [Prop(OrderIndex: 90, DefaultValue: "false")]
        [VistaUI(ControlUI: ControlUI.HTML_TOGGLE, Accion: Acciones.update)]
        public bool? EsRaiz { get; set; }


        public Permiso Permiso { get; set; }

        [XmlIgnore]
        [JsonIgnore]
        public Carpeta CarpetaPadre { get; set; }
       
        public virtual ICollection<Carpeta> Subcarpetas { get; set; }

        [XmlIgnore]
        [JsonIgnore]
        public PuntoMontaje PuntoMontaje { get; set; }

    }
}
