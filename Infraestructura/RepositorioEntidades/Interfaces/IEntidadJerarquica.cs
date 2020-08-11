using System;
using System.Collections.Generic;
using System.Text;

namespace RepositorioEntidades
{
    public interface IEntidadJerarquica
    {
        public Boolean EsRaiz { get; set; }
        public string NombreJerarquico { get;}
    }
}
