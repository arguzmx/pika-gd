using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PIKA.Infraestructura.Comun.Tareas
{
    public interface IInstanciaTareaBackground
    {

        event EventHandler TareaFinalizada;
        Task<ResultadoTareaBackground> EjecutarTarea(string InputPayload = null);
        Task<ResultadoTareaBackground> CaducarTarea(string InputPayload = null, string OutputPayload = null);
    }
}
