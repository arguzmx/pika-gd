using System;
using System.Collections.Generic;
using System.Text;

namespace PIKA.Modelo.Metadatos
{
    [AttributeUsage(AttributeTargets.All, AllowMultiple = false, Inherited = false)]
    public class TblAttribute : Attribute
    {

        private bool _IncludeIntable;
        private bool _Visible;
        private bool _Togglable;
        private int _OrderIndex;
        private bool _Searchable;
        private bool _Orderable;
        private string _TableClientId;

        public TblAttribute(string TableClientId = "*", bool IncludeIntable = true,
            bool Visible = true, bool Togglable = true, bool Searchable=false,
            bool Orderable = false, int OrderIndex = 0)
        {
            this._IncludeIntable = IncludeIntable;
            this._Visible = Visible;
            this._Togglable = Togglable;
            this._OrderIndex = OrderIndex;
            this._TableClientId = TableClientId;
            this._Searchable = Searchable;
            this._Orderable = Orderable;
        }

        public virtual string TableClientId
        {
            get { return _TableClientId; }
        }

        public virtual int OrderIndex
        {
            get { return _OrderIndex; }
        }

        public virtual bool Togglable
        {
            get { return _Togglable; }
        }

        public virtual bool IncludeIntable
        {
            get { return _IncludeIntable; }
        }

        public virtual bool Visible
        {
            get { return _Visible; }
        }

        public virtual bool Searchable
        {
            get { return _Searchable; }
        }

        public virtual bool Orderable
        {
            get { return _Orderable; }
        }
    }
}
