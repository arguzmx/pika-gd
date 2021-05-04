using PIKA.Modelo.Contenido;
using RepositorioEntidades;
using System.Threading.Tasks;

namespace PIKA.ServicioBusqueda.Contenido
{
    public interface IServicioBusquedaContenido
    {
        Task<Paginado<ElementoBusqueda>> Buscar(BusquedaContenido busqueda);
    }
}
