using PIKA.Modelo.Metadatos.Atributos;
using RepositorioEntidades;
using System;
using System.Collections.Generic;
using System.Text;

namespace PIKA.Modelo.Metadatos
{
    public class ValorLista: Entidad<string>
    {
        /// <summary>
        /// Identificador único del elemento de lista
        /// </summary>
        [Prop(Required: false, isId: true, Visible: false, OrderIndex: 0)]
        [VistaUI(ControlUI: ControlUI.HTML_HIDDEN, Accion: Acciones.update)]
        public override string Id { get; set; }

        /// <summary>
        /// Testo del elemento de lista
        /// </summary>
        [Prop(Required: true, OrderIndex: 5)]
        [VistaUI(ControlUI: ControlUI.HTML_TEXT, Accion: Acciones.addupdate)]
        [ValidString(minlen: 1, maxlen: 200)]
        public string Texto { get; set; }

        /// <summary>
        /// Indice para ordenar los elemento de la lista
        /// </summary>
        [Prop(Required: true, OrderIndex: 10)]
        [VistaUI(ControlUI: ControlUI.HTML_NUMBER, Accion: Acciones.update)]
        public int Indice { get; set; }


    }
}
