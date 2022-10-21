using PIKA.Constantes.Aplicaciones.GestorDocumental;
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
    [Entidad(PaginadoRelacional: false, EliminarLogico: true, HabilitarSeleccion: true,
       TokenMod: ConstantesAppGestionDocumental.MODULO_UNIDADESADMIN,
       TokenApp: ConstantesAppGestionDocumental.APP_ID)]

    [EntidadVinculada(TokenSeguridad: ConstantesAppGestionDocumental.MODULO_ACTIVOS,
        EntidadHijo: "Activo",
        Cardinalidad: TipoCardinalidad.UnoVarios, PropiedadPadre: "Id",
        PropiedadHijo: "UnidadAdministrativaArchivoId")]

    [EntidadVinculada(TokenSeguridad: ConstantesAppGestionDocumental.MODULO_UNIDADESADMIN,
        EntidadHijo: "PermisosUnidadAdministrativaArchivo",
        Cardinalidad: TipoCardinalidad.UnoVarios, PropiedadPadre: "Id",
        PropiedadHijo: "UnidadAdministrativaArchivoId", TipoDespliegueVinculo: TipoDespliegueVinculo.Tabular)]
    public class UnidadAdministrativaArchivo : Entidad<string>, IEntidadRelacionada
    {
        public UnidadAdministrativaArchivo()
        {
            Permisos = new HashSet<PermisosUnidadAdministrativaArchivo>();
        }

        public string TipoOrigenDefault => ConstantesModelo.IDORIGEN_UNIDAD_ORGANIZACIONAL;

        [Prop(Required: false, isId: true, Visible: false, OrderIndex: 0)]
        [VistaUI(ControlUI: ControlUI.HTML_HIDDEN, Accion: Acciones.update)]
        public override string Id { get => base.Id; set => base.Id = value; }

        [Prop(Required: true, OrderIndex: 5)]
        [VistaUI(ControlUI: ControlUI.HTML_TEXT, Accion: Acciones.addupdate)]
        [ValidString(minlen: 2, maxlen: 200)]
        public string UnidadAdministrativa { get; set; }

        [Prop(Required: false, OrderIndex: 10)]
        [VistaUI(ControlUI: ControlUI.HTML_TEXT, Accion: Acciones.addupdate)]
        [ValidString(minlen: 2, maxlen: 200)]
        public string AreaProcedenciaArchivo { get; set; }

        [Prop(Required: false, OrderIndex: 15)]
        [VistaUI(ControlUI: ControlUI.HTML_TEXT, Accion: Acciones.addupdate)]
        [ValidString(minlen: 2, maxlen: 200)]
        public string Responsable { get; set; }

        [Prop(Required: false, OrderIndex: 20)]
        [VistaUI(ControlUI: ControlUI.HTML_TEXT, Accion: Acciones.addupdate)]
        [ValidString(minlen: 2, maxlen: 200)]
        public string Cargo { get; set; }


        [Prop(Required: false, OrderIndex: 30)]
        [VistaUI(ControlUI: ControlUI.HTML_TEXT, Accion: Acciones.addupdate)]
        [ValidString(minlen: 2, maxlen: 200)]
        public string Telefono { get; set; }


        [Prop(Required: false, OrderIndex: 35)]
        [VistaUI(ControlUI: ControlUI.HTML_TEXT, Accion: Acciones.addupdate)]
        [ValidString(minlen: 2, maxlen: 200)]
        public string Email { get; set; }


        [Prop(Required: false, OrderIndex: 40)]
        [VistaUI(ControlUI: ControlUI.HTML_TEXTAREA, Accion: Acciones.addupdate)]
        [ValidString(minlen: 2, maxlen: 200)]
        public string Domicilio { get; set; }

        [Prop(Required: false, OrderIndex: 45)]
        [VistaUI(ControlUI: ControlUI.HTML_TEXTAREA, Accion: Acciones.addupdate)]
        [ValidString(minlen: 2, maxlen: 200)]
        public string UbicacionFisica { get; set; }


        /// <summary>
        /// Identificador único del archivo de trámite donde se crearán los activos del acervo
        /// </summary>
        [Prop(Required: true, OrderIndex: 110)]
        [VistaUI(ControlUI: ControlUI.HTML_SELECT, Accion: Acciones.addupdate)]
        [List(Entidad: "Archivo", DatosRemotos: true, TypeAhead: false)]
        public string ArchivoTramiteId { get; set; }

        /// <summary>
        /// Identificador único del archivo de concentración donde se crearán los activos del acervo
        /// </summary>
        [Prop(Required: false, OrderIndex: 115)]
        [VistaUI(ControlUI: ControlUI.HTML_SELECT, Accion: Acciones.addupdate)]
        [List(Entidad: "Archivo", DatosRemotos: true, TypeAhead: false)]
        public string ArchivoConcentracionId { get; set; }

        /// <summary>
        /// Identificador único del archivo histórico donde se crearán los activos del acervo
        /// </summary>
        [Prop(Required: false, OrderIndex: 1200)]
        [VistaUI(ControlUI: ControlUI.HTML_SELECT, Accion: Acciones.addupdate)]
        [List(Entidad: "Archivo", DatosRemotos: true, TypeAhead: false)]
        public string ArchivoHistoricoId { get; set; }

        /// <summary>
        /// El tipo de orígen en para este modelo es el elemento de la unidad organizacional 
        /// Este elemento puede ser unn departamento u oficina que tiene acervo as su cargo
        /// </summary>
        [Prop(Required: true, OrderIndex: 1000, Contextual: true, ShowInTable: false, Searchable: false, DefaultValue: ConstantesModelo.IDORIGEN_UNIDAD_ORGANIZACIONAL)]
        [VistaUI(ControlUI: ControlUI.HTML_HIDDEN, Accion: Acciones.addupdate)]
        public string TipoOrigenId { get; set; }

        /// <summary>
        /// Identificador de la organización a la que pertenece el cuadro de clasificación
        /// </summary>
        [Prop(Required: true, Visible: false, OrderIndex: 1010, Contextual: true, ShowInTable: false, IdContextual: ConstantesModelo.GLOBAL_UOID)]
        [VistaUI(ControlUI: ControlUI.HTML_HIDDEN, Accion: Acciones.addupdate)]
        public string OrigenId { get; set; }

        [JsonIgnore]
        [XmlIgnore]
        public Archivo ArchivoTramite { get; set; }

        [JsonIgnore]
        [XmlIgnore]
        public Archivo ArchivoConcentracion { get; set; }

        [JsonIgnore]
        [XmlIgnore]
        public Archivo ArchivoHistorico { get; set; }

        [JsonIgnore]
        [XmlIgnore]
        public ICollection<Activo> Activos { get; set; }

        [JsonIgnore]
        [XmlIgnore]
        public ICollection<PermisosUnidadAdministrativaArchivo> Permisos { get; set; }

        [JsonIgnore]
        [XmlIgnore]
        public List<EstadisticaClasificacionAcervo> EstadisticasClasificacionAcervo { get; set; }

    }
}
