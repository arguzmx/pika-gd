using PIKA.Infraestructura.Comun;
using PIKA.Modelo.GestorDocumental;
using RepositorioEntidades;

namespace PIKA.Servicio.GestionDocumental.Interfaces
{
    public interface IServicioEventoTransferencia : IServicioRepositorioAsync<EventoTransferencia, string>
        , IServicioAutenticado<EstadoTransferencia>
    {
    }
}
