using PIKA.Modelo.GestorDocumental;
using RepositorioEntidades;

namespace PIKA.Servicio.GestionDocumental.Interfaces
{
    public interface IServicioActivoPrestamo : IServicioRepositorioAsync<ActivoPrestamo, string>
    {
    }
}

