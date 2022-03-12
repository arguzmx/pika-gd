using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PIKA.Infraestructura.Comun.Tareas
{
    public interface IProveedorTareasAutomaticas
    {
        List<TareaAutomatica> ObtieneTareasAutomaticas();
        IInstanciaTareaBackground InstanciaTarea(string DominioId, string Id, string TokenSegumiento, IConfiguration configuracion, IServiceProvider serviceProvider, CancellationToken stoppingToken);

    }
}
