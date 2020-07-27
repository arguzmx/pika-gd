using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;
using System.Xml.Serialization;
using PIKA.Modelo.Metadatos;
using PIKA.Modelo.Metadatos.Atributos;

namespace PIKA.Modelo.Contenido
{

    /// <summary>
    /// Destinatario del permiso, puede ser por ejemplo un grupo o una cuenta de usurios
    /// </summary>
    [Entidad(PaginadoRelacional: false, EliminarLogico: false)]
    public class DestinatarioPermiso
    {
        /// <summary>
        /// Identiicador único del permiso de contenido
        /// </summary>
        [Prop(Required: false, isId: true, Visible: false, OrderIndex: 0)]
        [VistaUI(ControlUI: ControlUI.HTML_HIDDEN, Accion: Acciones.update)]
        public string PermisoId { get; set; }

        /// <summary>
        /// Identificador único del destinatario del permiso
        /// </summary>
        [Prop(Required: true, OrderIndex: 5)]
        [VistaUI(ControlUI: ControlUI.HTML_SELECT, Accion: Acciones.addupdate)]
        [List(Entidad: "Usuario", DatosRemotos: true, TypeAhead: true, OrdenarAlfabetico: true)]
        [List(Entidad: "Grupos", DatosRemotos: true, TypeAhead: true, OrdenarAlfabetico: true)]
        public string DestinatarioId { get; set; }

        /// <summary>
        /// Indica si el permiso es aplciable a un grupo de usuarios
        /// </summary>
        [Prop(Required: true,  Visible: false, OrderIndex: 10)]
        [VistaUI(ControlUI: ControlUI.HTML_HIDDEN, Accion: Acciones.addupdate)]
        public bool EsGrupo { get; set; }

        [XmlIgnore]
        [JsonIgnore]
        public Permiso Permiso { get; set; }
    }
}
