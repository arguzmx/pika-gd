using System;
using System.Collections.Generic;
using System.Text;

namespace PIKA.Modelo.Metadatos
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true, Inherited = false)]
    public class LinkViewParameterAttribute: Attribute
    {
        private string _Vista;
        private string _ParamName;
        private bool _Multiple;
        /// <summary>
        /// PArametro enviado a una vista en el forntend
        /// </summary>
        /// <param name="Vista">id unico de la vista</param>
        public LinkViewParameterAttribute( string Vista, bool Multiple = false, string ParamName ="" )
        {
            this._Vista = Vista;
            this._Multiple = Multiple;
        }

        public virtual bool Multiple
        {
            get { return _Multiple; }
        }

        public virtual string ParamName
        {
            get { return _ParamName; }
        }

        public virtual string Vista
        {
            get { return _Vista; }
        }
    }
}
