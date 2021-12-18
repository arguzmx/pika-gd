using PIKA.Constantes.Aplicaciones.GestorDocumental;
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
    [Entidad(PaginadoRelacional: false, EliminarLogico: false,
       TokenApp: ConstantesAppGestionDocumental.APP_ID, TokenMod: ConstantesAppGestionDocumental.MODULO_UNIDADESADMIN)]
    public class PermisosUnidadAdministrativaArchivo : Entidad<string>
    {
        /// <summary>
        /// Indetificador único del permiso
        /// </summary>
        [Prop(Required: false, isId: true, Visible: false, OrderIndex: 0)]
        [VistaUI(ControlUI: ControlUI.HTML_HIDDEN, Accion: Acciones.update)]
        [LinkViewParameter(Vista: "activos-unidad-admin")]
        public override string Id { get; set; }

        [Prop(Required: false, isId: false, Visible: false, OrderIndex: 0)]
        [VistaUI(ControlUI: ControlUI.HTML_HIDDEN, Accion: Acciones.addupdate)]
        public string UnidadAdministrativaArchivoId { get; set; }

        /// <summary>
        /// Identificador único del ROL destinatario del permiso
        /// </summary>
        [Prop(Required: true, OrderIndex: 10)]
        [VistaUI(ControlUI: ControlUI.HTML_SELECT, Accion: Acciones.add)]
        [List(Entidad: "Rol", DatosRemotos: true, TypeAhead: false)]
        public string DestinatarioId { get; set; }


        /// <summary>
        /// Determina el permiso para leer datos del acervo de la unidad
        /// </summary>
        [Prop(OrderIndex: 15, DefaultValue: "false")]
        [VistaUI(ControlUI: ControlUI.HTML_TOGGLE, Accion: Acciones.addupdate)]
        public bool LeerAcervo { get; set; }

        /// <summary>
        /// Determina el permiso para crear acervo en la unidad
        /// </summary>
        [Prop(OrderIndex: 20, DefaultValue: "false")]
        [VistaUI(ControlUI: ControlUI.HTML_TOGGLE, Accion: Acciones.addupdate)]
        public bool CrearAcervo { get; set; }

        /// <summary>
        /// Determina el permiso para actualizar acervo en la unidad
        /// </summary>
        [Prop(OrderIndex: 30, DefaultValue: "false")]
        [VistaUI(ControlUI: ControlUI.HTML_TOGGLE, Accion: Acciones.addupdate)]
        public bool ActualizarAcervo { get; set; }
        
        /// <summary>
        /// Determina el permiso para elminar acervo de la unidad
        /// </summary>
        [Prop(OrderIndex: 40, DefaultValue: "false")]
        [VistaUI(ControlUI: ControlUI.HTML_TOGGLE, Accion: Acciones.addupdate)]
        public bool ElminarAcervo { get; set; }


        [JsonIgnore]
        [XmlIgnore]
        public UnidadAdministrativaArchivo UnidadAdministrativaArchivo { get; set; }

    }
}
