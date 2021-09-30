using PIKA.Constantes.Aplicaciones.GestorDocumental;
using PIKA.Infraestructura.Comun;
using PIKA.Modelo.Metadatos;
using PIKA.Modelo.Metadatos.Atributos;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using System.Text.Json.Serialization;
using System.Xml.Serialization;

namespace PIKA.Modelo.GestorDocumental
{
    [Entidad(EliminarLogico: false, PaginadoRelacional: false, PermiteCambios: false,
        TokenApp: ConstantesAppGestionDocumental.MODULO_PRESTAMO, TokenMod: ConstantesAppGestionDocumental.APP_ID)]

    public class ActivoPrestamo
    {

        /// <summary>
        /// Identtificaor unico del reistros
        /// </summary>
        [Prop(Required: false, isId: true, Visible: false, OrderIndex: 0)]
        [VistaUI(ControlUI: ControlUI.HTML_HIDDEN, Accion: Acciones.update)]
        public string Id { get; set; }


        /// <summary>
        /// Clave principal
        /// </summary>
        [Prop(Searchable: false, Required: true, Visible: false, OrderIndex: 10, HieParent: false, ShowInTable: false,
        Contextual: true, IdContextual: ConstantesModelo.PREFIJO_CONEXTO + "Id")]
        [VistaUI(ControlUI: ControlUI.HTML_HIDDEN, Accion: Acciones.addupdate)]
        public string PrestamoId { get; set; }

        /// <summary>
        /// Clave principal
        /// </summary>
        [Prop(Required: true, Visible: true, OrderIndex: 20)]
        [VistaUI(ControlUI: ControlUI.HTML_SELECT_MULTI, Accion: Acciones.addupdate)]
        [List(Entidad: "Activo", DatosRemotos: true, TypeAhead: true, FiltroBusqueda: true)]
        public string ActivoId { get; set; }

        /// <summary>
        /// Indica si el elemento de prestamo ha sido devuelto
        /// ESTOS VALORES SE CALCULAR POR SISTEMA  EN BASE AL CONTROL DE PRESTAMO
        /// </summary>
        [Prop(Required: false, OrderIndex: 30, DefaultValue: "false", Visible: true)]
        [VistaUI(ControlUI: ControlUI.HTML_HIDDEN, Accion: Acciones.none)]
        public bool Devuelto { get; set; }

        /// <summary>
        /// Fecha en la que el elemento fue devuelto en formato UTC
        /// </summary>
        [Prop(Required: false, OrderIndex: 40, DefaultValue: "false", Visible: true)]
        [VistaUI(ControlUI: ControlUI.HTML_DATETIME, Accion: Acciones.none)]
        public DateTime? FechaDevolucion { get; set; }

        [XmlIgnore]
        [JsonIgnore]
        public Activo Activo { get; set; }
        [XmlIgnore]
        [JsonIgnore]
        public Prestamo Prestamo { get; set; }

    }
}
