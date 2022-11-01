using System;
using System.Collections.Generic;
using System.Text;

namespace PIKA.Modelo.Metadatos
{
    public  class ParametroLinkVista
    {
        public virtual string ParamName
        {
            get; set;
        }

        public virtual bool Multiple
        {
            get; set;
        }

        public virtual string Vista
        {
            get; set;
        }
    }
}
