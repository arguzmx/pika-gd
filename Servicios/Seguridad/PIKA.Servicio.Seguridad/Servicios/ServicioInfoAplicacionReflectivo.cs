using LazyCache;
using Microsoft.Extensions.Logging;
using PIKA.Infraestructura.Comun;
using PIKA.Servicio.Seguridad.Interfaces;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace PIKA.Servicio.Seguridad.Servicios
{
    public class ServicioInfoAplicacionReflectivo: IServicioInfoAplicacion
    {
        private readonly ILogger<ServicioInfoAplicacionReflectivo> logger;
        private readonly IAppCache cache;
        private static TimeSpan cacheExpiry = new TimeSpan(0, 30, 0);
        private const string APP_CACHE_KEYS = "cache-aplicaciones";


        public ServicioInfoAplicacionReflectivo(ILogger<ServicioInfoAplicacionReflectivo> logger,
              IAppCache cache)
        {
            this.logger = logger;
            this.cache = cache;
        }

        public async Task<List<Aplicacion>> ObtieneAplicaciones(string AppPath)
        {

            return await cache.GetOrAddAsync(APP_CACHE_KEYS, async () =>
            {
                return await this.ObtieneAplicacionesEnsamblados(AppPath);
            }, cacheExpiry);
           
       }

        private async Task<List<Aplicacion>> ObtieneAplicacionesEnsamblados(string AppPath)
        {
            List<Aplicacion> l = new List<Aplicacion>();
            var assemblies = Directory.GetFiles(AppPath, "*.dll", new EnumerationOptions() { RecurseSubdirectories = true });
            logger.LogWarning(AppPath);
            foreach (var item in assemblies)
            {
                try
                {
                    var assembly = Assembly.LoadFile(item);
                    var Tipos = assembly.GetTypes()
                            .Where(t =>
                            !t.IsAbstract &&
                            typeof(IInformacionAplicacion).IsAssignableFrom(t))
                            .ToArray();

                    foreach (var t in Tipos)
                    {
                        logger.LogInformation(t.FullName);
                       TipoAdministradorModulo s = new TipoAdministradorModulo();
                        var instancia = assembly.CreateInstance(t.FullName);
                        Aplicacion tmp = ((IInformacionAplicacion)instancia).Info().Copia();
                        Aplicacion existente = l.Where(x => x.Id == tmp.Id).SingleOrDefault();
                        if(existente == null)
                        {
                            l.Add(tmp.Copia());
                        } else
                        {
                            foreach(var m in tmp.Modulos)
                            {
                                if ( existente.Modulos.Where(x=>x.Id == m.Id).Count() == 0)
                                {
                                    existente.Modulos.Add(m);
                                }
                            }

                            foreach (var trad in tmp.Traducciones)
                            {
                                if (existente.Traducciones.Where(x => x.Id == trad.Id).Count() == 0)
                                {
                                    existente.Traducciones.Add(trad);
                                }
                            }
                        }
                    }
                }
                catch (Exception)
                {

                }
            }

            await Task.Delay(1);

            foreach(var a in l)
            {
                logger.LogInformation(a.Nombre);
            }
            return l;
        }

    }
}
