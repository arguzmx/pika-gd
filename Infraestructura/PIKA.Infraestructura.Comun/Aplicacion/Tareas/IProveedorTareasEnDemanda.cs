using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PIKA.Infraestructura.Comun.Tareas
{
    public interface IProveedorTareasEnDemanda: IDisposable
    {
        List<TareaEnDemanda> ObtieneTareasEnDemanda();
        Task EliminaCaducos(string DominioId, string IdProceso, string IdTarea, string TokenSegumiento, IConfiguration configuracion, IServiceProvider serviceProvider, CancellationToken stoppingToken, string InputPayload, string OutputPayload);
        TareaEnDemanda ObtieneTarea(string Id);
        IInstanciaTareaBackground InstanciaTarea(string DominioId, string IdProceso, string IdTarea, string TokenSegumiento, IConfiguration configuracion, IServiceProvider serviceProvider, CancellationToken stoppingToken);

    }
}
