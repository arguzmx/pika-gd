using System.Collections.Generic;
using System.Threading.Tasks;

namespace RepositorioEntidades
{
    public interface IRepositorioEntidadSeleccionada<S,T>
    {
        Task<List<T>> ObtieneSeleccionados(string TemaId, string UsuarioId);
        Task CrearSeleccion(S Id, string TemaId, string UsuarioId);
        Task EliminaSeleccion(S Id, string TemaId, string UsuarioId);
        Task CrearSeleccion(List<S> Ids, string TemaId, string UsuarioId);
        Task EliminaSeleccion(List<S> Ids, string TemaId, string UsuarioId);
        Task BorraSeleccion(string TemaId, string UsuarioId);
        Task<S> CreaTema(string Tema, string UsuarioId);
        Task EliminaTema(string Id, string UsuarioId);
        Task<List<ValorListaOrdenada>> ObtienTemas(string usuatioId);


    }
}
