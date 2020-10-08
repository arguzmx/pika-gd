using PIKA.Infraestructura.Comun.Menus;
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
        Task<int> CrearActualizarAsync(string DominioId, params PermisoAplicacion[] entities);
        Task<int> EliminarAsync(string DominioId, params PermisoAplicacion[] entities);
        Task<ICollection<PermisoAplicacion>> ObtienePermisosAsync(string tipo, string id, string DominioId);
    }
}
