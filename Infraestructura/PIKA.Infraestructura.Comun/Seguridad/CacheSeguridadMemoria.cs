using LazyCache;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PIKA.Infraestructura.Comun.Menus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace PIKA.Infraestructura.Comun.Seguridad
{
    public class CacheSeguridadMemoria : ICacheSeguridad
    {
        private readonly ConfiguracionServidor Config;
        private readonly IServicioTokenSeguridad SecurityTokenService;
        private readonly ILogger<CacheSeguridadMemoria> Logger;
        

        public CacheSeguridadMemoria(
            IServicioTokenSeguridad SecurityTokenService, 
            IOptions<ConfiguracionServidor> Config,
            ILogger<CacheSeguridadMemoria> Logger)
        {
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
            bool allow = false;
            int Mask = 0;

            DefinicionSeguridadUsuario usuario = await SecurityTokenService.ObtenerSeguridadUsuario(UserId, DomainId);

            if ( (usuario == null) || (usuario.Permisos == null)
                || (usuario.Permisos.Where(x => x.AplicacionId == AppId && x.ModuloId == ModuleId && x.NegarAcceso).Count() > 0)
                ) return false;

            Mask = (usuario.EsAdmin) ? int.MaxValue : this.ObtienePermisos(usuario.Permisos, Config.seguridad_minimo_permisos, AppId, ModuleId);

            if (Mask > 0)
            {
                PermisoAplicacion p = new PermisoAplicacion();
                p.EstablacerMascara(Mask);

                //verifica que el permiso de negado explícito no exista
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
                        return false;
                }

            }

            return allow;
        }

        public async Task<UsuarioAPI> DatosUsuarioGet(string Id)
        {
            return await SecurityTokenService.DatosUsuarioGet(Id);
        }

        public async Task DatosUsuarioSet(UsuarioAPI Usuario)
        {
            await  SecurityTokenService.DatosUsuarioSet(Usuario);
        }

        private int ObtienePermisos(List<PermisoAplicacion> Permisos, bool Minimos, string ApppId, string ModuleId)
        {
            int p = 0;

            List<PermisoAplicacion> efectivos = Permisos.Where(x => x.AplicacionId.Equals(ApppId, StringComparison.InvariantCultureIgnoreCase)
            && x.ModuloId.Equals(ModuleId, StringComparison.InvariantCultureIgnoreCase)).ToList();

            if (Minimos && (efectivos.Where(x => x.NegarAcceso == true).Count() > 0)) return 0;

            if (Minimos)
            {
                p = int.MaxValue;
                foreach (var permiso in efectivos)
                {
                    p &= permiso.Mascara;
                }

            } else
            {
                foreach(var permiso in efectivos)
                {
                    p |= permiso.Mascara;
                }
            }

            return p;
        }

    }
}
