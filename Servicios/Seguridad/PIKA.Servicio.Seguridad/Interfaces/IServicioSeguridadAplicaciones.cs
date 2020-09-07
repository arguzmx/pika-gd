using PIKA.Infraestructura.Comun.Seguridad;
using RepositorioEntidades;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PIKA.Servicio.Seguridad.Interfaces
{
    public interface IServicioSeguridadAplicaciones
    {
        Task<int> CrearActualizarAsync(params PermisoAplicacion[] entities);
        Task<int> EliminarAsync(params PermisoAplicacion[] entities);
        Task<ICollection<PermisoAplicacion>> ObtienePermisosAsync(string tipo, string id);
     }
}
