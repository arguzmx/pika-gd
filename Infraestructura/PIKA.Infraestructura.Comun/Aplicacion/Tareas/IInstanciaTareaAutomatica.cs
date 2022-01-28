using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PIKA.Infraestructura.Comun.Tareas
{
    public interface IInstanciaTareaAutomatica
    {

        event EventHandler TareaFinalizada;
        Task<ResultadoTareaAutomatica> EjecutarTarea();
    }
}
