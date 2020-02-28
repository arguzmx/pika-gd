using System;
using System.Collections.Generic;
using System.Text;

namespace RepositorioEntidades.DatatablesPlugin
{
 

    public class SolicitudDatatables
    {

        public SolicitudDatatables()
        {
            this.order = new List<DatatatablesOrder>();
            this.columns = new List<Datatablescolumn>();
            this.Filters = new List<FiltroConsulta>();
            this.search = new DatatablesSearch() { regex = false, value = "" };
        }
        public int draw { get; set; }
        public int start { get; set; }

        public int length { get; set; }

        public DatatablesSearch search { get; set; }

        public List<DatatatablesOrder> order { get; set; }

        public List<Datatablescolumn> columns { get; set; }

        public List<FiltroConsulta> Filters { get; set; }
    }



    public class DatatablesSearch
    {
        public string value { get; set; }
        public bool regex { get; set; }
    }


    public class DatatatablesOrder
    {
        public int column { get; set; }
        public string dir { get; set; }

    }


    public class Datatablescolumn
    {

        public Datatablescolumn()
        {
            this.search = new DatatablesSearch() { regex = false, value = "" };
        }

        public string data { get; set; }
        public string name { get; set; }
        public bool searchable { get; set; }

        public bool orderable { get; set; }

        public DatatablesSearch search { get; set; }
    }
}
