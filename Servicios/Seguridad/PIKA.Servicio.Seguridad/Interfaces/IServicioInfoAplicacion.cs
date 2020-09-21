using PIKA.Infraestructura.Comun;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PIKA.Servicio.Seguridad.Interfaces
{
    public interface IServicioInfoAplicacion
    {
        Task<List<Aplicacion>> ObtieneAplicaciones(string AppPath);
    }
}
