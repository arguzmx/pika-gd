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
    public class AsyncACLActionFilter : IAsyncActionFilter
    {
        private readonly ICacheSeguridad SecurityCache;
        private readonly ConfiguracionServidor Config;
        private readonly ILogger<AsyncACLActionFilter> Logger;
        private  string ModuleId;
        private  string AppId;


        public AsyncACLActionFilter(ILogger<AsyncACLActionFilter> Logger,
            ICacheSeguridad SecurityCache, IOptions<ConfiguracionServidor> Config, 
            string AppId="", string ModuleId="", string[] ids = null )
        {
            this.SecurityCache = SecurityCache;
            this.Config = Config.Value;
            this.Logger = Logger;

            if (ids != null)
            {
                this.AppId = AppId;
                this.ModuleId = ModuleId;
            }

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
                
                //Analiza por reflección de IMetadateProvider si no están presentes los IDs solicutados
                if (string.IsNullOrEmpty(AppId) || string.IsNullOrEmpty(ModuleId))
                {

                   Type  t =LocalizadorEnsamblados.ObtieneTipoMetadata(context.Controller.GetType());
                    foreach (var m in ServicioAplicacion.ModulosAdministrados)
                    {
                        foreach (var ta in m.TiposAdministrados)
                        {
                            if(ta.FullName == t.FullName)
                            {
                                this.AppId = m.AplicacionId;
                                this.ModuleId = m.ModuloId;
                                break;
                            }
                        }

                    }

                    if (string.IsNullOrEmpty(AppId) || string.IsNullOrEmpty(ModuleId))
                    {
                        context.Result = new BadRequestResult();
                        return;
                    }
               }


                //VErifica la validez los datos del modelo
                if (!context.ModelState.IsValid)
                {
                    context.Result = new BadRequestResult();
                    return;
                }

             
                if (context.HttpContext.Request.Headers.TryGetValue(Config.header_idusuario, out values)) UserId = values[0];
                if (context.HttpContext.Request.Headers.TryGetValue(Config.header_dominio, out values)) DomainId = values[0];
                if (context.HttpContext.Request.Headers.TryGetValue(Config.header_tenantid, out values)) OwnerId = values[0];


         

                if (string.IsNullOrEmpty(UserId)  || string.IsNullOrEmpty(AppId))
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

                    ((ACLController)context.Controller).UsuarioId = UserId;
                    ((ACLController)context.Controller).TenatId = OwnerId;
                    ((ACLController)context.Controller).DominioId = DomainId;

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
