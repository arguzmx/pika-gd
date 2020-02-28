using System;
using System.Collections.Generic;
using System.Text;

namespace RepositorioEntidades
{
    public interface IPaginado<T>
    {
        int Desde { get; }

        int Indice { get; }

        int Tamano { get; }

        int Conteo { get; }

        int Paginas { get; }

        IList<T> Elementos { get; }

        bool TienePrevio { get; }

        bool TieneSiguiente { get; }
    }
}
