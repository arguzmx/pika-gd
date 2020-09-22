using System;
using System.Collections.Generic;
using System.Text;

namespace PIKA.Modelo.Metadatos
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true, Inherited = false)]
    public class LinkViewParameterAttribute: Attribute
    {
        private string _Vista;

        /// <summary>
        /// PArametro enviado a una vista en el forntend
        /// </summary>
        /// <param name="Vista">id unico de la vista</param>
        public LinkViewParameterAttribute( string Vista)
        {
            this._Vista = Vista;
        }

        public virtual string Vista
        {
            get { return _Vista; }
        }
    }
}
