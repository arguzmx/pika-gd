using System;
using System.Collections.Generic;
using System.Text;

namespace RepositorioEntidades
{

    /// <summary>
    /// 
    /// </summary>
    public interface IEntidadRegistroCreacion
    {

        string CreadorId { get; set; }

        DateTime FechaCreacion { get; set; }

    }
}
