using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using PIKA.Infraestructura.Comun;
using PIKA.Infraestructura.Comun.Interfaces;
using PIKA.Infraestructura.Comun.Seguridad;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Claims;

namespace PIKA.GD.API.Filters
{


    /// <summary>
    /// Aplica seguridad de acceso para el controlador en base un ID de aplciación y de módulo
    /// 
    /// En el caso de que los IDS no se encuentren utiliza reflección sobre la interfaz IMetadatProvider
    /// Para atrata de ubicar en el arreglo estático ServicioAplicacion.ModulosAdministrados 
    /// una coincidencia para el tipo genérico asociado
    /// 
    /// La lista de ServicioAplicacion.ModulosAdministrados  se obtiene por refleccion en el arranque 
    /// para los servicios que implementan IInformacionAplicacion
    /// 
    /// </summary>
    public class AsyncIdentityFilter : IAsyncActionFilter
    {
        
        private readonly ILogger<AsyncACLActionFilter> Logger;
        private readonly ConfiguracionServidor Config;

        public AsyncIdentityFilter(IOptions<ConfiguracionServidor> Config, ILogger<AsyncACLActionFilter> Logger)
        {
            this.Config = Config.Value;
            this.Logger = Logger;

        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            
            StringValues values;
            string DomainId = "";
            string UserId = "";
            string UOid = "";
         


            var identity = context.HttpContext.User.Identity as ClaimsIdentity;
            var valor = identity.Claims.Where(x => x.Type == JwtRegisteredClaimNames.Sub).FirstOrDefault();
            if (valor != null) UserId = valor.Value;


            if (string.IsNullOrEmpty(UserId))
            {
                context.Result = new UnauthorizedResult();
                return;
            }


            if (context.HttpContext.Request.Headers.TryGetValue(Config.header_dominio, out values)) DomainId = values[0];
            if (context.HttpContext.Request.Headers.TryGetValue(Config.header_tenantid, out values)) UOid = values[0];


            if (string.IsNullOrEmpty(UserId) 
                || string.IsNullOrEmpty(DomainId)
                || string.IsNullOrEmpty(UOid))
            {
                context.Result = new UnauthorizedResult();
                return;
            }


            ((ACLController)context.Controller).UsuarioId = UserId;
            ((ACLController)context.Controller).TenantId = UOid;
            ((ACLController)context.Controller).DominioId = DomainId;

            var result = await next().ConfigureAwait(false);

        }
    }
}
