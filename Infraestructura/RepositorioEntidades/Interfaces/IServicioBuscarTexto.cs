using Microsoft.EntityFrameworkCore.Query;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace RepositorioEntidades
{
    public interface IServicioBuscarTexto<T>
    {
        Task<IPaginado<T>> ObtenerPaginadoAsync(string Texto, Consulta Query, 
               Func<IQueryable<T>, IIncludableQueryable<T, object>> include = null,
               bool disableTracking = true,
               CancellationToken cancellationToken = default(CancellationToken));
        
    }
}
