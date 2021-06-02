using PIKA.Modelo.Contenido;
using RepositorioEntidades;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PIKA.ServicioBusqueda.Contenido
{
    public interface IServicioBusquedaContenido
    {
        // DEvuelve el identificador del cache donde se encuentran almacenados los elemeentos
        Task<Paginado<string>> BuscarIds(BusquedaContenido busqueda);


        Task<Paginado<ElementoBusqueda>> Buscar(BusquedaContenido busqueda);
    }
}
