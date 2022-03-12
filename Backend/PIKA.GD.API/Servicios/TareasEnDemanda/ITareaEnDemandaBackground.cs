using System.Threading;
using System.Threading.Tasks;

namespace PIKA.GD.API.Servicios.TareasAutomaticas
{
    public interface ITareaEnDemandaBackground
    {
        Task DoWork(CancellationToken stoppingToken);

    }
}
