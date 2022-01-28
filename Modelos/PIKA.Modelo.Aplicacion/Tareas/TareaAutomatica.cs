using PIKA.Infraestructura.Comun;
using PIKA.Modelo.Metadatos.Atributos;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using PIKA.Modelo.Metadatos;
using PIKA.Constantes.Aplicaciones.Aplicaciones;

namespace PIKA.Modelo.Aplicacion.Tareas
{
    [EntidadVinculada(TokenSeguridad: ConstantesAppAplicacionPlugin.MODULO_APLICACIONES,
        EntidadHijo: "BitacoraTarea",
        Cardinalidad: TipoCardinalidad.UnoVarios, PropiedadPadre: "Id",
        PropiedadHijo: "TareaId")]

    [Entidad(PaginadoRelacional: false, EliminarLogico: false,
    PermiteAltas: false, PermiteBajas: false, PermiteCambios: true,
    TokenApp: ConstantesAppAplicacionPlugin.APP_ID, TokenMod: ConstantesAppAplicacionPlugin.MODULO_APLICACIONES)]
    public class TareaAutomatica
    {
        public TareaAutomatica()
        {
            Bitacora = new HashSet<BitacoraTarea>();
        }

        [NotMapped]
        public string TipoOrigenDefault => ConstantesModelo.IDORIGEN_DOMINIO;

        [Prop(Required: false, isId: true, Visible: false, OrderIndex: 2000)]
        [VistaUI(ControlUI: ControlUI.HTML_HIDDEN, Accion: Acciones.update)]
        public string Id { get; set; }


        /// <summary>
        /// NOmbre de la tarea
        /// </summary>
        [Prop(Required: false, isId: false, Visible: true, OrderIndex: 2010)]
        [VistaUI(ControlUI: ControlUI.HTML_HIDDEN, Accion: Acciones.none)]
        public string Nombre { get; set; }


        /// <summary>
        /// Determina cuando fue realizada la última ejecución
        /// </summary>
        [Prop(Required: false, isId: false, Visible: true, OrderIndex: 2020)]
        [VistaUI(ControlUI: ControlUI.HTML_DATETIME, Accion: Acciones.none)]
        public DateTime? UltimaEjecucion { get; set; }

        /// <summary>
        /// Determina si la  última ejecución resulto exitosa
        /// </summary>
        [Prop(Required: false, isId: false, Visible: true, OrderIndex: 2030)]
        [VistaUI(ControlUI: ControlUI.HTML_TOGGLE, Accion: Acciones.none)]
        public bool? Exito { get; set; }

        /// <summary>
        /// Duración en minutos de la última ejecución de la tarea
        /// </summary>
        [Prop(Required: false, isId: false, Visible: true, OrderIndex: 2040)]
        [VistaUI(ControlUI: ControlUI.HTML_NUMBER, Accion: Acciones.none)]
        public int? Duracion { get; set; }

        /// <summary>
        /// Determina cuando será realizada la próxima ejecución
        /// </summary>
        [Prop(Required: false, isId: false, Visible: true, OrderIndex: 2050)]
        [VistaUI(ControlUI: ControlUI.HTML_DATETIME, Accion: Acciones.none)]
        public DateTime? ProximaEjecucion { get; set; }

        /// <summary>
        /// En el caso de que la tare falle almacena el error asociado
        /// </summary>
        [Prop(Required: false, isId: false, Visible: true, OrderIndex: 2060)]
        [VistaUI(ControlUI: ControlUI.HTML_TEXT, Accion: Acciones.none)]
        public string CodigoError { get; set; }
        //Unico =0 , Hora =1 , Diario=2, DiaSemana=3, DiaMes=4
        [Prop(Required: false, isId: false, Visible: false, ShowInTable: false, OrderIndex: 70)]
        [VistaUI(ControlUI: ControlUI.HTML_SELECT, Accion: Acciones.update)]
        [List(Entidad: "", Default: "2", DatosRemotos: false, OrdenarAlfabetico: false, ValoresCSV: "0|listas.tareaautomatica.unico,1|listas.tareaautomatica.hora,2|listas.tareaautomatica.diario,3|listas.tareaautomatica.diasemana,4|listas.tareaautomatica.diames,10|listas.tareaautomatica.continuo")]
        public Infraestructura.Comun.Tareas.PeriodoProgramacion Periodo { get; set; }



        /// <summary>
        /// El estado de la tarea sólo puede ser modificado por el sistema o en base a un comando no es parte de la captura del usaurio
        /// </summary>
        [Prop(Required: false, isId: false, Visible: true, ShowInTable: true, OrderIndex: 2070)]
        [VistaUI(ControlUI: ControlUI.HTML_NONE, Accion: Acciones.none)]
        [List(Entidad: "", Default: "0", DatosRemotos: false, OrdenarAlfabetico: false, ValoresCSV: "0|listas.tareaautomatica.habilidata,1|listas.tareaautomatica.enejecucion,2|listas.tareaautomatica.pausada,3|Error configuración")]
        public Infraestructura.Comun.Tareas.EstadoTarea Estado { get; set; }


        /// <summary>
        /// Determina si la tarea se encuentra en ejjecución continua, es decir que no requiere programación
        /// </summary>
        [Prop(Required: false, isId: false, Visible: true, ShowInTable: true, OrderIndex: 2080)]
        [VistaUI(ControlUI: ControlUI.HTML_NONE, Accion: Acciones.none)]
        public bool TareaEjecucionContinua { get; set; }


        /// <summary>
        /// Minutos de espera para el relanzamiento de la tarea una vez finalizada la instancia previa
        /// </summary>
        [Prop(Required: false, isId: false, Visible: true, ShowInTable: true, OrderIndex: 2090)]
        [VistaUI(ControlUI: ControlUI.HTML_NONE, Accion: Acciones.none)]
        public int TareaEjecucionContinuaMinutos { get; set; }

        /// <summary>
        /// Fecha u hora de ejecución de acuerdo al tipo programado
        /// </summary>
        [Prop(Required: false, isId: false, Visible: true, ShowInTable: false, OrderIndex: 80)]
        [VistaUI(ControlUI: ControlUI.HTML_DATETIME, Accion: Acciones.update)]
        [Event(Entidad: "Periodo", Evento: Eventos.AlCambiar, Operacion: Operaciones.Mostrar, "{0} === 0")]
        public DateTime? FechaHoraEjecucion { get; set; }


        /// <summary>
        /// Fecha u hora de ejecución de acuerdo al tipo programado
        /// </summary>
        [NotMapped]
        [Prop(Required: false, isId: false, Visible: true, ShowInTable: false, OrderIndex: 80)]
        [VistaUI(ControlUI: ControlUI.HTML_TIME, Accion: Acciones.update)]
        [Event(Entidad: "Periodo", Evento: Eventos.AlCambiar, Operacion: Operaciones.Mostrar, "{0} !== 0")]
        public DateTime? HoraEjecucion { get; set; }


        /// <summary>
        /// Determina el intervalo para la ejecución de acuerdo al tipo
        /// Unico, no aplica
        /// Diario, no aplica
        /// Hora, se ejecuta cada Intervalo de horas
        /// DiaSemana, día de la semana iniciando en domingo =0
        /// DiaMes, día del mes si el mes tiene menos días se recorre al último disponible 
        /// </summary>
        [Prop(Required: false, isId: false, Visible: false, ShowInTable: false, OrderIndex: 90)]
        [VistaUI(ControlUI: ControlUI.HTML_NUMBER, Accion: Acciones.update)]
        [Event(Entidad: "Periodo", Evento: Eventos.AlCambiar, Operacion: Operaciones.Mostrar, "{0}===1")]
        public int? Intervalo { get; set; }

        [NotMapped]
        [Prop(Required: false, isId: false, Visible: false, ShowInTable: false, OrderIndex: 90)]
        [VistaUI(ControlUI: ControlUI.HTML_SELECT, Accion: Acciones.update)]
        [List(Entidad: "", Default: "1", DatosRemotos: false, OrdenarAlfabetico: false, ValoresCSV: "1|listas.tareaautomatica.lunes,2|listas.tareaautomatica.martes,3|listas.tareaautomatica.miercoles,4|listas.tareaautomatica.jueves,5|listas.tareaautomatica.viernes,6|listas.tareaautomatica.sabado,0|listas.tareaautomatica.domingo")]
        [Event(Entidad: "Periodo", Evento: Eventos.AlCambiar, Operacion: Operaciones.Mostrar, "{0}===3")]
        public int? DiaSemana { get; set; }

        [NotMapped]
        [Prop(Required: false, isId: false, Visible: false, ShowInTable: false, OrderIndex: 90)]
        [VistaUI(ControlUI: ControlUI.HTML_NUMBER, Accion: Acciones.update)]
        [ValidNumeric(min: 1, max:31, usemin: true, usemax: true)]
        [Event(Entidad: "Periodo", Evento: Eventos.AlCambiar, Operacion: Operaciones.Mostrar, "{0}===4")]
        public int? DiaMes { get; set; }


        [NotMapped]
        [Prop(Required: false, isId: false, Visible: true, OrderIndex: 90)]
        [VistaUI(ControlUI: ControlUI.HTML_LABEL, Accion: Acciones.none)]
        public string EtiquetaIntervalo { get; set; }

        [NotMapped]
        [Prop(Required: false, isId: false, Visible: true, OrderIndex: 90)]
        [VistaUI(ControlUI: ControlUI.HTML_LABEL, Accion: Acciones.none)]
        public string EtiquetaPeriodo { get; set; }


        [NotMapped]
        [Prop(Required: false, isId: false, Visible: true, OrderIndex: 90)]
        [VistaUI(ControlUI: ControlUI.HTML_LABEL, Accion: Acciones.none)]
        public string EtiquetaFecha { get; set; }

        /// <summary>
        /// El tipo de origen para las tareas programadas es por dominio
        /// </summary>
        [Prop(Required: false, isId: false, Visible: false, ShowInTable: false, OrderIndex: 1000)]
        [VistaUI(ControlUI: ControlUI.HTML_HIDDEN, Accion: Acciones.update)]
        public  string TipoOrigenId { get; set; }

        /// <summary>
        /// Identificador único del dominio para la ejecución de tareas
        /// </summary>
        [Prop(Required: false, isId: false, Visible: false, ShowInTable: false, OrderIndex: 1000)]
        [VistaUI(ControlUI: ControlUI.HTML_HIDDEN, Accion: Acciones.update)]
        public  string OrigenId { get; set; }


        /// <summary>
        /// Nombre completo del ensamblado que proporciona la tarea
        /// </summary>
        [Prop(Required: false, isId: false, Visible: false, ShowInTable: false, OrderIndex: 1000)]
        [VistaUI(ControlUI: ControlUI.HTML_HIDDEN, Accion: Acciones.update)]
        public  string Ensamblado { get; set; }


        public ICollection<BitacoraTarea> Bitacora { get; set; }
    }
}
