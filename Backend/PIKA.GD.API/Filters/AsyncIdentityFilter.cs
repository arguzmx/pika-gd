using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using PIKA.Infraestructura.Comun;
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
        private readonly ICacheSeguridad SecurityCache;
        private readonly ILogger<AsyncACLActionFilter> Logger;
        private readonly ConfiguracionServidor Config;

        public AsyncIdentityFilter(IOptions<ConfiguracionServidor> Config, ILogger<AsyncACLActionFilter> Logger, ICacheSeguridad SecurityCache)
        {
            this.SecurityCache = SecurityCache;
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

            ((ACLController)context.Controller).Roles = new List<string>();
            ((ACLController)context.Controller).Accesos = new List<string>();
            ((ACLController)context.Controller).AdminGlobal = false;


            identity.Claims.Where(x => x.Type == "http://schemas.microsoft.com/ws/2008/06/identity/claims/role").ToList().ForEach(c =>
            {
                switch (c.Value)
                {
                    case "adminglobal":
                        ((ACLController)context.Controller).AdminGlobal = true;
                        break;

                    default:

                        if (c.Value.IndexOf(":") > 0)
                        {
                            // Acceso a universos
                            ((ACLController)context.Controller).Accesos.Add(c.Value);

                        }
                        else
                        {
                            // rol de usuario
                            ((ACLController)context.Controller).Roles.Add(c.Value);
                        }
                        break;
                }


            });

            ((ACLController)context.Controller).UsuarioId = UserId;
            ((ACLController)context.Controller).TenantId = UOid;
            ((ACLController)context.Controller).DominioId = DomainId;
            var eventos = await SecurityCache.EventosAuditables(DomainId, UOid);

            var u = ObtieneUsuarioAPI(UserId,
                ((ACLController)context.Controller).Roles,
                ((ACLController)context.Controller).AdminGlobal,
                ((ACLController)context.Controller).Accesos);

            u.gmtOffset = context.HttpContext.Request.Headers.Any(x => x.Key == "gmtoffset") ? int.Parse(context.HttpContext.Request.Headers.First(x => x.Key == "gmtoffset").Value) * -1 : 0;
            ((ACLController)context.Controller).usuario = u;

            ContextoRegistroActividad a = new ContextoRegistroActividad()
            {
                DominioId = DomainId,
                UnidadOrgId = UOid,
                DireccionInternet = context.HttpContext.Connection.RemoteIpAddress.ToString(),
                FechaUTC = DateTime.UtcNow,
                IdConexion = context.HttpContext.Connection.Id,
                UsuarioId = UserId
            };

            ((ACLController)context.Controller).contextoRegistro = a;

            ((ACLController)context.Controller).EstableceContextoSeguridad(u, a, eventos);

            var result = await next().ConfigureAwait(false);

        }


        public UsuarioAPI ObtieneUsuarioAPI(string Id, List<string> Roles, bool AdminGlobal, List<string> Accesos)
        {
            UsuarioAPI u = new UsuarioAPI()
            {
                Id = Id,
                Roles = Roles,
                AdminGlobal = AdminGlobal
            };

            Accesos.ForEach(a =>
            {
                List<string> data = a.Split(':').ToList();
                u.Accesos.Add(new Acceso()
                {
                    Admin = data[2] == "admin",
                    Dominio = data[0],
                    OU = data[1]
                });
            });

            return u;
        }
    }
}
