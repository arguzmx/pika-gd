using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PIKA.Infraestructura.Comun.Tareas
{
    public interface IInstanciaTareaEnDemanda
    {

        event EventHandler TareaFinalizada;
        Task<ResultadoTareaAutomatica> EjecutarTarea(string InputPayload = null);
    }
}
