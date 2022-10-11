﻿using PIKA.Constantes.Aplicaciones.GestorDocumental;
using PIKA.Infraestructura.Comun;
using PIKA.Modelo.Metadatos;
using PIKA.Modelo.Metadatos.Atributos;
using RepositorioEntidades;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using System.Xml.Serialization;

namespace PIKA.Modelo.GestorDocumental
{

    /// <summary>
    /// Una trasferencia es el proceso para enviar actuivos de un archivo a otro
    /// a trvés de una lista de activos seleccionados para ser incluidos en dicha tarsnferencia
    /// </summary>

    [Entidad(PaginadoRelacional: false, EliminarLogico: true,
        TokenMod: ConstantesAppGestionDocumental.MODULO_TRANSFERENCIA,
        TokenApp: ConstantesAppGestionDocumental.APP_ID)]


    [EntidadVinculada(TokenSeguridad: ConstantesAppGestionDocumental.MODULO_TRANSFERENCIA,
        EntidadHijo: "ActivoTransferencia",
        Cardinalidad: TipoCardinalidad.UnoVarios, PropiedadPadre: "Id",
        PropiedadHijo: "TransferenciaId")]


    public class Transferencia : Entidad<string>, IEntidadNombrada, IEntidadUsuario
    {

        public Transferencia()
        {
            Eventos = new HashSet<EventoTransferencia>();
            Comentarios = new HashSet<ComentarioTransferencia>();
            ActivosIncluidos = new HashSet<ActivoTransferencia>();
        }

        /// <summary>
        /// Identificador único interno para la trasnferencias
        /// </summary>
        [Prop(Required: false, isId: true, Visible: false, OrderIndex: 0)]
        [VistaUI(ControlUI: ControlUI.HTML_HIDDEN, Accion: Acciones.update)]
        public override string Id { get; set; }


        /// <summary>
        /// Nombre asociado a la trasnferencia
        /// </summary>
        [Prop(Required: true, OrderIndex: 10, IsLabel: true)]
        [VistaUI(ControlUI: ControlUI.HTML_TEXT, Accion: Acciones.addupdate)]
        [ValidString(minlen: 2, maxlen: 200)]
        public string Nombre { get; set; }

        /// <summary>
        /// Identificador único del árchivo destino
        /// </summary>
        [Prop(Required: true, OrderIndex: 20)]
        [VistaUI(ControlUI: ControlUI.HTML_SELECT, Accion: Acciones.addupdate)]
        [List(Entidad: "Archivo", DatosRemotos: true, TypeAhead: false)]
        public string ArchivoDestinoId { get; set; }

        [Prop(Required: false, OrderIndex: 30)]
        [VistaUI(ControlUI: ControlUI.HTML_SELECT, Accion: Acciones.addupdate)]
        [List(Entidad: "CuadroClasificacion", DatosRemotos: true, TypeAhead: false, Default: "")]
        public string CuadroClasificacionId { get; set; }


        [Prop(Required: false, OrderIndex: 40)]
        [VistaUI(ControlUI: ControlUI.HTML_SELECT, Accion: Acciones.addupdate)]
        [List(Entidad: "EntradaClasificacion", DatosRemotos: true, TypeAhead: false)]
        [Event(Entidad: "CuadroClasificacionId", Evento: Metadatos.Atributos.Eventos.AlCambiar, Operacion: Operaciones.Actualizar, "CuadroClasificacionId")]
        public string EntradaClasificacionId { get; set; }


        
        /// <summary>
        /// Número de fplio de la trasnferenci
        /// </summary>
        [Prop(Required: true, isId: false, Visible: true, OrderIndex: 15)]
        [VistaUI(ControlUI: ControlUI.HTML_TEXT, Accion: Acciones.addupdate)]
        public string Folio { get; set; }

        /// <summary>
        ///  Número de elementos involucrados en la trasnferencia
        /// </summary>
        [Prop(Required: false, isId: false, Visible: true, OrderIndex: 210)]
        [VistaUI(ControlUI: ControlUI.HTML_NUMBER, Accion: Acciones.none)]
        public int CantidadActivos { get; set; }

        /// <summary>
        /// Fechas de creación de la trasnfenrecia
        /// </summary>
        [Prop(Required: false, OrderIndex: 220, IsLabel: true)]
        [VistaUI(ControlUI: ControlUI.HTML_DATE, Accion: Acciones.none)]
        public DateTime FechaCreacion { get; set; }
        // Debe ser .Now en formato UTC

        /// <summary>
        /// Esatdo actual de la trasnferencia
        /// </summary>
        [Prop(Required: false, OrderIndex: 230, IsLabel: true)]
        [VistaUI(ControlUI: ControlUI.HTML_TEXT, Accion: Acciones.none)]
        public string EstadoTransferenciaId { get; set; }
        //Obligatorios

        /// <summary>
        /// Identificador único del árchivo origen
        /// </summary>
        [Prop(Required: true, OrderIndex: 240, Visible: false, Contextual: true, ShowInTable: false,
        ToggleInTable: false, IdContextual: ConstantesModelo.PREFIJO_CONEXTO + "ArchivoId")]
        [VistaUI(ControlUI: ControlUI.HTML_HIDDEN, Accion: Acciones.addupdate)]
        public string ArchivoOrigenId { get; set; }


        /// <summary>
        /// Identificador único del usuario que la creó
        /// </summary>
        [Prop(Required: true, Visible: true, OrderIndex: 250)]
        [VistaUI(ControlUI: ControlUI.HTML_NONE, Accion: Acciones.none)]
        public string UsuarioId { get; set; }

        [NotMapped]
        [Prop(Required: false, Visible: true, OrderIndex: 100)]
        [VistaUI(ControlUI: ControlUI.HTML_SELECT, Accion: Acciones.add)]
        [List(Entidad: "Activo", DatosRemotos: true, TypeAhead: false, EsListaTemas: true)]
        public string TemaId { get; set; }


        [NotMapped]
        [Prop(Required: false, Visible: true, OrderIndex: 110)]
        [VistaUI(ControlUI: ControlUI.HTML_TOGGLE, Accion: Acciones.add)]
        public bool EliminarTema { get; set; }



        [XmlIgnore]
        [JsonIgnore]
        public virtual ICollection<EventoTransferencia> Eventos { get; set; }

        [XmlIgnore]
        [JsonIgnore]
        public virtual ICollection<ActivoTransferencia> ActivosIncluidos { get; set; }

        [XmlIgnore]
        [JsonIgnore]
        public virtual  ICollection<ComentarioTransferencia> Comentarios { get; set; }


        [XmlIgnore]
        [JsonIgnore]
        public virtual EntradaClasificacion EntradaClasificacion { get; set; }

        [XmlIgnore]
        [JsonIgnore]
        public virtual CuadroClasificacion CuadroClasificacion { get; set; }

        [XmlIgnore]
        [JsonIgnore]
        public virtual EstadoTransferencia Estado { get; set; }
        [XmlIgnore]
        [JsonIgnore]
        public virtual Archivo ArchivoOrigen { get; set; }
        [XmlIgnore]
        [JsonIgnore]
        public virtual Archivo ArchivoDestino { get; set; }
        
    }
}
