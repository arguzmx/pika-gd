using System;
using System.Collections.Generic;
using System.Text;

namespace PIKA.Modelo.Metadatos
{
    [AttributeUsage(AttributeTargets.All, AllowMultiple = false, Inherited = false)]
    public class EntidadAttribute : Attribute
    {

        private bool _EliminarLogico;

        public EntidadAttribute(bool EliminarLogico = false)
        {
            this._EliminarLogico = EliminarLogico;
        }

        public virtual bool EliminarLogico
        {
            get { return _EliminarLogico; }
        }

        
    }
}
