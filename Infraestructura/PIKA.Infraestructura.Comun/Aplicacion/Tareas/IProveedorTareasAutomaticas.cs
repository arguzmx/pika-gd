using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PIKA.Infraestructura.Comun.Tareas
{
    public interface IProveedorTareasAutomaticas
    {
        public List<TareaAutomatica> ObtieneTareasAutomaticas();
        public Task<ResultadoTareaAutomatica> EjecutarTarea(string DominioId, string Id, IConfiguration configuracion, IServiceProvider serviceProvider);

    }
}
