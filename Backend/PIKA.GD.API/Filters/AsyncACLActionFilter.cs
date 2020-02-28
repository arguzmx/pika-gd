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
using System.Linq;
using System.Threading.Tasks;

namespace PIKA.GD.API.Filters
{
    public class AsyncACLActionFilter : IAsyncActionFilter
    {
        private readonly ICacheSeguridad SecurityCache;
        private readonly ConfiguracionServidor Config;
        private readonly ILogger<AsyncACLActionFilter> Logger;
        private readonly string ModuleId;
        private readonly string AppId;
        private readonly ILocalizadorFiltroACL localizadorFiltro;

        public AsyncACLActionFilter(string AppId, string ModuleId, ILocalizadorFiltroACL localizadorFiltro ,
            ILogger<AsyncACLActionFilter> Logger,
            ICacheSeguridad SecurityCache, IOptions<ConfiguracionServidor> Config, string[] ids)
        {
            this.localizadorFiltro = localizadorFiltro;
            this.SecurityCache = SecurityCache;
            this.ModuleId = ModuleId;
            this.Config = Config.Value;
            this.Logger = Logger;
            this.AppId = AppId;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {

            StringValues values;
            string DomainId = "";
            string UserId = "";
            string OwnerId = "";
            bool IsAPIController = false;
            bool ExcludeACL = false;




            
            IsAPIController =
            (ApiControllerAttribute)Attribute.GetCustomAttribute(context.Controller.GetType(),
            typeof(ApiControllerAttribute)) != null ? true : false;

            ExcludeACL =
            (ExcludedACLControllerAttribute)Attribute.GetCustomAttribute(context.Controller.GetType(),
            typeof(ExcludedACLControllerAttribute)) != null ? true : false;



            if (IsAPIController && !ExcludeACL)
            {

                if (string.IsNullOrEmpty(AppId) || string.IsNullOrEmpty(ModuleId))
                {
                    context.Result = new BadRequestResult();
                    return;
                }

                if (!context.ModelState.IsValid)
                {
                    context.Result = new BadRequestResult();
                    return;
                }


                if (context.HttpContext.Request.Headers.TryGetValue(Config.header_idusuario, out values)) UserId = values[0];

                if (context.HttpContext.Request.Headers.TryGetValue(Config.header_dominio, out values)) DomainId = values[0];

                if (context.HttpContext.Request.Headers.TryGetValue(Config.header_tenantid, out values)) OwnerId = values[0];

                if (string.IsNullOrEmpty(UserId) || string.IsNullOrEmpty(OwnerId) || string.IsNullOrEmpty(AppId))
                {
                    context.Result = new UnauthorizedResult();
                    return;
                }

                bool allow = false;
                if (UserId == OwnerId)
                {
                    allow = true;
                }
                else
                {
                    string Method = context.HttpContext.Request.Method;
                    allow = await SecurityCache.AllowMethod(UserId, DomainId, AppId, ModuleId, Method).ConfigureAwait(false);
                }

                if (allow)
                {
                    ACLController controller = context.Controller as ACLController;
                    controller.UsuarioId = UserId;
                    controller.TenatId = OwnerId;
                    controller.DominioId = DomainId;

                    var result = await next().ConfigureAwait(true);
                }
                else
                {
                    context.Result = new UnauthorizedResult();
                    return;
                }
            }
            else
            {
                var result = await next().ConfigureAwait(true);
            }



        }
    }
}
