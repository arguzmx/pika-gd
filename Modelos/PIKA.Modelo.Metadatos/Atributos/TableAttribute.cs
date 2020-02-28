using System;
using System.Collections.Generic;
using System.Text;

namespace PIKA.Modelo.Metadatos
{
    public class TableAttribute
    {
        public bool IncludeIntable { get; set; }
        public bool Visible { get; set; }
        public bool Togglable { get; set; }
        public int OrderIndex { get; set; }
        public string TableClientId { get; set; }
    }
}
