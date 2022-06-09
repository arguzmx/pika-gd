using PIKA.Constantes.Aplicaciones.GestorDocumental;
using PIKA.Infraestructura.Comun;
using PIKA.Modelo.GestorDocumental.Reportes;
using PIKA.Modelo.Metadatos;
using PIKA.Modelo.Metadatos.Atributos;
using RepositorioEntidades;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using System.Text.Json.Serialization;
using System.Xml.Serialization;

namespace PIKA.Modelo.GestorDocumental
{
    [EntidadVinculada(TokenSeguridad: ConstantesAppGestionDocumental.MODULO_ALMACENARCHIVO, 
        EntidadHijo: "activocontenedoralmacen", 
        Cardinalidad: TipoCardinalidad.UnoVarios,
        PropiedadPadre: "Id", 
        PropiedadHijo: "ContenedorAlmacenId", 
        PropiedadIdMiembro: "ActivoId",
        TipoDespliegueVinculo: TipoDespliegueVinculo.Membresia)]

    [Entidad(PaginadoRelacional: false, EliminarLogico: true,
     TokenMod: ConstantesAppGestionDocumental.MODULO_ALMACENARCHIVO,
     TokenApp: ConstantesAppGestionDocumental.APP_ID)]

    /// <summary>
    /// Define un contenedor para la guarda de los activos del acervo por ejemplo una caja de archivo
    /// </summary>
    public class ContenedorAlmacen : Entidad<string>, IEntidadNombrada, IEntidadReportes
    {
        [NotMapped]
        [JsonIgnore]
        [XmlIgnore]
        public List<IProveedorReporte> Reportes { get; set; }
        public ContenedorAlmacen()
        {
            EventosContenedor = new List<EventoContenedorAlmacen>();
            this.Reportes = new List<IProveedorReporte>();
            this.Reportes.Add(new ReporteCaratulaContenedorAlmacen());
        }
        
        [Prop(Required: false, isId: true, Visible: false, OrderIndex: 0)]
        [VistaUI(ControlUI: ControlUI.HTML_HIDDEN, Accion: Acciones.update)]
        public override string Id { get => base.Id; set => base.Id = value; }

        /// <summary>
        /// Nomnbre del contenedor para refeencia humana puede ser el mismo que la clave
        /// </summary>
        [Prop(Required: true, OrderIndex: 10)]
        [VistaUI(ControlUI: ControlUI.HTML_TEXT, Accion: Acciones.addupdate)]
        [ValidString(minlen: 2, maxlen: 200)]
        public string Nombre { get; set; }

        /// <summary>
        /// Codigo de barras asociado
        /// </summary>
        [Prop(Required: false, OrderIndex: 40)]
        [VistaUI(ControlUI: ControlUI.HTML_TEXT, Accion: Acciones.addupdate)]
        [ValidString(minlen: 0, maxlen: 512)]
        public string CodigoBarras { get; set; }

        /// <summary>
        /// Codigo electrónico asociado
        /// </summary>
        [Prop(Required: false, OrderIndex: 50)]
        [VistaUI(ControlUI: ControlUI.HTML_TEXT, Accion: Acciones.addupdate)]
        [ValidString(minlen: 0, maxlen: 512)]
        public string CodigoElectronico { get; set; }

        /// <summary>
        /// Porcentaje de ocupación del contenedor
        /// </summary>
        [Prop(Required: true, OrderIndex: 35)]
        [VistaUI(ControlUI: ControlUI.HTML_NUMBER, Accion: Acciones.addupdate)]
        [ValidNumeric(min: 0, usemin: true, defaulvalue: 0, max: 100, usemax: true)]
        public decimal Ocupacion { get; set; }

        /// <summary>
        /// Identificador unico de la zona en el que se ubica el contenedor
        /// La zona del contenedor es opcional
        /// </summary>
        [Prop(Required: true, OrderIndex: 100)]
        [VistaUI(ControlUI: ControlUI.HTML_SELECT, Accion: Acciones.addupdate)]
        [List(Entidad: "ZonaAlmacen", DatosRemotos: true, TypeAhead: false, FiltroBusqueda: true)]
        public string ZonaAlmacenId { get; set; }

        /// <summary>
        /// Identificador unico de la posición en la que se ubica el contenedor
        /// EL contenedor puede no estar asociado a una posición puede esta solamente en una zona por ejemplo FUMIGACION
        /// </summary>
        [Prop(Required: false, OrderIndex: 110)]
        [VistaUI(ControlUI: ControlUI.HTML_SELECT, Accion: Acciones.addupdate)]
        [List(Entidad: "PosicionAlmacen", DatosRemotos: true, TypeAhead: false, FiltroBusqueda: false)]
        [Event(Entidad: "ZonaAlmacenId", Evento: Eventos.AlCambiar, Operacion: Operaciones.Actualizar, "ZonaAlmacenId")]
        public string PosicionAlmacenId { get; set; }

        /// <summary>
        /// Identificaodr único del archivo en el que se ubica el contenedor
        /// </summary>
        [Prop(Required: false, OrderIndex: 600, Visible: false, Contextual: true, IdContextual: ConstantesModelo.PREFIJO_CONEXTO + "ArchivoId")]
        [VistaUI(ControlUI: ControlUI.HTML_HIDDEN, Accion: Acciones.addupdate)]
        [List(Entidad: "Archivo", DatosRemotos: true, TypeAhead: false)]
        public string ArchivoId { get; set; }

        /// <summary>
        /// Identificador unico del almacen en el que se ubica el contenedor
        /// </summary>
        [Prop(Required: false, OrderIndex: 600, Visible: false, Contextual: true, IdContextual: ConstantesModelo.PREFIJO_CONEXTO + "AlmacenArchivoId")]
        [VistaUI(ControlUI: ControlUI.HTML_HIDDEN, Accion: Acciones.addupdate)]
        [List(Entidad: "AlmacenArchivo", DatosRemotos: true, TypeAhead: false)]
        public string AlmacenArchivoId { get; set; }


        [XmlIgnore]
        [JsonIgnore]
        public PosicionAlmacen Posicion { get; set; }


        [XmlIgnore]
        [JsonIgnore]
        public ZonaAlmacen Zona { get; set; }

        [XmlIgnore]
        [JsonIgnore]
        public AlmacenArchivo Almacen { get; set; }


        [XmlIgnore]
        [JsonIgnore]
        public Archivo Archivo { get; set; }

        [XmlIgnore]
        [JsonIgnore]
        public List<EventoContenedorAlmacen> EventosContenedor { get; set; }

        [XmlIgnore]
        [JsonIgnore]
        public List<ActivoContenedorAlmacen> Activos { get; set; }
    }
}
