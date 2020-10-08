using PIKA.Modelo.GestorDocumental;
using RepositorioEntidades;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PIKA.Servicio.GestionDocumental.Interfaces
{
    public interface IServicioPrestamo : IServicioRepositorioAsync<Prestamo, string>
    {
        Task<List<string>> Purgar();
        Task<ICollection<string>> EliminarPrestamo(string[] ids);
    }
}
