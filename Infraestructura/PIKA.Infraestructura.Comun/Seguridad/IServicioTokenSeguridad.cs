using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PIKA.Infraestructura.Comun.Seguridad
{

    //SecurityTokenService
    public interface IServicioTokenSeguridad 
    {

        Task<DefinicionSeguridadUsuario> ObtenerSeguridadUsuario(string UserId, string DomainId);
        Task DatosUsuarioSet(UsuarioAPI Usuario);

        Task<UsuarioAPI> DatosUsuarioGet(string Id);
    }
}
