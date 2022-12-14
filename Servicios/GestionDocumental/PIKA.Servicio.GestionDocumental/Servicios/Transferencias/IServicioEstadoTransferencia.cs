using PIKA.Infraestructura.Comun;
using PIKA.Modelo.GestorDocumental;
using RepositorioEntidades;

namespace PIKA.Servicio.GestionDocumental.Interfaces
{
    public interface IServicioEstadoTransferencia : IServicioRepositorioAsync<EstadoTransferencia, string>, IServicioValorTextoAsync<EstadoTransferencia>
        , IServicioAutenticado<EstadoTransferencia>
    {
    }
}
