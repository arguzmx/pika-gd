using RepositorioEntidades;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;
using System.Xml.Serialization;
using PIKA.Modelo.Metadatos;
using PIKA.Modelo.Metadatos.Atributos;
using PIKA.Infraestructura.Comun;
using System.ComponentModel.DataAnnotations.Schema;
using PIKA.Constantes.Aplicaciones.GestorDocumental;

namespace PIKA.Modelo.GestorDocumental
{

    /// <summary>
    /// Corresponde a una instacia que permite la clasificación documental
    /// </summary>
    [Entidad(EliminarLogico: true,
        TokenMod: ConstantesAppGestionDocumental.MODULO_CUADROCLASIF,
        TokenApp: ConstantesAppGestionDocumental.APP_ID)]
    [LinkCatalogo(EntidadCatalogo: "TipoValoracionDocumental",
        IdCatalogo: "Id", IdEntidad: "Id",
        IdCatalogoMap: "TipoValoracionDocumentalId", IdEntidadMap: "EntradaClasificacionId",
        EntidadVinculo: "ValoracionEntradaClasificacion",
        PropiedadReceptora: "TipoValoracionDocumentalId",
        Despliegue: TipoDespliegueVinculo.GrupoCheckbox)]

    public class EntradaClasificacion : Entidad<string>, IEntidadNombrada,
        IEntidadEliminada
    {

        public EntradaClasificacion()
        {
            ValoracionesEntrada = new HashSet<ValoracionEntradaClasificacion>();
            Activos = new HashSet<Activo>();
        }

        /// <summary>
        ///  Identificador único del la entrada del cuadro de clasificación
        /// </summary>
        [Prop(Required: false, isId: true, Visible: false, OrderIndex: 10)]
        [VistaUI(ControlUI: ControlUI.HTML_HIDDEN, Accion: Acciones.update)]
        public override string Id { get; set; }



        /// <summary>
        /// Clave para la entrada del cuandro, generalmente comienza con la del elemento de clasificación
        /// </summary>
        [Prop(Required: true, OrderIndex: 20)]
        [VistaUI(ControlUI: ControlUI.HTML_TEXT, Accion: Acciones.addupdate)]
        [ValidString(minlen: 2, maxlen: 200)]
        public string Clave { get; set; }

        /// <summary>
        /// Nombre de la entrada
        /// </summary>
        [Prop(Required: true, OrderIndex: 30)]
        [VistaUI(ControlUI: ControlUI.HTML_TEXT, Accion: Acciones.addupdate)]
        [ValidString(minlen: 2, maxlen: 200)]
        public string Nombre { get; set; }

        /// <summary>
        /// Especifica los meses que debe permanecer el expediente o documento en el archivo de trámite una vez que ha sido cerrado
        /// </summary>
        [Prop(Required: true, OrderIndex: 40)]
        [VistaUI(ControlUI: ControlUI.HTML_NUMBER, Accion: Acciones.addupdate)]
        [ValidNumeric(min: 0, max: 60, defaulvalue: 0)]
        public int VigenciaTramite { get; set; }

        /// <summary>
        /// Especifica los meses que debe permanecer el expediente o documento en el archivo de concentración una vez que ha sido cerrado
        /// </summary>
        [Prop(Required: true, OrderIndex: 50)]
        [VistaUI(ControlUI: ControlUI.HTML_NUMBER, Accion: Acciones.addupdate)]
        [ValidNumeric(min: 0, max: 60, defaulvalue: 0)]
        public int VigenciaConcentracion { get; set; }



        /// <summary>
        /// La Descripcion para la entrada del cuandro, generalmente es el contenido de la entrada
        /// </summary>
        [Prop(Required: false, Visible: true, OrderIndex: 65)]
        [VistaUI(ControlUI: ControlUI.HTML_TEXTAREA, Accion: Acciones.addupdate)]
        [ValidString(minlen: 2, maxlen: 1000)]
        public string Descripcion { get; set; }

        /// <summary>
        /// Identificador único del tipo de disposición documental para la entrada
        /// </summary>
        [Prop(Required: false, OrderIndex: 70)]
        [VistaUI(ControlUI: ControlUI.HTML_SELECT, Accion: Acciones.addupdate)]
        [List(Entidad: "TipoDisposicionDocumental", DatosRemotos: true, TypeAhead: false)]
        public string TipoDisposicionDocumentalId { get; set; }

        /// <summary>
        /// Identificador único del elemento de clasificación al que pertenece la entrada
        /// </summary>
        [Prop(Required: false, Visible: false, OrderIndex: 1000, Contextual: true, IdContextual: ConstantesModelo.PREFIJO_CONEXTO + "PadreId")]
        [VistaUI(ControlUI: ControlUI.HTML_HIDDEN, Accion: Acciones.addupdate)]
        public string ElementoClasificacionId { get; set; }



        /// <summary>
        /// Determina si la entrada del cuadro ha sido eliminada
        /// </summary>
        [Prop(OrderIndex: 90, DefaultValue: "false")]
        [VistaUI(ControlUI: ControlUI.HTML_NONE, Accion: Acciones.none)]
        public bool Eliminada { get; set; }

        /// <summary>
        /// Determina la posición relativa del elemento en relación con los elementos del mismo nivle
        /// </summary>
        public int Posicion { get; set; }

        [Prop(Required: true, Visible: false, HieRoot: true, OrderIndex: 1010, Contextual: true, IdContextual: ConstantesModelo.PREFIJO_CONEXTO + "CuadroClasificacion")]
        [VistaUI(ControlUI: ControlUI.HTML_HIDDEN, Accion: Acciones.addupdate)]
        public string CuadroClasifiacionId { get; set; }

        [NotMapped]
        public string NombreCompleto { get { return $"{Clave} {Nombre}"; } }

        /// <summary>
        /// propiedad receptora para el arreglo de ids de valoración documental
        /// </summary>
        [NotMapped]
        public string[] TipoValoracionDocumentalId { get; set; }


        //[XmlIgnore]
        //[JsonIgnore]
        //public string[] Valoracionids { get; set; }
        public ICollection<ValoracionEntradaClasificacion> ValoracionesEntrada { get; set; }

        /// <summary>
        /// Activos del elemento clasificacion
        /// </summary>
        [XmlIgnore]
        [JsonIgnore]
        public virtual ICollection<Activo> Activos { get; set; }

        public virtual TipoDisposicionDocumental DisposicionEntrada { get; set; }

    }
}
