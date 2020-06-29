using PIKA.Modelo.Metadatos.Atributos;
using System;
using System.Collections.Generic;
using System.Text;

namespace PIKA.Modelo.Metadatos
{
    [AttributeUsage(AttributeTargets.All, AllowMultiple = false, Inherited = false)]
    public class EventAttribute : Attribute
    {


        public EventAttribute(string Entidad, Eventos Evento, Operaciones Opeacion, string Parametros) {
            this._Entidad = Entidad;
            this._Evento = Evento;
            this._Operacion = Operacion;
            this._Parametros = Parametros;
        }

        private string _Entidad;

        /// <summary>
        /// Nombre de la entidad emisora del evento
        /// </summary>
        public virtual string Entidad
        {
            get { return _Entidad; }
        }


        private string _Parametros;

        /// <summary>
        /// Nombre de la Parametros emisora del evento
        /// </summary>
        public virtual string Parametros
        {
            get { return _Parametros; }
        }

        private Operaciones _Operacion;

        /// <summary>
        /// Operacion para notificar el cambio
        /// </summary>
        public Operaciones Operacion { get => _Operacion; }


        private Eventos _Evento;
        
        /// <summary>
        /// Evento para notificar el cambio
        /// </summary>
        public Eventos  Evento { get => _Evento; }

    }
}
