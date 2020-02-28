using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace RepositorioEntidades
{
    public class UnidadDeTrabajo<TContext> : IRepositoryFactory, IUnidadDeTrabajo<TContext>, IUnidadDeTrabajo
           where TContext : DbContext, IDisposable
    {
        private Dictionary<Type, object> _repositories;
        
        public UnidadDeTrabajo(TContext context)
        {
            Context = context ?? throw new ArgumentNullException(nameof(context));
        }

 
        public TContext Context { get; }

        public int SaveChanges()
        {
            return Context.SaveChanges();
        }

        public void Dispose()
        {
            Context?.Dispose();
        }

        public IRepositorioAsync<T> ObtenerRepositoryAsync<T>( ICompositorConsulta<T> compositor ) where T : class
        {
            if (_repositories == null) _repositories = new Dictionary<Type, object>();
            var type = typeof(T);
            if (!_repositories.ContainsKey(type)) _repositories[type] = new RepositorioAsync<T>(Context, compositor);
            return (IRepositorioAsync<T>)_repositories[type];
        }

 
    }
}
