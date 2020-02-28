using System;
using System.Collections.Generic;
using System.Text;

namespace PIKA.Modelo.Metadatos
{
    [AttributeUsage(AttributeTargets.All, AllowMultiple = true, Inherited = false)]
    public class AttrAttribute : Attribute
    {

        /// <summary>
        /// Atributo para definir la longitud de un objeto
        /// </summary>
        public const string Len = "len";


        /// <summary>
        /// Determina si la propiedad es un rango 
        /// </summary>
        public const string IsRange = "isrange";


        private string _Id;
        private object _Value;

        public AttrAttribute(string Id, object Value)
        {
            this._Id = Id;
            this._Value = Value;
        }


        public virtual string Id
        {
            get { return _Id; }
        }

        public virtual object Value
        {
            get { return _Value; }
        }
    }
}
