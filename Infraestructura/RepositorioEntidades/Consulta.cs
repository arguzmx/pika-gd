﻿using System;
using System.Collections.Generic;
using System.Text;

namespace RepositorioEntidades
{
    public class Consulta : ParametrosConsulta
    {
        public Consulta()
        {

            this.Filtros = new List<FiltroConsulta>();
        }

        public List<FiltroConsulta> Filtros { get; set; }

    }
}
