using Microsoft.AspNetCore.Routing.Internal;
using PIKA.Modelo.Metadatos.Atributos;
using RepositorioEntidades;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using System.Text.Json.Serialization;
using System.Xml.Serialization;
using static PIKA.Modelo.Metadatos.PropAttribute;

namespace PIKA.Modelo.Metadatos
{

    /// <summary>
    /// Define una propiedad de meatdatos, se basa en el atributo web PIKA.modelo.Metadatos.PropAttribue
    /// Culauqier cambio en alguno de los 2 debe sincornizarse manualemnte
    /// </summary>
    public class Propiedad : Entidad<string>, IEntidadNombrada
    {

        public Propiedad()
        {
            this.OrdenarValoresListaPorNombre = true;
            //this.ValoresLista = new HashSet<ValorLista>();
            this.AtributosEvento = new List<AtributoEvento>();
            this.AtributosVistaUI = new List<AtributoVistaUI>();
            this.ValorDefault = null;
        }

        /// <summary>
        /// Identificador único de la propiedad
        /// </summary>
        [Prop(Required: false, isId: true, Visible: false, OrderIndex: 0)]
        [VistaUI(ControlUI: ControlUI.HTML_HIDDEN, Accion: Acciones.update)]
        public override string Id { get; set; }

        /// <summary>
        /// Nombre asociado a la propiedad
        /// </summary>
        [Prop(Required: true, OrderIndex: 5)]
        [VistaUI(ControlUI: ControlUI.HTML_TEXT, Accion: Acciones.addupdate)]
        [ValidString(minlen: 2, maxlen: 200)]
        public string Nombre { get; set; }


        /// <summary>
        /// Identificador del tipo de propiedad
        /// </summary>
        [Prop(Required: false, OrderIndex: 10)]
        [VistaUI(ControlUI: ControlUI.HTML_SELECT, Accion: Acciones.add)]
        [List(Entidad: "TipoDato", DatosRemotos: true, TypeAhead: false)]
        public string TipoDatoId { get; set; }


        /// <summary>
        /// Valor por defecto de la propiedad serializado como string
        /// en el caso de los binarios debe serializarrse como base 64
        /// </summary>
        [Prop(Required: false, OrderIndex: 15)]
        [VistaUI(ControlUI: ControlUI.HTML_TEXT, Accion: Acciones.addupdate)]
        [ValidString(minlen: 0, maxlen: 200)]
        public string ValorDefault { get; set; }

        /// <summary>
        /// DEtermina si es requerido
        /// </summary>
        [Prop(Required: true, OrderIndex: 30, DefaultValue: "true")]
        [VistaUI(ControlUI: ControlUI.HTML_TOGGLE, Accion: Acciones.addupdate)]
        public bool Requerido { get; set; }

        /// <summary>
        /// Determina si el valos es un campo de índice 
        /// </summary>
        [Prop(Required: false, OrderIndex: 40)]
        [VistaUI(ControlUI: ControlUI.HTML_TOGGLE, Accion: Acciones.none)]
        public bool EsIndice { get; set; }

        /// <summary>
        /// Determina si es posible realizar bpsuquedas sobre la propiedad
        /// </summary>
        [Prop(Required: false, OrderIndex: 50)]
        [VistaUI(ControlUI: ControlUI.HTML_TOGGLE, Accion: Acciones.addupdate)]
        public bool Buscable { get; set; }

        /// <summary>
        /// Determina si es posible realizar ordenamiento sobre la propiedad
        /// </summary>
        [Prop(Required: false, OrderIndex: 60)]
        [VistaUI(ControlUI: ControlUI.HTML_TOGGLE, Accion: Acciones.addupdate)]
        public bool Ordenable { get; set; }

        /// <summary>
        /// Determina si la propieda es vivisble
        /// </summary>
        [Prop(Required: true, OrderIndex: 70, DefaultValue: "false")]
        [VistaUI(ControlUI: ControlUI.HTML_TOGGLE, Accion: Acciones.update)]
        public bool Visible { get; set; }


        /// <summary>
        /// Índice de despliegue para la propiedad
        /// </summary>
        [Prop(Required: false, OrderIndex: 50)]
        [VistaUI(ControlUI: ControlUI.HTML_NUMBER, Accion: Acciones.update)]
        [ValidNumeric(min: 0, max: 0, defaulvalue: 0, usemin: true, usemax: false)]
        public int IndiceOrdenamiento { get; set; }


        /// <summary>
        /// DEtermina si es una clave primaria
        /// </summary>
        [Prop(Required: false, OrderIndex: 1000, ShowInTable: false)]
        [VistaUI(ControlUI: ControlUI.HTML_NONE, Accion: Acciones.none)]
        public bool EsIdClaveExterna { get; set; }

        /// <summary>
        /// Dettermina si es el identificador de un registro
        /// </summary>
        [Prop(Required: false, OrderIndex: 1000, ShowInTable: false)]
        [VistaUI(ControlUI: ControlUI.HTML_NONE, Accion: Acciones.none)]
        public bool EsIdRegistro { get; set; }

        /// <summary>
        /// Dettermina si es el identificador de una jeraquía
        /// </summary>
        [Prop(Required: false, OrderIndex: 1000, ShowInTable: false)]
        [VistaUI(ControlUI: ControlUI.HTML_NONE, Accion: Acciones.none)]
        public bool EsIdJerarquia { get; set; }

        /// <summary>
        /// Dettermina si coreresponde al texto de un registro
        /// </summary>
        [Prop(Required: false, OrderIndex: 1000, ShowInTable: false)]
        [VistaUI(ControlUI: ControlUI.HTML_NONE, Accion: Acciones.none)]
        public bool EsTextoJerarquia { get; set; }

        /// <summary>
        /// Dettermina si coreresponde a la propiedad padre para una relacion jerárquica
        /// </summary>
        [Prop(Required: false, OrderIndex: 1000, ShowInTable: false)]
        [VistaUI(ControlUI: ControlUI.HTML_NONE, Accion: Acciones.none)]
        public bool EsIdRaizJerarquia { get; set; }


        /// <summary>
        /// Dettermina si coreresponde a la propiedad padre para una relacion jerárquica
        /// </summary>
        [Prop(Required: false, OrderIndex: 1000, ShowInTable: false)]
        [VistaUI(ControlUI: ControlUI.HTML_NONE, Accion: Acciones.none)]
        public bool EsFiltroJerarquia { get; set; }

        /// <summary>
        /// Determina si el valos es autogenerado
        /// </summary>
        [Prop(Required: false, OrderIndex: 1000, ShowInTable: false)]
        [VistaUI(ControlUI: ControlUI.HTML_NONE, Accion: Acciones.none)]
        public bool Autogenerado { get; set; }


        [Prop(Required: false, OrderIndex: 1000, ShowInTable: false)]
        [VistaUI(ControlUI: ControlUI.HTML_NONE, Accion: Acciones.none)]
        public bool OrdenarValoresListaPorNombre { get; set; }

        /// <summary>
        /// Especifica el control de HTML preferido
        /// </summary>
        [Prop(Required: false, OrderIndex: 1000, ShowInTable: false)]
        [VistaUI(ControlUI: ControlUI.HTML_NONE, Accion: Acciones.none)]

        public string ControlHTML { get; set; }

        /// <summary>
        /// establece la posición del campo en el despliegue tabular
        /// </summary>
        [NotMapped]
        [Prop(Required: false, OrderIndex: 1000, ShowInTable:false)]
        [VistaUI(ControlUI: ControlUI.HTML_NONE, Accion: Acciones.none)]
        public int IndiceOrdenamientoTabla { get; set; }

        /// <summary>
        /// Determina si el contenido de l apropiedad puede utilizaerse como etiqueta de despliegie
        /// para humanos
        /// </summary>
        [Prop(Required: false, OrderIndex: 1000, ShowInTable: false)]
        [VistaUI(ControlUI: ControlUI.HTML_NONE, Accion: Acciones.none)]

        public bool Etiqueta { get; set; }

        /// <summary>
        /// Especifica si al propiedad debe inferirse del contexto de ejecución
        /// </summary>
        [NotMapped]
        public bool Contextual { get; set; }

        [NotMapped]
        public string IdContextual { get; set; }


        [NotMapped]
        public bool MostrarEnTabla { get; set; }

        [NotMapped]
        public bool AlternarEnTabla { get; set; }

        public virtual TipoDato TipoDato { get; set; }

        public virtual AtributoLista AtributoLista { get; set; }
       
        public virtual AtributoTabla AtributoTabla { get; set; }

        public virtual List<AtributoEvento> AtributosEvento { get; set; }

        public virtual List<AtributoVistaUI> AtributosVistaUI { get; set; }

        public virtual ValidadorTexto ValidadorTexto { get; set; }
        
        public virtual ValidadorNumero ValidadorNumero { get; set; }
        
        [NotMapped]
        public virtual ParametroLinkVista ParametroLinkVista { get; set; }

        // public virtual ICollection<ValorLista> ValoresLista { get; set; }
    }
}
