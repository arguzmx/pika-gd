using PIKA.Modelo.GestorDocumental;
using RepositorioEntidades;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace PIKA.Servicio.GestionDocumental.Interfaces
{
    public interface IServicioPrestamo : IServicioRepositorioAsync<Prestamo, string>
    {
        Task<List<string>> Purgar();
        Task<ICollection<string>> EliminarPrestamo(string[] ids);
        Task<Prestamo> CrearDesdeTemaAsync(Prestamo entity, string TemaId, CancellationToken cancellationToken = default);
        Task<RespuestaComandoWeb> ComandoWeb(string command, object payload);
    }
}
