using LazyCache;
using PIKA.Modelo.Metadatos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PIKA.GD.API.Servicios
{
    public class APICache<T> : IAPICache<T>
    {

        public const string PLANTILLA = "plantilla";

        private readonly IAppCache cache;
        public APICache(IAppCache cache) {
            this.cache = cache;
        }

        private string Clave(string id)
        {
            Type t = typeof(T);
            return t.FullName + id;
        }

        public async Task Elimina(string id)
        {
            await Task.Delay(1).ConfigureAwait(true);
            cache.Remove(Clave(id));
        }

        public async Task Inserta(string id, T entidad, TimeSpan expira)
        {
            await Task.Delay(1).ConfigureAwait(true);
            cache.Add<T>(Clave(id), entidad, expira);
            
        }

        public async Task<T> Obtiene(string id)
        {
            T resultado = await cache.GetAsync<T>(Clave(id)).ConfigureAwait(true);
            return resultado;
        }

    }
}
