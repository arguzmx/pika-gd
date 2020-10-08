using LazyCache;
using Microsoft.AspNetCore.Server.IIS.Core;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PIKA.Infraestructura.Comun;
using PIKA.Infraestructura.Comun.Seguridad;
using PIKA.Servicio.Organizacion.Interfaces;
using PIKA.Servicio.Seguridad.Interfaces;
using PIKA.Servicio.Usuarios;
using Serilog.Sinks.File;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace PIKA.GD.API.Servicios
{
    public class ServicioTokenSeguridad : IServicioTokenSeguridad
    {
        private const string CACHE_P_USUARIOS = "pu";
        private const string CACHE_P_ROLES = "pr";
        private const string CACHE_ROLES_USUARIO = "ru";

        private readonly ConfiguracionServidor Config;
        private readonly IServicioPerfilUsuario ServicioPerfilUsuario;
        private readonly ILogger<ServicioTokenSeguridad> Logger;
        private readonly IServicioUsuariosRol ServicioUsuariosRol;
        private readonly IServicioSeguridadAplicaciones ServicioSeguridadAplicaciones;
        private readonly IAppCache cache;
        public ServicioTokenSeguridad(
            IAppCache cache,
            IServicioPerfilUsuario ServicioPerfilUsuario,
            IServicioSeguridadAplicaciones ServicioSeguridadAplicaciones,
            IServicioUsuariosRol ServicioUsuariosRol,
            IOptions<ConfiguracionServidor> Config,
            ILogger<ServicioTokenSeguridad> Logger)
        {
            this.ServicioPerfilUsuario = ServicioPerfilUsuario;
            this.ServicioSeguridadAplicaciones = ServicioSeguridadAplicaciones;
            this.ServicioUsuariosRol = ServicioUsuariosRol;
            this.Config = Config.Value;
            this.Logger = Logger;
            this.cache = cache;
        }




        private string ObtieneClaveCachePermisosUsuario(string uid)
        {
            return $"{CACHE_P_USUARIOS}-{uid}";
        }

        private string ObtieneClaveCacheRolesUsuario(string uid)
        {
            return $"{CACHE_ROLES_USUARIO}-{uid}";
        }

        private string ObtieneClaveCachePermisosRol(string rid)
        {
            return $"{CACHE_P_ROLES}-{rid}";
        }

        public async Task<DefinicionSeguridadUsuario> ObtenerSeguridadUsuario(string UserId, string DomainId)
        {

            Logger.LogError(UserId + "---------");
            string key = ObtieneClaveCachePermisosUsuario(UserId);
            // Obtiene la entrada en cache de los permisos del usuario

            DefinicionSeguridadUsuario cacheUcuario = null;

            List<PermisoAplicacion> permisos = new List<PermisoAplicacion>();
            
            if (Config.seguridad_almacenar_cache) cacheUcuario = await cache.GetAsync<DefinicionSeguridadUsuario>(key).ConfigureAwait(false);

            // NO hay permisos en el cache
            if (cacheUcuario == null)
            {
                List <PermisoAplicacion> permisosU = await ObtienePermisosUsuario(DomainId, UserId).ConfigureAwait(false);
                if (permisosU != null) permisos.AddRange(permisosU);

                // Obtiene los roles del usuario y sus permisos
                string keyroles = ObtieneClaveCacheRolesUsuario(UserId);
                List<string> roles = null;
                if (Config.seguridad_almacenar_cache)  roles = await cache.GetAsync<List<string>>(keyroles).ConfigureAwait(false);

                // Los roles no estan en el cache;
                if (roles == null)
                {
                    
                    // Instenta obtenerlos del repositorio
                    roles = await this.ServicioUsuariosRol.IdentificadoresRolesUsuario(UserId).ConfigureAwait(false);

                    if (Config.seguridad_almacenar_cache && (roles != null ))
                    {
                        cache.Add<List<string>>(keyroles, roles, TimeSpan.FromMinutes(Config.seguridad_cache_segundos));
                    }
                }

                if (roles != null)
                {
                    // Revisa los permisos para cada rol
                    foreach (var r in roles)
                    {
                       // Verifica si está en cache
                       string rolkey = this.ObtieneClaveCachePermisosRol(r);
                        List<PermisoAplicacion> prol = null;
                        if (Config.seguridad_almacenar_cache) prol = await cache.GetAsync<List<PermisoAplicacion>>(rolkey).ConfigureAwait(false);
                        
                        if (prol == null)
                        {
                            prol = await ObtienePermisosRol(DomainId, r).ConfigureAwait(false);
                            Logger.LogError($"{r}, {prol.Count}");
                            if (Config.seguridad_almacenar_cache && (prol != null ))
                            {
                                cache.Add<List<PermisoAplicacion>>(rolkey, prol, TimeSpan.FromMinutes(Config.seguridad_cache_segundos));
                            }
                        }

                        if (prol != null) permisos.AddRange(prol);
                    }
                }

                bool esAdmin = await ServicioPerfilUsuario.EsAdmin(UserId, DomainId).ConfigureAwait(false);
                cacheUcuario = new DefinicionSeguridadUsuario()
                {
                    UsuarioId = UserId,
                    EsAdmin = esAdmin,
                    Permisos = permisos, 
                    DominioId = DomainId, 
                    OUId = ""
                };

                if (Config.seguridad_almacenar_cache && (permisos.Count > 0))
                {
                    cache.Add<DefinicionSeguridadUsuario>(key, cacheUcuario, TimeSpan.FromMinutes(Config.seguridad_cache_segundos));
                }
            }

            Logger.LogError($"{cacheUcuario.Permisos.Count}");
            return cacheUcuario;

        }

        private async Task<List<PermisoAplicacion>> ObtienePermisosUsuario(string dominioid, string uid)
        {
            var p = await this.ServicioSeguridadAplicaciones
                .ObtienePermisosAsync(PermisoAplicacion.TIPO_USUARIO, uid, dominioid).ConfigureAwait(false);

            return p?.ToList();
        }

        private async Task<List<PermisoAplicacion>> ObtienePermisosRol(string dominioid, string rid)
        {
            var p = await this.ServicioSeguridadAplicaciones
                .ObtienePermisosAsync(PermisoAplicacion.TIPO_ROL, rid, dominioid).ConfigureAwait(false);

            return p?.ToList();
        }




    }
}
