using System;
using System.Collections.Generic;
using System.Text;

namespace RepositorioEntidades
{
    public interface IEntidadCatalogo<T>
    {
        List<T> Seed();
    }
}
