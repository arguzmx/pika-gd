using RepositorioEntidades;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;
using System.Xml.Serialization;
using PIKA.Infraestructura.Comun;
using PIKA.Modelo.Metadatos;
using PIKA.Modelo.Metadatos.Atributos;
using System.ComponentModel.DataAnnotations.Schema;
using PIKA.Constantes.Aplicaciones.Contenido;

namespace PIKA.Modelo.Contenido
{
    /// <summary>
    /// Proporciona una estructura jerárquica de folders para situar elementos del contenido
    /// </summary>
    // [EntidadVinculada(EntidadHijo: "Elemento", Cardinalidad: TipoCardinalidad.UnoVarios, PropiedadPadre: "Id", PropiedadHijo: "CarpetaId")]
    [Entidad(PaginadoRelacional: false, EliminarLogico: true,
TokenApp: ConstantesAppContenido.APP_ID, TokenMod: ConstantesAppContenido.MODULO_ESTRUCTURA_CONTENIDO)]
    public class Carpeta : Entidad<string>, IEntidadNombrada, 
        IEntidadEliminada, IEntidadRegistroCreacion, IEntidadJerarquica
    {
        public Carpeta() {
            Subcarpetas = new HashSet<Carpeta>();
        }

        /// <summary>
        /// Identificador del punto de montaje asociado a la carpeta
        /// </summary>
        [Prop(Required: true, Visible: false, HieRoot: true, OrderIndex: 1010, Contextual: true, IdContextual: ConstantesModelo.PREFIJO_CONEXTO + "PuntoMontaje")]
        [VistaUI(ControlUI: ControlUI.HTML_HIDDEN, Accion: Acciones.addupdate)]
        public string PuntoMontajeId { get; set; }


        /// <summary>
        /// Identificador único de la carptea
        /// </summary>
        [Prop(Required: false, isId: true, Visible: false, OrderIndex: 10, HieId: true)]
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
        [VistaUI(ControlUI: ControlUI.HTML_HIDDEN, Accion: Acciones.update)]
        public bool Eliminada { get; set; }

        /// <summary>
        /// Identificador de la carpeta padre de la actual 
        /// </summary>
        [Prop(Required: false, Visible: false, OrderIndex: 70, HieParent: true,
            Contextual: true, IdContextual: ConstantesModelo.PREFIJO_CONEXTO + "PadreId")]
        [VistaUI(ControlUI: ControlUI.HTML_HIDDEN, Accion: Acciones.addupdate)]
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
        [Prop(Required: true, Visible: false, OrderIndex: 1020, Contextual: true, IdContextual: ConstantesModelo.CONTEXTO_ESRAIZ)]
        [VistaUI(ControlUI: ControlUI.HTML_HIDDEN, Accion: Acciones.addupdate)]
        public bool EsRaiz { get; set; }


        public Permiso Permiso { get; set; }

        [XmlIgnore]
        [JsonIgnore]
        public Carpeta CarpetaPadre { get; set; }
       
        public virtual ICollection<Carpeta> Subcarpetas { get; set; }

        [XmlIgnore]
        [JsonIgnore]
        public PuntoMontaje PuntoMontaje { get; set; }

        [NotMapped]
        [Prop(Required: false, Visible: false, OrderIndex: 1050, ShowInTable: false, HieName: true)]
        [VistaUI()]
        public string NombreJerarquico
        {
            get
            {
                return this.Nombre;
            }
        }

   
    }
}
