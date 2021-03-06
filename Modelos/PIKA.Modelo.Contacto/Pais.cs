﻿using PIKA.Modelo.Metadatos;
using PIKA.Modelo.Metadatos.Atributos;
using RepositorioEntidades;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;
using System.Xml.Serialization;

namespace PIKA.Modelo.Contacto
{
    [Entidad(EliminarLogico: false)]
    public class Pais : EntidadCatalogo<string, Pais>
    {
        public Pais()
        {

            Estados = new HashSet<Estado>();

        }

        [Prop(Required: false, isId: true, Visible: false, OrderIndex: 0)]
        [VistaUI(ControlUI: ControlUI.HTML_HIDDEN, Accion: Acciones.update)]
        public override string Id { get; set; }

        [Prop(Required: true, OrderIndex: 10)]
        [VistaUI(ControlUI: ControlUI.HTML_TEXT, Accion: Acciones.addupdate)]
        public override string Nombre { get; set; }

        /// <summary>
        /// Esatdos asociados al pais
        /// </summary>
        public virtual ICollection<Estado> Estados { get; set; }

        /// <summary>
        /// Navegacion
        /// </summary>
        [XmlIgnore]
        [JsonIgnore]
        public virtual ICollection<DireccionPostal> Direcciones { get; set; }

    }
}
