using System;
using System.Collections.Generic;
using System.Text;

namespace RepositorioEntidades
{
    public class FiltroConsultaPropiedad
    {

        public FiltroConsultaPropiedad()
        {
            Filtros = new List<FiltroConsulta>();
        }
        public string PropiedadId { get; set; }
        public List<FiltroConsulta> Filtros { get; set; }

    }
}
