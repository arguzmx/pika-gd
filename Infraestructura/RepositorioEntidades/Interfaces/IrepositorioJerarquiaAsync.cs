using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace RepositorioEntidades.Interfaces
{
    public interface IrepositorioJerarquiaAsync<T, U>
    {
        Task<List<T>> ObtenerHijosAsync(U PadreId);
        Task<T> ObtenerPadreAsync(U HijoId);

        Task<List<ValorListaOrdenada>> ObtenerParesHijosAsync(U PadreId);

    }
}
