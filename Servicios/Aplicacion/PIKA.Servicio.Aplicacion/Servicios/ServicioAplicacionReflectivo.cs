using LazyCache;
using Microsoft.Extensions.Logging;
using PIKA.Infraestructura.Comun;
using PIKA.Servicio.AplicacionPlugin.Interfaces;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace PIKA.Servicio.AplicacionPlugin.Servicios
{
    public class ServicioAplicacionReflectivo: IServicioAplicacion
    {
        private readonly ILogger<ServicioAplicacionReflectivo> logger;
        private readonly IAppCache cache;
        private static TimeSpan cacheExpiry = new TimeSpan(0, 30, 0);
        private const string APP_CACHE_KEYS = "cache-aplicaciones";


        public ServicioAplicacionReflectivo(ILogger<ServicioAplicacionReflectivo> logger,
              IAppCache cache)
        {
            this.logger = logger;
            this.cache = cache;
        }

        public async Task<List<Aplicacion>> OntieneAplicaciones(string AppPath)
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
            logger.LogInformation("Obteniendo aplicaciones desde ensamblados");
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
                        Aplicacion tmp = ((IInformacionAplicacion)instancia).Info();
                        l.Add(tmp.Copia());
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
