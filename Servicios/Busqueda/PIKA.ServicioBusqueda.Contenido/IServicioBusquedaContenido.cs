using PIKA.Modelo.Contenido;
using PIKA.Servicio.Contenido.ElasticSearch;
using RepositorioEntidades;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PIKA.ServicioBusqueda.Contenido
{
    public interface IServicioBusquedaContenido
    {
        // DEvuelve el identificador del cache donde se encuentran almacenados los elemeentos
        Task<Paginado<string>> BuscarIds(BusquedaContenido busqueda);
        Task<List<HighlightHit>> BuscarSinopsis(string Id, List<string> Ids);
        Task<Paginado<ElementoBusqueda>> Buscar(BusquedaContenido busqueda);

    }
}
