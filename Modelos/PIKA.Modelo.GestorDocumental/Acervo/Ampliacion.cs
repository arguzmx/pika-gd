﻿using RepositorioEntidades;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;
using System.Xml.Serialization;
using PIKA.Modelo.Metadatos;
using PIKA.Modelo.Metadatos.Atributos;
using PIKA.Constantes.Aplicaciones.GestorDocumental;

namespace PIKA.Modelo.GestorDocumental
{
    [Entidad(PaginadoRelacional: false, EliminarLogico: false,
        TokenMod: ConstantesAppGestionDocumental.MODULO_ACTIVOS,
        TokenApp: ConstantesAppGestionDocumental.APP_ID)]
    public class Ampliacion: Entidad<string>
    {
        public Ampliacion()
        {
            this.Anos = 0;
            this.Meses = 0;
            this.Dias = 0;
        }

        [Prop(Required: false, isId: true, Visible: false, OrderIndex: 0)]
        [VistaUI(ControlUI: ControlUI.HTML_HIDDEN, Accion: Acciones.update)]
        public override string Id { get => base.Id; set => base.Id = value; }

        /// <summary>
        /// Identificadorúnico del tipo de ampliacion
        /// </summary>
        [Prop(Required: true, OrderIndex: 50)]
        [VistaUI(ControlUI: ControlUI.HTML_SELECT, Accion: Acciones.addupdate)]
        [List(Entidad: "TipoAmpliacion", DatosRemotos: true, TypeAhead: false)]
        public string TipoAmpliacionId { get; set; }

        /// <summary>
        /// Especifica que la reserva debe tener una fecha de fin de reserva si este campo se encuentra activo
        /// en caso contrario deben indicarse un valor en anos, meses o días
        /// </summary>
        [Prop(Required: true, OrderIndex: 100, DefaultValue: "false", Visible: false)]
        [VistaUI(ControlUI: ControlUI.HTML_TOGGLE, Accion: Acciones.addupdate)]
        public bool FechaFija { get; set; }



        /// <summary>
        /// Identifica si la amplaición se encuentra vigente, 
        /// Este valor se calcula a partir de las fechas de inicio y la fecha final o el periodo
        /// </summary>
        [Prop(Required: true, OrderIndex: 150, DefaultValue: "false", Visible: false)]
        [VistaUI(ControlUI: ControlUI.HTML_NONE, Accion: Acciones.none)]
        public bool Vigente { get; set; }


        /// <summary>
        /// Fecha de inicio de la reserva, corresponde a la fecha de cierre si no se especifica
        /// </summary>
        [Prop(Required: false, OrderIndex: 200)]
        [VistaUI(ControlUI: ControlUI.HTML_DATE, Accion: Acciones.addupdate)]
        public DateTime? Inicio { get; set; }




        /// <summary>
        /// Fecha de finalización de la reserva, si este campo es especificado no son tomados en cuenta los campos, anos, meses, dias
        /// </summary>
        [Prop(Required: false, OrderIndex: 250)]
        [VistaUI(ControlUI: ControlUI.HTML_DATE, Accion: Acciones.addupdate)]
        [Event(Entidad: "FechaFija", Evento: Eventos.AlCambiar, Operacion: Operaciones.Mostrar, "{0}===true")]
        public DateTime? Fin { get; set; }



        /// <summary>
        /// Funcamento para la reserva
        /// </summary>
        [Prop(Required: true, OrderIndex: 300, Visible: false)]
        [VistaUI(ControlUI: ControlUI.HTML_TEXTAREA, Accion: Acciones.addupdate)]
        [ValidString(minlen: 2, maxlen: 2000)]
        public string FundamentoLegal { get; set; }


        /// <summary>
        /// Años para la amepliación
        /// </summary>
        [Prop(Required: false, OrderIndex: 2000, Visible: false)]
        [VistaUI(ControlUI: ControlUI.HTML_NUMBER, Accion: Acciones.addupdate)]
        [ValidNumeric(min: 0, max: 0, usemin: true, usemax: false, defaulvalue:0)]
        [Event(Entidad: "FechaFija", Evento: Eventos.AlCambiar, Operacion: Operaciones.Mostrar, "{0}===false")]
        public int? Anos { get; set; }

        /// <summary>
        /// Meses para la ampliacion
        /// </summary>
        [Prop(Required: false, OrderIndex: 2000, Visible: false)]
        [Event(Entidad: "FechaFija", Evento: Eventos.AlCambiar, Operacion: Operaciones.Mostrar, "{0}===false")]
        [VistaUI(ControlUI: ControlUI.HTML_NUMBER, Accion: Acciones.addupdate)]
        [ValidNumeric(min: 0, max: 0, usemin: true, usemax: false, defaulvalue: 0)]
        public int? Meses { get; set; }

        /// <summary>
        /// Dias para la ampliación
        /// </summary>
        [Prop(Required: false, OrderIndex: 2000, Visible: false)]
        [Event(Entidad: "FechaFija", Evento: Eventos.AlCambiar, Operacion: Operaciones.Mostrar, "{0}===false")]
        [VistaUI(ControlUI: ControlUI.HTML_NUMBER, Accion: Acciones.addupdate)]
        [ValidNumeric(min: 0, max: 0, usemin: true, usemax: false, defaulvalue: 0)]
        public int? Dias { get; set; }


        /// <summary>
        /// Identificador único del activo al que se asocia la reserva
        /// </summary>
        [Prop(Required: true, OrderIndex: 1000, Contextual: true, ShowInTable: false, Searchable: false)]
        [VistaUI(ControlUI: ControlUI.HTML_HIDDEN, Accion: Acciones.addupdate)]
        public string ActivoId { get; set; }


        [XmlIgnore]
        [JsonIgnore]
        public TipoAmpliacion TipoAmpliacion { get; set; }


        [XmlIgnore]
        [JsonIgnore]
        public Activo activo { get; set; }

    }
}
