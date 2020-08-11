using PIKA.Modelo.Metadatos.Atributos;
using System;
using System.Collections.Generic;
using System.Text;

namespace PIKA.Modelo.Metadatos
{
    [AttributeUsage(AttributeTargets.All, AllowMultiple = true, Inherited = false)]
    public class VistaUIAttribute: Attribute
    {

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ControlUI">Tipo de control en base a las constante sde la clase ControlUI</param>
        /// <param name="Accion">Accion CRUD en la que participa la vista</param>
        public VistaUIAttribute(string ControlUI= ControlUI.HTML_NONE, Acciones Accion =  Acciones.none, string Plataforma = ControlUI.PLATAFORMA_WEB) {
            this.Control = ControlUI;
            this.Accion = Accion;
            this.Plataforma = Plataforma;
        }

        /// <summary>
        /// Tiupo de control de visualización en base a la clase ControleUI
        /// </summary>
        public string Control { get; }

        /// <summary>
        /// Accion para la cual se encuentra destiando el desplieguie de UI
        /// </summary>
        public Acciones Accion { get; }

        /// <summary>
        /// defina la plataforma para la cual esta destiando la vista
        /// </summary>
        public string Plataforma { get; set; }


    }
}
