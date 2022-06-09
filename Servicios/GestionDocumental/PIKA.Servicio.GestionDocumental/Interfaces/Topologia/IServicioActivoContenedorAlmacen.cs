using PIKA.Modelo.GestorDocumental;
using RepositorioEntidades;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PIKA.Servicio.GestionDocumental.Interfaces.Topologia
{
    public interface IServicioActivoContenedorAlmacen: IServicioRepositorioAsync<ActivoContenedorAlmacen, string>
    {
        Task<int> PostIds(string rolid, string[] ids);
        Task<ICollection<string>> DeleteIds(string rolid, string[] ids);
        Task<List<string>> IdentificadoresRolesUsuario(string uid);
       
    }
}
