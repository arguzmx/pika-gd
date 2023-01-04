using PIKA.Infraestructura.Comun;
using PIKA.Infraestructura.Comun.Seguridad;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PIKA.Servicio.Seguridad.Interfaces
{
    public interface IServicioSeguridadAplicaciones: IServicioAutenticado<PermisoAplicacion>
    {
        Task<int> CrearActualizarAsync(params PermisoAplicacion[] entities);
        Task<int> EliminarAsync(params PermisoAplicacion[] entities);
        Task<ICollection<PermisoAplicacion>> ObtienePermisosAsync(string tipo, string id, string DominioId);

    }
}
