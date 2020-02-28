using System;
using System.Collections.Generic;
using System.Text;

namespace RepositorioEntidades
{
    public interface IRepositoryFactory
    {
        IRepositorioAsync<T> ObtenerRepositoryAsync<T>(ICompositorConsulta<T> compositor) where T : class;
    }
}
