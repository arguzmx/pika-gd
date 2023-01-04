using System.Collections.Generic;
using System.Threading.Tasks;
using PIKA.Infraestructura.Comun;
using PIKA.Infraestructura.Comun.Seguridad;
using RepositorioEntidades;

namespace PIKA.Servicio.Seguridad.Interfaces
{
    public interface IServicioEventoAuditoriaActivo : IServicioRepositorioAsync<EventoAuditoriaActivo, string>,
        IServicioAutenticado<EventoAuditoriaActivo>
    {

        public Task<List<EventoAuditoriaActivo>> ObtieneEventosActivos();
        public Task ActualizaEventosActivos(List<EventoAuditoriaActivo> eventos);

    }
}
