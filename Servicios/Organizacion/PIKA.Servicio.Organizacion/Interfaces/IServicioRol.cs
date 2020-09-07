using PIKA.Modelo.Organizacion;
using RepositorioEntidades;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PIKA.Servicio.Organizacion.Interfaces
{
    public interface IServicioRol : IServicioRepositorioAsync<Rol,string>
    {

        Task<ICollection<Rol>> ObtieneRoles(string idDominio);
        Task<ICollection<string>> Vincular(string rolId, string[] ids);
        Task<ICollection<string>> Desvincular(string rolId, string[] ids);

    }
}
