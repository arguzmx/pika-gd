using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PIKA.Infraestructura.Comun.Tareas
{
    public interface IProveedorTareasEnDemanda
    {
        List<TareaEnDemanda> ObtieneTareasEnDemanda();

        TareaEnDemanda ObtieneTarea(string Id);
        IInstanciaTareaEnDemanda InstanciaTarea(string DominioId, string Id, string TokenSegumiento, IConfiguration configuracion, IServiceProvider serviceProvider, CancellationToken stoppingToken);

    }
}
