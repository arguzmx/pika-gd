﻿using PIKA.Infraestructura.Comun;
using RepositorioEntidades;
using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using System.Xml.Serialization;
using PIKA.Modelo.Metadatos;
using PIKA.Modelo.Metadatos.Atributos;
using System.ComponentModel.DataAnnotations.Schema;
using PIKA.Constantes.Aplicaciones.GestorDocumental;
using PIKA.Modelo.GestorDocumental.Reportes;

namespace PIKA.Modelo.GestorDocumental
{
    [Entidad(PaginadoRelacional: false, EliminarLogico: true,
        TokenMod: ConstantesAppGestionDocumental.MODULO_ACTIVOS,
        TokenApp: ConstantesAppGestionDocumental.APP_ID)]
    [EntidadVinculada(TokenSeguridad: ConstantesAppGestionDocumental.MODULO_ACTIVOS, 
        EntidadHijo: "ampliacion", Cardinalidad: TipoCardinalidad.UnoVarios,
        PropiedadPadre: "Id", PropiedadHijo: "ActivoId")]
    public class Activo: Entidad<string>, IEntidadRelacionada, IEntidadIdElectronico, 
        IEntidadEliminada, IEntidadReportes
    {

        public Activo()
        {
            TipoOrigenId = TipoOrigenDefault;
            HistorialArchivosActivo = new HashSet<HistorialArchivoActivo>();
            Ampliaciones = new HashSet<Ampliacion>();
            PrestamosRelacionados = new HashSet<ActivoPrestamo>();
            TransferenciasRelacionados = new HashSet<ActivoTransferencia>();
            DeclinadosTransferenciaRelacionados = new HashSet<ActivoDeclinado>();

            this.Reportes = new List<IProveedorReporte>();
            this.Reportes.Add(new ReporteCaratulaActivo());
        }

        [Prop(Required: false, isId: true, Visible: false, OrderIndex: 0)]
        [VistaUI(ControlUI: ControlUI.HTML_HIDDEN, Accion: Acciones.update)]
        public override string Id { get => base.Id; set => base.Id = value; }


        /// <summary>
        /// Identificador único del cuadro de clasificación, 
        /// Este se llena del lado del servidor
        /// </summary>
        [Prop(Required: false, OrderIndex: 0, Visible: false)]
        [VistaUI(ControlUI: ControlUI.HTML_NONE, Accion: Acciones.none)]
        public string CuadroClasificacionId { get; set; }


        /// <summary>
        /// Identificador único de la etraada de clasificación
        /// </summary>
        [Prop(Required: true, OrderIndex: 5)]
        [VistaUI(ControlUI: ControlUI.HTML_SELECT, Accion: Acciones.addupdate)]
        [List(Entidad: "EntradaClasificacion", DatosRemotos: true, TypeAhead: true)]
        public string EntradaClasificacionId { get; set; }

        /// <summary>
        /// Nombre de la entrada de inventario por ejemplo el número de expediente
        /// </summary>
        [Prop(Required: true, OrderIndex: 10)]
        [VistaUI(ControlUI: ControlUI.HTML_TEXT, Accion: Acciones.addupdate)]
        [ValidString(minlen: 2, maxlen: 200)]
        public string Nombre { get; set; }
       
        /// <summary>
        /// ID Unico de la entrada de inventario por ejemplo el número de expediente
        /// </summary>
        [Prop(Required: false, OrderIndex: 12)]
        [VistaUI(ControlUI: ControlUI.HTML_TEXT, Accion: Acciones.addupdate)]
        [ValidString(minlen: 2, maxlen: 250)]
        public string IDunico { get; set; }

        /// <summary>
        /// Fecha de apertura UTC del activo
        /// </summary>
        [Prop(Required: true, OrderIndex: 15)]
        [VistaUI(ControlUI: ControlUI.HTML_DATE, Accion: Acciones.addupdate)]
        public DateTime FechaApertura { get; set; }

        /// <summary>
        /// Fecha opcional de ciere del activo
        /// </summary>
        [Prop(Required: false, OrderIndex: 16)]
        [VistaUI(ControlUI: ControlUI.HTML_DATE, Accion: Acciones.addupdate)]
        public DateTime? FechaCierre { get; set; }

        /// <summary>
        /// Asunto de la entrada de inventario
        /// </summary>
        [Prop(Required: false, OrderIndex: 30, Visible: false)]
        [VistaUI(ControlUI: ControlUI.HTML_TEXTAREA, Accion: Acciones.addupdate)]
        [ValidString(minlen: 2, maxlen: 2048)]
        public string Asunto { get; set; }


        /// <summary>
        /// Código de barras o QR de la entrada para ser leído por un scanner 
        /// </summary>
        [Prop(Required: false, OrderIndex: 40, Visible: false)]
        [VistaUI(ControlUI: ControlUI.HTML_TEXT, Accion: Acciones.addupdate)]
        [ValidString(minlen: 1, maxlen: 512)]
        public string CodigoOptico { get; set; }



        /// <summary>
        /// Código electrónico de acceso al elemento por ejemplo RFID
        /// </summary>
        [Prop(Required: false, OrderIndex: 50, Visible: false)]
        [VistaUI(ControlUI: ControlUI.HTML_TEXT, Accion: Acciones.update)]
        [ValidString(minlen: 1, maxlen: 512)]
        public string CodigoElectronico { get; set; }

        /// <summary>
        /// Indica que el elemento se encuentra en formato electrónico desde su creación
        /// </summary>
        [Prop(Required: false, OrderIndex: 60, DefaultValue: "false", Visible: false)]
        [VistaUI(ControlUI: ControlUI.HTML_TOGGLE, Accion: Acciones.addupdate)]
        public bool EsElectronico { get; set; }

        /// <summary>
        /// Especifica si el activo se encuentra marcado como en reserva
        /// </summary>
        [Prop(Required: false, OrderIndex: 60, DefaultValue: "false", Visible: false)]
        [VistaUI(ControlUI: ControlUI.HTML_TOGGLE, Accion: Acciones.addupdate)]
        public bool Reservado { get; set; }

        /// <summary>
        /// Especifica si el activo se encuentra marcado como confidenxial
        /// </summary>
        [Prop(Required: false, OrderIndex: 60, DefaultValue: "false", Visible: false)]
        [VistaUI(ControlUI: ControlUI.HTML_TOGGLE, Accion: Acciones.addupdate)]
        public bool Confidencial { get; set; }



        [Prop(Required: false, OrderIndex: 100, DefaultValue: "false", Visible: false)]
        [VistaUI(ControlUI: ControlUI.HTML_HIDDEN, Accion: Acciones.none)]
        public bool Eliminada { get; set; }


        /// <summary>
        /// Establece el archivo en el que fue originado el activo
        /// </summary>
        [Prop(Required: false, OrderIndex: 6 , Visible:false)]
        [VistaUI(ControlUI: ControlUI.HTML_SELECT, Accion: Acciones.none)]
        public string ArchivoOrigenId { get; set; }


        /// <summary>
        /// Identificador único del archivo actual del activo
        /// ESTOS VALORES SE CALCULAR POR SISTEMA  EN BASE AL LOS PROCESOS DE TRASNFENRENCIA
        /// </summary>
        [Prop(Required: true, OrderIndex: 600, Visible: false, Contextual: true, IdContextual: ConstantesModelo.PREFIJO_CONEXTO + "ArchivoId")]
        [VistaUI(ControlUI: ControlUI.HTML_HIDDEN, Accion: Acciones.addupdate)]
        public string ArchivoId { get; set; }
        //# la relacion del actuivo con archovo es de 1 a 1, un activo puede estar en un sólo archivo al mismo tiempo

        /// <summary>
        /// ESTOS VALORES SE CALCULAR POR SISTEMA  EN BASE AL CONTROL DE PRESTAMO
        /// </summary>
        [Prop(Required: false, OrderIndex: 110, DefaultValue: "false", Visible: false)]
        [VistaUI(ControlUI: ControlUI.HTML_HIDDEN, Accion: Acciones.none)]
        public bool EnPrestamo { get; set; }


        /// <summary>
        /// Especifica si el activo tiene ampliaciones vigentes
        /// ESTOS VALORES SE CALCULAR POR SISTEMA  EN BASE A SI TIENE AMPLIACIONES ACTIVAS
        /// </summary>
        [Prop(Required: false, OrderIndex: 120, DefaultValue: "false", Visible: false)]
        [VistaUI(ControlUI: ControlUI.HTML_HIDDEN, Accion: Acciones.none)]
        public bool Ampliado { get; set; }

        /// <summary>
        /// Fecha límite de retención calculada al cierre para el archivo de trámite
        /// ESTOS VALORES SE CALCULAR POR SISTEMA CUANDO SE ESTABLECE LA FECHA DE CIERRE
        /// </summary>
        [Prop(Required: false, OrderIndex: 130, Visible: false)]
        [VistaUI(ControlUI: ControlUI.HTML_DATE, Accion: Acciones.none)]
        public DateTime? FechaRetencionAT { get; set; }

        /// <summary>
        /// Fecha límite de retención calculada al cierre para el archivo de concentracion
        /// ESTOS VALORES SE CALCULAR POR SISTEMA CUANDO SE ESTABLECE LA FECHA DE CIERRE
        /// </summary>
        [Prop(Required: false, OrderIndex: 140, Visible: false)]
        [VistaUI(ControlUI: ControlUI.HTML_DATE, Accion: Acciones.none)]
        public DateTime? FechaRetencionAC { get; set; }


        /// <summary>
        /// Los activos del inventario son propiedad de las unidades oragnizacionales
        /// y éstas a su vez pertenecen a un dominio lo que garantiza la cobertura de movivimentos
        /// </summary>
        [XmlIgnore]
        [JsonIgnore]
        public string TipoOrigenDefault => ConstantesModelo.IDORIGEN_UNIDAD_ORGANIZACIONAL;

        /// <summary>
        /// En este ID 
        /// </summary>
        [Prop(Required: true, OrderIndex: 1000, Contextual: false, ShowInTable: false, Searchable: false, DefaultValue: ConstantesModelo.IDORIGEN_UNIDAD_ORGANIZACIONAL)]
        [VistaUI(ControlUI: ControlUI.HTML_HIDDEN, Accion: Acciones.addupdate)]
        public string TipoOrigenId { get; set; }

        /// <summary>
        /// Alamcena el ID de la unidad organizaciónal creadora de la entrada
        /// </summary>
        [Prop(Required: true, Visible: false, OrderIndex: 1010, Contextual: true, IdContextual: ConstantesModelo.GLOBAL_UOID)]
        [VistaUI(ControlUI: ControlUI.HTML_HIDDEN, Accion: Acciones.addupdate)]
        public string OrigenId { get; set; }


        /// <summary>
        /// Propiedad para detrminar el tipo de archivo en el que se encuenra el activo
        /// </summary>
        [Prop(Required: true, Visible: false, OrderIndex: 1020, Contextual: false, ShowInTable: false)]
        [VistaUI(ControlUI: ControlUI.HTML_HIDDEN, Accion: Acciones.none)]
        public string TipoArchivoId { get; set; }


        [NotMapped]
        [Prop(Required: false, OrderIndex: 1500, Visible: true)]
        [VistaUI(ControlUI: ControlUI.HTML_NONE, Accion: Acciones.none)]
        [List("", false, false, false, "0", "0,7,15,30,60,90,120,150,180")]
        public int? Vencidos { get; set; }


        [XmlIgnore]
        [JsonIgnore]
        public CuadroClasificacion CuadroClasificacion { get; set; }

        [XmlIgnore]
        [JsonIgnore]
        public EntradaClasificacion EntradaClasificacion { get; set; }

        [XmlIgnore]
        [JsonIgnore]
        public Archivo ArchivoActual { get; set; }

        [XmlIgnore]
        [JsonIgnore]
        public Archivo ArchivoOrigen { get; set; }

        [XmlIgnore]
        [JsonIgnore]
        public Asunto Asuntos { get; set; }

        /// <summary>
        /// Historial de archivos por los que ha pasado el activo
        /// </summary>
        
        [XmlIgnore]
        [JsonIgnore]
        public virtual ICollection<HistorialArchivoActivo> HistorialArchivosActivo { get; set; }

        [XmlIgnore]
        [JsonIgnore]
        public virtual TipoArchivo TipoArchivo { get; set; }


        [XmlIgnore]
        [JsonIgnore]
        public virtual ICollection<Ampliacion> Ampliaciones { get; set; }

        [XmlIgnore]
        [JsonIgnore]
        public virtual ICollection<ActivoPrestamo> PrestamosRelacionados { get; set; }

        [XmlIgnore]
        [JsonIgnore]
        public virtual ICollection<ActivoTransferencia> TransferenciasRelacionados { get; set; }

        [XmlIgnore]
        [JsonIgnore]
        public virtual ICollection<ActivoDeclinado> DeclinadosTransferenciaRelacionados { get; set; }


        [NotMapped]
        [JsonIgnore]
        [XmlIgnore]
        public List<IProveedorReporte> Reportes { get; set; }

    }
}
