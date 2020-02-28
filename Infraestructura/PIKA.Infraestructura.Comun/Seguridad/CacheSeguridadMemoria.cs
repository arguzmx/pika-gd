using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PIKA.Infraestructura.Comun.Seguridad
{
    public class CacheSeguridadMemoria : ICacheSeguridad
    {
        private readonly ConfiguracionServidor Config;
        private readonly CacheMemoria Cache;
        private readonly IServicioTokenSeguridad SecurityTokenService;
        private readonly ILogger<CacheSeguridadMemoria> Logger;

        public CacheSeguridadMemoria(CacheMemoria Cache,
            IServicioTokenSeguridad SecurityTokenService, 
            IOptions<ConfiguracionServidor> Config,
            ILogger<CacheSeguridadMemoria> Logger)
        {
            this.Cache = Cache;
            this.SecurityTokenService = SecurityTokenService;
            this.Config = Config.Value;
            this.Logger = Logger;
        }


        /// <summary>
        /// DEtermina si el usuario tiene el derecho de acceso al módulo seleccionado
        /// </summary>
        /// <param name="UserId"></param>
        /// <param name="DomainId"></param>
        /// <param name="AppId"></param>
        /// <param name="ModuleId"></param>
        /// <param name="Method">Médodo HTTP</param>
        /// <returns></returns>
        public async Task<bool> AllowMethod(string UserId, string DomainId, string AppId, string ModuleId, string Method)
        {

            //Return true

            SecurityToken Token = await GetSecurityToken(UserId, DomainId, AppId);

            if (Token == null)
            {
                return false;
            }

            bool allow = false;

            List<AppDomainGrant> Grants = SecurityToken.DeserializeGrants(Token.Content);

            if (Grants != null)
            {

                // calcula los permisos totales para el módulo
                ulong Mask = 0;
                foreach (var item in Grants.Where(x => x.ModuleId == ModuleId).ToList())
                {
                    Mask = Mask | item.GrantMask;
                }

                ///Verifica que haya un amáscara válida
                if (Mask > 0)
                {
                    PermisoAplicacion p = new PermisoAplicacion() { Mascara = Mask };

                    //verifica que el permiso de negado explícito no exista
                    if (!p.NegarAcceso)
                    {
                        switch (Method.ToUpper())
                        {
                            case "DELETE":
                                allow = p.Eliminar;
                                break;

                            case "GET":
                                allow = p.Leer;
                                break;

                            case "PUT":
                                allow = p.Escribir;
                                break;

                            case "POST":
                                allow = p.Escribir;
                                break;

                            case "HEAD":
                                allow = p.Leer;
                                break;

                            default:
                                throw new Exception("Invalid HTTP Verb");
                        }
                    }

                }

            }



            return allow;

        }



        /// <summary>
        /// Otiene los permisos de acceso para una aplicación en un dominio
        /// </summary>
        /// <param name="UserId"></param>
        /// <param name="DomainId"></param>
        /// <param name="AppId"></param>
        /// <returns></returns>
        public async Task<SecurityToken> GetSecurityToken(string UserId, string DomainId, string AppId)
        {
            string CacheId = $"{UserId }.{DomainId}.{AppId}";



            SecurityToken Token = null;

            if (!Cache.Cache.TryGetValue<SecurityToken>(CacheId, out Token))
            {

                Token = await SecurityTokenService.ObtenerToken(UserId, DomainId, AppId);

                if (Token != null)
                {
                    var cacheEntryOptions = new MemoryCacheEntryOptions().SetSize(1)
                    .SetSlidingExpiration(TimeSpan.FromSeconds(Config.cache_seguridad_segundos));
                    Cache.Cache.Set(CacheId, Token, cacheEntryOptions);
                }

            }

            return Token;
        }
    }
}
