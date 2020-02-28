using System;
using System.Collections.Generic;
using System.Text;

namespace RepositorioEntidades.DatatablesPlugin
{


    public class RespuestaDatatables<T>
    {
        public int draw { get; set; }

        public int recordsTotal { get; set; }

        public int recordsFiltered { get; set; }

        public T[] data { get; set; }

        public string error { get; set; }
    }
 
}
