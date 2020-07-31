using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace RepositorioEntidades
{
    public interface IRepositorioJerarquia<T, U, V>
    {
        Task<List<T>> ObtenerHijosAsync(U PadreId, V JerquiaId );
        Task<List<T>> ObtenerRaicesAsync( V JerquiaId);
   
    }
}
