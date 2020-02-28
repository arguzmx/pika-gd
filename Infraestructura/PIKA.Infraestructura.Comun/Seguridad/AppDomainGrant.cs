using System;
using System.Collections.Generic;
using System.Text;

namespace PIKA.Infraestructura.Comun.Seguridad
{
    public class AppDomainGrant : AppGrant
    {
        public string DomainId { get; set; }

        public AppGrant ToAppGrant()
        {
            return new AppGrant()
            {
                AppId = this.AppId,
                GrantedId = this.GrantedId,
                GrantedType = this.GrantedType,
                GrantMask = this.GrantMask,
                ModuleId = this.ModuleId
            };
        }

    }
}
