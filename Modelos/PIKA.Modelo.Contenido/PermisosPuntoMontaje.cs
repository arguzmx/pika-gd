using PIKA.Constantes.Aplicaciones.Contenido;
using PIKA.Modelo.Metadatos;
using PIKA.Modelo.Metadatos.Atributos;
using RepositorioEntidades;
using System;
using System.Collections.Generic;
using System.Text;

namespace PIKA.Modelo.Contenido
{
    [Entidad(PaginadoRelacional: false, EliminarLogico: true,
       TokenApp: ConstantesAppContenido.APP_ID, TokenMod: ConstantesAppContenido.MODULO_PERMISOS_CONTENIDO)]
    public class PermisosPuntoMontaje: Entidad<string>
    {
        /// <summary>
        /// Indetificador único del permiso
        /// </summary>
        [Prop(Required: false, isId: true, Visible: false, OrderIndex: 0)]
        [VistaUI(ControlUI: ControlUI.HTML_HIDDEN, Accion: Acciones.update)]
        [LinkViewParameter(Vista: "visorcontenido")]
        public override string Id { get; set; }

        [Prop(Required: false, isId: false, Visible: false, OrderIndex: 0)]
        [VistaUI(ControlUI: ControlUI.HTML_HIDDEN, Accion: Acciones.addupdate)]
        public string PuntoMontajeId { get; set; }

        /// <summary>
        /// Identificador único del destinatario del permiso
        /// </summary>
        [Prop(Required: true, OrderIndex: 10)]
        [VistaUI(ControlUI: ControlUI.HTML_SELECT, Accion: Acciones.add)]
        [List(Entidad: "Rol", DatosRemotos: true, TypeAhead: false)]
        public string DestinatarioId { get; set; }


        /// <summary>
        /// Determina el permiso para crear carpetas
        /// </summary>
        [Prop(OrderIndex: 15, DefaultValue: "false")]
        [VistaUI(ControlUI: ControlUI.HTML_TOGGLE, Accion: Acciones.addupdate)]
        public bool Leer { get; set; }

        /// <summary>
        /// Determina el permiso para crear carpetas
        /// </summary>
        [Prop(OrderIndex: 20, DefaultValue: "false")]
        [VistaUI(ControlUI: ControlUI.HTML_TOGGLE, Accion: Acciones.addupdate)]
        public bool Crear { get; set; }

        /// <summary>
        /// Determina el permiso para actualizar carpetas
        /// </summary>
        [Prop(OrderIndex: 30, DefaultValue: "false")]
        [VistaUI(ControlUI: ControlUI.HTML_TOGGLE, Accion: Acciones.addupdate)]
        public bool Actualizar { get; set; }
        
        /// <summary>
        /// Determina el permiso para elminar carpetas
        /// </summary>
        [Prop(OrderIndex: 40, DefaultValue: "false")]
        [VistaUI(ControlUI: ControlUI.HTML_TOGGLE, Accion: Acciones.addupdate)]
        public bool Elminar { get; set; }

           /// <summary>
        /// Determina el permiso para realizar la gestión de contenido de los documentos
        /// </summary>
        [Prop(OrderIndex: 90, DefaultValue: "false")]
        [VistaUI(ControlUI: ControlUI.HTML_TOGGLE, Accion: Acciones.addupdate)]
        public bool GestionContenido { get; set; }


        /// <summary>
        /// Determina el permiso para realizar la gestión de metadatos de los documentos
        /// </summary>
        [Prop(OrderIndex: 100, DefaultValue: "false")]
        [VistaUI(ControlUI: ControlUI.HTML_TOGGLE, Accion: Acciones.addupdate)]
        public bool GestionMetadatos { get; set; }


        public PuntoMontaje PuntoMontaje { get; set; }

    }
}
