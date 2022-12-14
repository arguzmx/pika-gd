using PIKA.Infraestructura.Comun;
using PIKA.Modelo.GestorDocumental;
using RepositorioEntidades;

namespace PIKA.Servicio.GestionDocumental.Interfaces
{
    public interface IServicioComentarioTransferencia : IServicioRepositorioAsync<ComentarioTransferencia, string>,
        IServicioAutenticado<ComentarioTransferencia>
    {
    }
}
