﻿using LazyCache;
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
using System.Text.Json;
using System.Threading.Tasks;

namespace PIKA.GD.API.Servicios
{
    public class ServicioTokenSeguridad : IServicioTokenSeguridad
    {
        private const string CACHE_P_USUARIOS = "pu";
        private const string CACHE_P_ROLES = "pr";
        private const string CACHE_ROLES_USUARIO = "ru";
        private const string CACHE_USUARIO_API = "uapi";
        private const string CACHE_EVENTOS_AUDITABLES = "eaudi";

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

        private string ObtieneClaveCacheUsuarioAPI(string uid)
        {
            return $"{CACHE_USUARIO_API}-{uid}";
        }

        private string ObtieneClaveEventosAuditables(string dominioId, string OUid)
        {
            return $"{CACHE_EVENTOS_AUDITABLES}-{dominioId}-{OUid}";
        }

        public async Task EventosAuditablesSet(string dominioId, string OUid, List<EventoAuditoriaActivo> eventos)
        {
            string key = ObtieneClaveEventosAuditables(dominioId, OUid);
            var cacheEventos = await cache.GetAsync<EventoAuditoriaActivo>(key).ConfigureAwait(false);
            if (cacheEventos != null)
            {
                cache.Remove(key);
            }
            cache.Add(key, eventos, DateTimeOffset.UtcNow.AddSeconds(Config.seguridad_cache_segundos));
        }

        public async Task<List<EventoAuditoriaActivo>> EventosAuditablesGet(string dominioId, string OUid)
        {
            string key = ObtieneClaveEventosAuditables(dominioId, OUid);
            return await cache.GetAsync<List<EventoAuditoriaActivo>>(key).ConfigureAwait(false);

        }

        public async Task DatosUsuarioSet(UsuarioAPI usuario)
        {
            string key = ObtieneClaveCacheUsuarioAPI(usuario.Id);
            var cacheUsuario = await cache.GetAsync<UsuarioAPI>(key).ConfigureAwait(false);
            if(cacheUsuario != null)
            {
                cache.Remove(key);
            }
            cache.Add<UsuarioAPI>(key, usuario, DateTimeOffset.UtcNow.AddSeconds(Config.seguridad_cache_segundos));
        }

        public async Task<UsuarioAPI> DatosUsuarioGet(string Id)
        {
            string key = ObtieneClaveCacheUsuarioAPI(Id);
            return await cache.GetAsync<UsuarioAPI>(key).ConfigureAwait(false);
        }


        public async Task<DefinicionSeguridadUsuario> ObtenerSeguridadUsuario(string UserId, string DomainId)
        {

            string key = ObtieneClaveCachePermisosUsuario(UserId);
            // Obtiene la entrada en cache de los permisos del usuario

            DefinicionSeguridadUsuario cacheUcuario = null;

            List<PermisoAplicacion> permisos = new List<PermisoAplicacion>();
            
            if (Config.seguridad_almacenar_cache) cacheUcuario = await cache.GetAsync<DefinicionSeguridadUsuario>(key).ConfigureAwait(false);

            // NO hay permisos en el cache
            if (cacheUcuario == null)
            {
                List<PermisoAplicacion> permisosU = await ObtienePermisosUsuario(DomainId, UserId).ConfigureAwait(false);


                if (permisosU != null) permisos.AddRange(permisosU);

                // Obtiene los roles del usuario y sus permisos
                string keyroles = ObtieneClaveCacheRolesUsuario(UserId);
                List<string> roles = await cache.GetAsync<List<string>>(keyroles).ConfigureAwait(false);
                
                // Los roles no estan en el cache;
                if (roles == null)
                {

                    // Instenta obtenerlos del repositorio
                    roles = await this.ServicioUsuariosRol.IdentificadoresRolesUsuario(UserId).ConfigureAwait(false);

                    if (Config.seguridad_almacenar_cache && (roles != null))
                    {
                        cache.Add<List<string>>(keyroles, roles, DateTimeOffset.UtcNow.AddSeconds(Config.seguridad_cache_segundos));
                    }
                }

                if (roles != null)
                {

                    // Revisa los permisos para cada rol
                    foreach (var r in roles)
                    {

                        // Logger.LogError(r + " RRR-------");
                        // Verifica si está en cache
                        string rolkey = this.ObtieneClaveCachePermisosRol(r);
                        List<PermisoAplicacion> prol = null;
                        if (Config.seguridad_almacenar_cache) prol = await cache.GetAsync<List<PermisoAplicacion>>(rolkey).ConfigureAwait(false);

                        if (prol == null)
                        {
                            prol = await ObtienePermisosRol(DomainId, r).ConfigureAwait(false);
                            // Logger.LogError($"{r}, {prol.Count}");
                            if (Config.seguridad_almacenar_cache && (prol != null))
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
                    Logger.LogDebug($"Cache permisos usuario {UserId }");
                    cache.Add<DefinicionSeguridadUsuario>(key, cacheUcuario, DateTimeOffset.UtcNow.AddSeconds(Config.seguridad_cache_segundos));
                }
            }

           // Logger.LogError(JsonSerializer.Serialize(permisos, new JsonSerializerOptions() { WriteIndented = true }));

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

        public async Task<PermisoAplicacion> PermisosModuloId(string UserId, string DomainId, string ModuloId)
        {
            var acl = await  this.ObtenerSeguridadUsuario(UserId, DomainId);
            var permiso = acl.Permisos.Where(x => x.ModuloId == ModuloId && x.DominioId == DomainId ).FirstOrDefault();
            return permiso;
        }
    }
}
