using System;
using System.Collections.Generic;
using System.Text;

namespace PIKA.Modelo.Metadatos
{

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
    public class MenuAttribute: Attribute
    {
        private string _Icono;
        private string _Titulo;
        private string _Condicion;
        private string _MenuId;
        private int _MenuIndex;

        public MenuAttribute(string Icono, string Titulo, string MenuId, string Condicion = "", int menuIndex = 0)
        {
            this._Icono = Icono;
            this._Titulo = Titulo;
            this._Condicion = Condicion;
            _MenuId = MenuId;
            _MenuIndex = menuIndex;
        }

        /// <summary>
        /// POsición del elemento en el menú
        /// </summary>
        public virtual int MenuIndex
        {
            get { return _MenuIndex; }
        }

        /// <summary>
        /// Identificador único del menú al que pertenece la opcion
        /// </summary>
        public virtual string MenuId
        {
            get { return _MenuId; }
        }

        public virtual string Condicion
        {
            get { return _Condicion; }
        }

        public virtual string Icono
        {
            get { return _Icono; }
        }

        public virtual string Titulo
        {
            get { return _Titulo; }
        }

    }
}
