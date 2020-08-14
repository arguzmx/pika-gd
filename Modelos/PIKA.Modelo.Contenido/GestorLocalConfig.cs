using PIKA.Infraestructura.Comun;
using PIKA.Modelo.Metadatos;
using PIKA.Modelo.Metadatos.Atributos;
using System;
using System.Collections.Generic;
using System.Text;

namespace PIKA.Modelo.Contenido
{
    public class GestorLocalConfig
    {
        [Prop(Required: false, isId: true, Visible: false, OrderIndex: 0,
             Contextual: true, IdContextual: ConstantesModelo.PREFIJO_CONEXTO + "VolumenId")]
        [VistaUI(ControlUI: ControlUI.HTML_HIDDEN, Accion: Acciones.addupdate)]
        public string VolumenId { get; set; }


        [Prop(Required: true, OrderIndex: 10)]
        [VistaUI(ControlUI: ControlUI.HTML_TEXT, Accion: Acciones.addupdate)]
        [ValidString(minlen: 2, maxlen: 200)]
        public string Ruta { get; set; }
        public Volumen Volumen { get; set; }

    }


   

}
