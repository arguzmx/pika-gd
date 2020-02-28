using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PIKA.Infraestructura.Comun.Seguridad
{

    //SecurityTokenService
    public interface IServicioTokenSeguridad 
    {

        Task<SecurityToken> ObtenerToken(string UserId, string DomainId, string AppId);

        Task<List<AppDomainGrant>> ObtenerGrants(string UserId, string DomainId, string AppId);

    }
}
