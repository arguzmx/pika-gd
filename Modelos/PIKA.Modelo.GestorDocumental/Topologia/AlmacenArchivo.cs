using PIKA.Constantes.Aplicaciones.GestorDocumental;
using PIKA.Infraestructura.Comun;
using PIKA.Modelo.GestorDocumental;
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

    [EntidadVinculada(TokenSeguridad: ConstantesAppGestionDocumental.MODULO_ACTIVOS,
    EntidadHijo: "ZonaAlmacen",
    Cardinalidad: TipoCardinalidad.UnoVarios, PropiedadPadre: "Id",
    PropiedadHijo: "AlmacenArchivoId")]

    [EntidadVinculada(TokenSeguridad: ConstantesAppGestionDocumental.MODULO_ACTIVOS,
    EntidadHijo: "ContenedorAlmacen",
    Cardinalidad: TipoCardinalidad.UnoVarios, PropiedadPadre: "Id",
    PropiedadHijo: "AlmacenArchivoId")]

    /// <summary>
    /// Define un espacio físico asociado al archivo donde se almacenan los activo físicos
    /// </summary>

    [Entidad(PaginadoRelacional: false, EliminarLogico: true,
        TokenMod: ConstantesAppGestionDocumental.MODULO_ARCHIVOS,
        TokenApp: ConstantesAppGestionDocumental.APP_ID)]
    public class AlmacenArchivo : Entidad<string>, IEntidadNombrada
    {
        public AlmacenArchivo()
        {
            Zonas = new List<ZonaAlmacen>();
            Posiciones = new List<PosicionAlmacen>();
            Contenedores = new List<ContenedorAlmacen>();
        }

        [Prop(Required: false, isId: true, Visible: false, OrderIndex: 0)]
        [VistaUI(ControlUI: ControlUI.HTML_HIDDEN, Accion: Acciones.update)]
        public override string Id { get => base.Id; set => base.Id = value; }

        /// <summary>
        /// Nombre único del almacén
        /// </summary>
        [Prop(Required: true, OrderIndex: 10)]
        [VistaUI(ControlUI: ControlUI.HTML_TEXT, Accion: Acciones.addupdate)]
        [ValidString(minlen: 2, maxlen: 200)]
        public string Nombre { get; set; }

        /// <summary>
        /// Clave asociada al arhchvi en la organización
        /// </summary>
        [Prop(Required: true, OrderIndex: 20)]
        [VistaUI(ControlUI: ControlUI.HTML_TEXT, Accion: Acciones.addupdate)]
        [ValidString(minlen: 0, maxlen: 200)]
        public string Clave { get; set; }

        /// <summary>
        /// Identificador único del archivo al qu pertenece el almacen
        /// </summary>
        [Prop(Required: false, OrderIndex: 600, Visible: false, Contextual: true, IdContextual: ConstantesModelo.PREFIJO_CONEXTO + "ArchivoId")]
        [VistaUI(ControlUI: ControlUI.HTML_HIDDEN, Accion: Acciones.addupdate)]
        [List(Entidad: "Archivo", DatosRemotos: true, TypeAhead: false)]
        public string ArchivoId { get; set; }

        /// <summary>
        /// Ubicación física del archivo por ejemplo una dirección
        /// </summary>
        [Prop(Required: false, OrderIndex: 30)]
        [VistaUI(ControlUI: ControlUI.HTML_TEXTAREA, Accion: Acciones.addupdate)]
        [ValidString(minlen: 0, maxlen: 200)]
        public string  Ubicacion { get; set; }


        /// <summary>
        /// Folio actual para la creacion de contenedores
        /// </summary>
        [Prop(Required: false, OrderIndex: 40)]
        [VistaUI(ControlUI: ControlUI.HTML_NUMBER, Accion: Acciones.addupdate)]
        [ValidNumeric(min:0, usemin:true)]
        public int FolioActualContenedor { get; set; }

        /// <summary>
        /// Cadena para la generación automática de contenedores
        /// </summary>
        [Prop(Required: false, OrderIndex: 50)]
        [VistaUI(ControlUI: ControlUI.HTML_TEXTAREA, Accion: Acciones.addupdate)]
        [ValidString(minlen: 0, maxlen: 250)]
        public string  MacroFolioContenedor { get; set; }

        /// <summary>
        /// Determina si el foliado esta atcivo al crear contenedores en el almacen
        /// </summary>
        [Prop(Required: false, OrderIndex: 35)]
        [VistaUI(ControlUI: ControlUI.HTML_TOGGLE, Accion: Acciones.addupdate)]
        public bool HabilitarFoliado { get; set; }

        [XmlIgnore]
        [JsonIgnore]
        public Archivo Archivo { get; set; }


        [XmlIgnore]
        [JsonIgnore]
        public List<ZonaAlmacen> Zonas { get; set; }


        [XmlIgnore]
        [JsonIgnore]
        public List<PosicionAlmacen> Posiciones { get; set; }

        [XmlIgnore]
        [JsonIgnore]
        public List<ContenedorAlmacen> Contenedores { get; set; }

    }
}
