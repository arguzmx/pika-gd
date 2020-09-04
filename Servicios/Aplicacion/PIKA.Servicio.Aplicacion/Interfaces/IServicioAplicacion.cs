using PIKA.Infraestructura.Comun;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PIKA.Servicio.AplicacionPlugin.Interfaces
{
    public interface IServicioAplicacion
    {
        Task<List<Aplicacion>> OntieneAplicaciones(string AppPath);
    }
}
