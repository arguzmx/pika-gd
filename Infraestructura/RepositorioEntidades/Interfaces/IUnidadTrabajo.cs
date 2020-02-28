using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace RepositorioEntidades
{
    public interface IUnidadDeTrabajo : IDisposable
    {
        IRepositorioAsync<T> ObtenerRepositoryAsync<T>(ICompositorConsulta<T> compositor) where T : class;
        
        int SaveChanges();
    }



    public interface IUnidadDeTrabajo<TContext> : IUnidadDeTrabajo where TContext : DbContext
    {
        TContext Context { get; }
    }
}
