using PIKA.Infraestructura.Comun;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PIKA.Servicio.Usuarios
{
    public interface IServicioPerfilUsuario
    {
        Task<List<DominioActivo>> Dominios(string UsuarioId);

        Task<bool> EsAdmin(string UsuarioId, string DomainId);

        Task<List<string>> ObtieneRoles(string UsuarioId);
    }
}
