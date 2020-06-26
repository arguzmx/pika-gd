using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using PIKA.Infraestructura.Comun;
using PIKA.Modelo.Seguridad;
using RepositorioEntidades;

namespace PIKA.Servicio.Seguridad.Interfaces
{
    public interface IServicioUsuarios : IServicioRepositorioAsync<PropiedadesUsuario, string>
    {
        Task<ICollection<string>> Inactivar(string[] ids);
        Task<ICollection<string>> Activar(string[] ids);
    }
}
