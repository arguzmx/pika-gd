using PIKA.Modelo.Organizacion;
using RepositorioEntidades;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PIKA.Servicio.Organizacion.Interfaces
{
    public interface IServicioUsuariosRol : IServicioRepositorioAsync<UsuariosRol,string>
    {
        Task<int> PostIds(string rolid, string[] ids);
        Task<ICollection<string>> DeleteIds(string rolid, string[] ids);
    }
}
