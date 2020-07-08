using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace RepositorioEntidades
{
    public interface IServicioValorTextoAsync<T>
    {
        Task<List<ValorListaOrdenada>> ObtenerParesAsync(Consulta Query);
        Task<List<ValorListaOrdenada>> ObtenerParesPorId(List<string> Lista);
    }
}
