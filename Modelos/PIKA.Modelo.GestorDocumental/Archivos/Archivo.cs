﻿using PIKA.Infraestructura.Comun;
using RepositorioEntidades;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;
using System.Xml.Serialization;
using PIKA.Modelo.Metadatos;
using PIKA.Modelo.Metadatos.Atributos;
using PIKA.Constantes.Aplicaciones.GestorDocumental;
using System.ComponentModel.DataAnnotations.Schema;
using PIKA.Modelo.GestorDocumental.Reportes;

namespace PIKA.Modelo.GestorDocumental
{

    [Entidad(PaginadoRelacional: false, EliminarLogico: true , 
        TokenMod: ConstantesAppGestionDocumental.MODULO_ARCHIVOS,
        TokenApp: ConstantesAppGestionDocumental.APP_ID)]
    
    [EntidadVinculada(TokenSeguridad: ConstantesAppGestionDocumental.MODULO_ACTIVOS,
        EntidadHijo: "Activo",
        Cardinalidad: TipoCardinalidad.UnoVarios, PropiedadPadre: "Id",
        PropiedadHijo: "ArchivoId")]


    [EntidadVinculada(TokenSeguridad: ConstantesAppGestionDocumental.MODULO_ACTIVOS,
        EntidadHijo: "Prestamo",
        Cardinalidad: TipoCardinalidad.UnoVarios, PropiedadPadre: "Id",
        PropiedadHijo: "ArchivoId")]


    [EntidadVinculada(TokenSeguridad: ConstantesAppGestionDocumental.MODULO_ARCHIVOS,
        EntidadHijo: "AlmacenArchivo",
        Cardinalidad: TipoCardinalidad.UnoVarios, PropiedadPadre: "Id",
        PropiedadHijo: "ArchivoId")]

    [EntidadVinculada(TokenSeguridad: ConstantesAppGestionDocumental.MODULO_TRANSFERENCIA,
        EntidadHijo: "Transferencia",
        Cardinalidad: TipoCardinalidad.UnoVarios, PropiedadPadre: "Id",
        PropiedadHijo: "ArchivoOrigenId")]

    [EntidadVinculada(TokenSeguridad: ConstantesAppGestionDocumental.MODULO_TRANSFERENCIA,
        EntidadHijo: "PermisosArchivo",
        Cardinalidad: TipoCardinalidad.UnoVarios, PropiedadPadre: "Id",
        PropiedadHijo: "ArchivoId", TipoDespliegueVinculo: TipoDespliegueVinculo.Tabular)]

    public class Archivo : Entidad<string>, IEntidadNombrada, IEntidadEliminada, 
        IEntidadRelacionada, IEntidadReportes
    {

        [XmlIgnore]
        [JsonIgnore]
        public string TipoOrigenDefault => ConstantesModelo.IDORIGEN_UNIDAD_ORGANIZACIONAL;

        /// <summary>
        /// Espacio físico en el que se deposita la 
        /// documentación que lo conforma, es decir, el archivo como
        /// depósito.
        /// </summary>
        public Archivo()
        {
            this.Almacenes = new HashSet<AlmacenArchivo>();
            this.Activos = new HashSet<Activo>();
            HistorialArchivosActivo = new HashSet<HistorialArchivoActivo>();
            Prestamos = new HashSet<Prestamo>();
            TransferenciasOrigen = new HashSet<Transferencia>();
            TransferenciasDestino = new HashSet<Transferencia>();
            ZonasAlmacen = new List<ZonaAlmacen>();
            PosicionesAlmacen = new List<PosicionAlmacen>();
            Contenedores = new List<ContenedorAlmacen>();

            this.Reportes = new List<IProveedorReporte>();
            this.Reportes.Add(new ReporteGuiaSimpleArchivo());
            this.Reportes.Add(new ReporteInventario());
        }


        [Prop(Required: false, isId: true, Visible: false, OrderIndex: 0)]
        [VistaUI(ControlUI: ControlUI.HTML_HIDDEN, Accion: Acciones.update)]
        public override string Id { get => base.Id; set => base.Id = value; }

        /// <summary>
        /// Nombre único del cuadro de clasificación
        /// </summary>
        [Prop(Required: true, OrderIndex: 5)]
        [VistaUI(ControlUI: ControlUI.HTML_TEXT, Accion: Acciones.addupdate)]
        [ValidString(minlen: 2, maxlen: 200)]
        public string Nombre { get; set; }

        /// <summary>
        /// Especifica si el elemento ha sido marcado como eliminado
        /// </summary>
        [Prop(Required: false, OrderIndex: 100, DefaultValue: "false")]
        [VistaUI(ControlUI: ControlUI.HTML_HIDDEN, Accion: Acciones.none)]
        public bool Eliminada { get; set; }

        /// <summary>
        /// El tipo de orígen en para este modelo es el elemento de la unidad organizacional 
        /// Este elemento puede ser unn departamento u oficina que tiene acervo as su cargo
        /// </summary>
        [Prop(Required: true, OrderIndex: 1000, Contextual: true, ShowInTable: false, Searchable: false, DefaultValue: ConstantesModelo.IDORIGEN_UNIDAD_ORGANIZACIONAL)]
        [VistaUI(ControlUI: ControlUI.HTML_HIDDEN, Accion: Acciones.addupdate)]
        public string TipoOrigenId { get; set; }

        /// <summary>
        /// Identificador de la organización a la que pertenece el archivo
        /// </summary>
        [Prop(Required: true, Visible: false, OrderIndex: 1010, Contextual: true, IdContextual: ConstantesModelo.GLOBAL_UOID)]
        [VistaUI(ControlUI: ControlUI.HTML_HIDDEN, Accion: Acciones.addupdate)]
        public string OrigenId { get; set; }


        /// <summary>
        /// IDentificador del tipo de archivo
        /// </summary>
        [Prop(Required: false, OrderIndex: 70)]
        [VistaUI(ControlUI: ControlUI.HTML_SELECT, Accion: Acciones.addupdate)]
        [List(Entidad: "TipoArchivo", DatosRemotos: true, TypeAhead: false)]
        public string TipoArchivoId { get; set; }

        [XmlIgnore]
        [JsonIgnore]
        /// <summary>
        /// Tipo de archivo 
        /// </summary>
        public virtual TipoArchivo Tipo { get; set; }



        [Prop(Required: false, OrderIndex: 80)]
        [VistaUI(ControlUI: ControlUI.HTML_SELECT, Accion: Acciones.addupdate)]
        [List(Entidad: "Volumen", DatosRemotos: true, TypeAhead: false)]
        public string VolumenDefaultId { get; set; }


        [Prop(Required: false, OrderIndex: 81)]
        [VistaUI(ControlUI: ControlUI.HTML_SELECT, Accion: Acciones.addupdate)]
        [List(Entidad: "PuntoMontaje", DatosRemotos: true, TypeAhead: false)]
        public string PuntoMontajeId { get; set; }

        /// <summary>
        /// Alamcenes físicos que tiene al archivo bajo su control
        /// </summary>
        //public virtual ICollection<AlmacenArchivo> Almacenes { get; set; }


        /// <summary>
        /// Historial movimientos de activos en el archivo
        /// </summary>
        [XmlIgnore]
        [JsonIgnore]
        public virtual ICollection<HistorialArchivoActivo> HistorialArchivosActivo { get; set; }


        /// <summary>
        /// Todos los activos que se encuentran en un archivo
        /// </summary>
        [XmlIgnore]
        [JsonIgnore]
        public virtual ICollection<Activo> Activos { get; set; }

        [XmlIgnore]
        [JsonIgnore]
        public virtual ICollection<Activo> ActivosOrigen { get; set; }

        /// <summary>
        /// Prestamos realizados en el archivo
        /// </summary>
        [XmlIgnore]
        [JsonIgnore]
        public virtual ICollection<Prestamo> Prestamos { get; set; }

        /// <summary>
        /// Almacenes donde se encuentra en el archivo
        /// </summary>
        [XmlIgnore]
        [JsonIgnore]
        public virtual ICollection<AlmacenArchivo> Almacenes { get; set; }

        /// <summary>
        /// Transferencias realizadoas en el archivo
        /// </summary>
        [XmlIgnore]
        [JsonIgnore]
        public virtual ICollection<Transferencia> TransferenciasOrigen { get; set; }

        [XmlIgnore]
        [JsonIgnore]
        public virtual ICollection<Transferencia> TransferenciasDestino { get; set; }

        [NotMapped]
        [JsonIgnore]
        [XmlIgnore]
        public List<IProveedorReporte> Reportes { get; set; }


        [JsonIgnore]
        [XmlIgnore]
        public List<UnidadAdministrativaArchivo> UnidadesTramite { get; set; }

        [JsonIgnore]
        [XmlIgnore]
        public List<UnidadAdministrativaArchivo> UnidadesConcentracion { get; set; }


        [JsonIgnore]
        [XmlIgnore]
        public List<UnidadAdministrativaArchivo> UnidadesHistorico { get; set; }

        [JsonIgnore]
        [XmlIgnore]
        public List<EstadisticaClasificacionAcervo> EstadisticasClasificacionAcervo { get; set; }


        [JsonIgnore]
        [XmlIgnore]
        public List<ZonaAlmacen> ZonasAlmacen { get; set; }

        [JsonIgnore]
        [XmlIgnore]
        public List<PosicionAlmacen> PosicionesAlmacen { get; set; }

        [XmlIgnore]
        [JsonIgnore]
        public List<ContenedorAlmacen> Contenedores { get; set; }


        [XmlIgnore]
        [JsonIgnore]
        public List<PermisosArchivo> PermisosArchivo { get; set; }
    }
}
