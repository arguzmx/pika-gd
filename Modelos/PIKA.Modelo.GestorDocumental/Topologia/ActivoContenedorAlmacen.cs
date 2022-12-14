using Newtonsoft.Json;
using PIKA.Constantes.Aplicaciones.GestorDocumental;
using PIKA.Infraestructura.Comun;
using PIKA.Modelo.Metadatos;
using PIKA.Modelo.Metadatos.Atributos;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace PIKA.Modelo.GestorDocumental
{
    /// <summary>
    /// Vincula a los usuarios con los diferentes roles
    /// </summary>

    [Entidad(EliminarLogico: false,
        TokenApp: ConstantesAppGestionDocumental.APP_ID, TokenMod: ConstantesAppGestionDocumental.MODULO_ALMACENARCHIVO)]
    public class ActivoContenedorAlmacen
    {
        /// <summary>
        /// Identificador único del rol
        /// </summary>
        [Prop(Searchable: false, Required: true, Visible: false, OrderIndex: 10, HieParent: false, ShowInTable: false,
        Contextual: true, IdContextual: ConstantesModelo.PREFIJO_CONEXTO + "Id")]
        [VistaUI(ControlUI: ControlUI.HTML_HIDDEN, Accion: Acciones.addupdate)]
        public string ContenedorAlmacenId { get; set; }

        /// <summary>
        /// Identificador único del usuario
        /// </summary>
        [Prop(Required: true, Visible: true, OrderIndex: 20)]
        [VistaUI(ControlUI: ControlUI.HTML_SELECT_MULTI, Accion: Acciones.addupdate)]
        [List(Entidad: "Activo", DatosRemotos: true, TypeAhead: true, FiltroBusqueda: true)]
        public string ActivoId { get; set; }

        [XmlIgnore]
        [JsonIgnore]
        public ContenedorAlmacen ContenedorAlmacen { get; set; }

    }
}
