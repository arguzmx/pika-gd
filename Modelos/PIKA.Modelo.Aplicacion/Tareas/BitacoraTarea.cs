using System;
using System.Collections.Generic;
using System.Text;
using PIKA.Constantes.Aplicaciones.Aplicaciones;
using PIKA.Modelo.Metadatos;
using PIKA.Modelo.Metadatos.Atributos;

namespace PIKA.Modelo.Aplicacion.Tareas
{

    [Entidad(PaginadoRelacional: false, EliminarLogico: false,
        PermiteAltas:false, PermiteBajas:false,PermiteCambios:false,
        TokenApp: ConstantesAppAplicacionPlugin.APP_ID, TokenMod: ConstantesAppAplicacionPlugin.MODULO_APLICACIONES)]
    public class BitacoraTarea
    {

        [Prop(Required: false, isId: true, Visible: false, OrderIndex: 0)]
        [VistaUI(ControlUI: ControlUI.HTML_HIDDEN, Accion: Acciones.none)]
        public  string Id { get; set; }

        /// <summary>
        /// Identificador único de la tarea programada
        /// </summary>
        [Prop(Required: false, isId: true, Visible: false, OrderIndex: 0)]
        [VistaUI(ControlUI: ControlUI.HTML_HIDDEN, Accion: Acciones.none)]
        public  string TareaId { get; set; }

        /// <summary>
        /// Determina cuando fue realizada la última ejecución
        /// </summary>
        [Prop(Required: false, isId: true, Visible: true, OrderIndex: 10)]
        [VistaUI(ControlUI: ControlUI.HTML_DATETIME, Accion: Acciones.none)]
        public  DateTime FechaEjecucion { get; set; }

        /// <summary>
        /// Duración en minutos de la última ejecución de la tarea
        /// </summary>
        [Prop(Required: false, isId: true, Visible: true, OrderIndex: 20)]
        [VistaUI(ControlUI: ControlUI.HTML_NUMBER, Accion: Acciones.none)]
        public  int Duracion { get; set; }

        /// <summary>
        /// Determina si la  última ejecución resulto exitosa
        /// </summary>
        [Prop(Required: false, isId: true, Visible: true, OrderIndex: 30)]
        [VistaUI(ControlUI: ControlUI.HTML_TOGGLE, Accion: Acciones.none)]
        public  bool Exito { get; set; }

        /// <summary>
        /// En el caso de que la tare falle almacena el error asociado
        /// </summary>

        [Prop(Required: false, isId: true, Visible: true, OrderIndex: 40)]
        [VistaUI(ControlUI: ControlUI.HTML_TEXT, Accion: Acciones.none)]
        public  string CodigoError { get; set; }

        public TareaAutomatica Tarea { get; set; }
    }
}
