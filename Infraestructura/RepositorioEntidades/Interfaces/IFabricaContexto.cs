using System;
using System.Collections.Generic;
using System.Text;

namespace RepositorioEntidades
{
    public interface IFabricaContexto<T>
    {
        T Crear();
    }

}
