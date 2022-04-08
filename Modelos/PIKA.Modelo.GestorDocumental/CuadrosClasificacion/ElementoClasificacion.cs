using RepositorioEntidades;
using System;
using System.Collections.Generic;
using System.Text;
using PIKA.Infraestructura.Comun;
using PIKA.Modelo.Metadatos;
using PIKA.Modelo.Metadatos.Atributos;
using System.Xml.Serialization;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using PIKA.Constantes.Aplicaciones.GestorDocumental;

namespace PIKA.Modelo.GestorDocumental
{

    /// <summary>
    /// Los elementos de clasificación consituyen la estructura jerárquca para acaomodar las entradas del cuadro
    /// </summary>
    [Entidad( EliminarLogico: true,
        TokenMod: ConstantesAppGestionDocumental.MODULO_CUADROCLASIF,
        TokenApp: ConstantesAppGestionDocumental.APP_ID)]
    public class ElementoClasificacion : Entidad<string>, IEntidadNombrada, 
        IEntidadEliminada, IEntidadJerarquica
    {

        public ElementoClasificacion() {
            Padre = null;
            Hijos = new HashSet<ElementoClasificacion>();
            Entradas = new HashSet<EntradaClasificacion>();
        }

        [Prop(Required: false, isId: true, Visible: false, OrderIndex: 10, HieId: true)]
        [VistaUI(ControlUI: ControlUI.HTML_HIDDEN, Accion: Acciones.update)]
        public override string Id { get => base.Id; set => base.Id = value; }


        /// <summary>
        /// Clave asociada el elemento de clasifaición por ejemplo 1.S
        /// </summary>
        [Prop(Required: true, OrderIndex: 20)]
        [VistaUI(ControlUI: ControlUI.HTML_TEXT, Accion: Acciones.addupdate)]
        [ValidString(minlen: 2, maxlen: 200)]
        public string Clave { get; set; }

        /// <summary>
        /// Nombre del elemento de clasifiación debe ser único en para los elemntos de un mismo padre
        /// </summary>
        [Prop(Required: true, OrderIndex: 30)]
        [VistaUI(ControlUI: ControlUI.HTML_TEXT, Accion: Acciones.addupdate)]
        [ValidString(minlen: 2, maxlen: 200)]
        public string Nombre { get; set; }

    

        /// <summary>
        /// Indica si el elemento ha sido marcado para eliminaciónrai
        /// </summary>
        [Prop(Required: false, OrderIndex: 100)]
        [VistaUI(ControlUI: ControlUI.HTML_HIDDEN, Accion: Acciones.none)]
        public bool Eliminada { get; set; }


  
        /// <summary>
        /// Determina la posición relativa del elemento en relación con los elementos del mismo nivle
        /// </summary>

        [Prop(Required: false, OrderIndex: 100)]
        [VistaUI(ControlUI: ControlUI.HTML_HIDDEN, Accion: Acciones.none)]
        [ValidString(minlen: 2, maxlen: 200)]
        public int Posicion { get; set; }

        /// <summary>
        /// Cuadro de clasificación al que pertenecen los elementos
        /// </summary>
        [Prop(Required: true, Visible: false, HieRoot: true, OrderIndex: 1010, Contextual: true, IdContextual: ConstantesModelo.PREFIJO_CONEXTO + "CuadroClasificacion")]
        [VistaUI(ControlUI: ControlUI.HTML_HIDDEN, Accion: Acciones.addupdate)]
        public string CuadroClasifiacionId { get; set; }


        /// <summary>
        /// Padre  del elemento actual 
        /// </summary>
        [Prop(Required: false, Visible: false, OrderIndex: 1020, HieParent: true,
            Contextual: true, IdContextual: ConstantesModelo.PREFIJO_CONEXTO + "PadreId")]
        [VistaUI(ControlUI: ControlUI.HTML_HIDDEN, Accion: Acciones.addupdate)]
        public string ElementoClasificacionId { get; set; }

        /// <summary>
        /// Indica si el elemento es un elemnto raiz
        /// </summary>
        [Prop(Required: true, Visible: false, OrderIndex: 1020, Contextual: true, IdContextual: ConstantesModelo.CONTEXTO_ESRAIZ)]
        [VistaUI(ControlUI: ControlUI.HTML_HIDDEN, Accion: Acciones.addupdate)]
        public bool EsRaiz { get; set; }


        [NotMapped]
        [Prop(Required: false, Visible: false, OrderIndex: 1050,  ShowInTable: false, HieName: true)]
        [VistaUI()]
        public string NombreJerarquico { get {
                return this.Nombre;
            } }

        /// <summary>
        /// Elemento padre del actual
        /// </summary>
        [XmlIgnore]
        [JsonIgnore]
        public virtual ElementoClasificacion Padre { get; set; }

        /// <summary>
        /// Elementos descencientes del elemento actual
        /// </summary>
        [XmlIgnore]
        [JsonIgnore]
        public virtual ICollection<ElementoClasificacion> Hijos { get; set; }

        /// <summary>
        /// Instancia del cuadro de clasificación
        /// </summary>
        [XmlIgnore]
        [JsonIgnore]
        public virtual CuadroClasificacion CuadroClasificacion { get; set; }


        /// <summary>
        /// Activos del elemento clasificacion
        /// </summary>
        [XmlIgnore]
        [JsonIgnore]
        public virtual ICollection<EntradaClasificacion> Entradas { get; set; }
    }
}
