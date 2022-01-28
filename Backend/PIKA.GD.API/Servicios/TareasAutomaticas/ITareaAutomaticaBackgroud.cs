using System.Threading;
using System.Threading.Tasks;

namespace PIKA.GD.API.Servicios.TareasAutomaticas
{
    public interface ITareaAutomaticaBackgroud
    {
        Task DoWork(CancellationToken stoppingToken);

    }
}
