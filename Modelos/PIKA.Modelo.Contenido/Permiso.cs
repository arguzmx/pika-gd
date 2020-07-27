using RepositorioEntidades;
using System;
using System.Collections.Generic;
using PIKA.Modelo.Metadatos;
using PIKA.Modelo.Metadatos.Atributos;
using System.Text.Json.Serialization;
using System.Xml.Serialization;

namespace PIKA.Modelo.Contenido
{

    /// <summary>
    /// Permiso aplicable al contenido 
    /// </summary>
    [Entidad(PaginadoRelacional: false, EliminarLogico: false)]
    public class Permiso: Entidad<string>
    {

        public Permiso()
        {

            Destinatarios = new HashSet<DestinatarioPermiso>();
            Carpetas = new HashSet<Carpeta>();
            Elementos = new HashSet<Elemento>();
        }

        /// <summary>
        /// Identificador único del contenido
        /// </summary>
        [Prop(Required: false, isId: true, Visible: false, OrderIndex: 0)]
        [VistaUI(ControlUI: ControlUI.HTML_HIDDEN, Accion: Acciones.update)]
        public override string Id { get; set; }

        [Prop(Required: true, OrderIndex: 5)]
        [VistaUI(ControlUI: ControlUI.HTML_TOGGLE, Accion: Acciones.addupdate)]
        public bool Leer { get; set; }

        [Prop(Required: true, OrderIndex: 10)]
        [VistaUI(ControlUI: ControlUI.HTML_TOGGLE, Accion: Acciones.addupdate)]
        public bool Escribir { get; set; }

        [Prop(Required: true, OrderIndex: 15)]
        [VistaUI(ControlUI: ControlUI.HTML_TOGGLE, Accion: Acciones.addupdate)]
        public bool Crear { get; set; }

        [Prop(Required: true, OrderIndex: 20)]
        [VistaUI(ControlUI: ControlUI.HTML_TOGGLE, Accion: Acciones.addupdate)]
        public bool Eliminar { get; set; }

        public virtual ICollection<DestinatarioPermiso> Destinatarios { get; set; }

        public virtual ICollection<Carpeta> Carpetas { get; set; }

        public virtual ICollection<Elemento> Elementos { get; set; }
    }
}
