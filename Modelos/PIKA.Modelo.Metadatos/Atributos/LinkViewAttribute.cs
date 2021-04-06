using System;
using System.Collections.Generic;
using System.Text;

namespace PIKA.Modelo.Metadatos
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
    public class LinkViewAttribute: Attribute
    {
        private string _Vista;
        private string _Icono;
        private string _Titulo;
        private bool _RequireSeleccion;

        /// <summary>
        /// Vínculo a vista en el frontend
        /// </summary>
        /// <param name="Vista">id único de lavista</param>
        /// <param name="Icono">icono de la librería material</param>
        /// <param name="Titulo">titulo o id de traducción</param>
        public LinkViewAttribute(string Vista, string Icono, string Titulo, bool RequireSeleccion = true)
        {
            this._Vista = Vista;
            this._Icono = Icono;
            this._Titulo = Titulo;
            this._RequireSeleccion = RequireSeleccion;
        }

        public virtual string Vista
        {
            get { return _Vista; }
        }

        public virtual string Icono
        {
            get { return _Icono; }
        }

        public virtual string Titulo
        {
            get { return _Titulo; }
        }

        public virtual bool RequireSeleccion
        {
            get { return _RequireSeleccion; }
        }
    }
}
