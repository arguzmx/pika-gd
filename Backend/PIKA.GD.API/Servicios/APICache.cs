using LazyCache;
using PIKA.Infraestructura.Comun;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PIKA.GD.API
{
    public class APICache<T> : IAPICache<T>
    {
        private readonly object CacheLock = new object();
        private List<string> IdentificadoresRegistrados;
        
        private readonly IAppCache cache;
        public APICache(IAppCache cache) {
            this.cache = cache;
            IdentificadoresRegistrados = new List<string>();
        }

        private string Clave(string id, string modulo)
        {
            Type t = typeof(T);
            return t.FullName + modulo + id;
        }

        public async Task Elimina(string id, string modulo)
        {
            string clave = Clave(id, modulo);
            lock (CacheLock)
            {
                cache.Remove(clave);
                IdentificadoresRegistrados.Remove(clave);
            }
            await Task.Delay(1).ConfigureAwait(false);
            
        }

        public async Task EliminaSiContiene(string id)
        {
            lock (CacheLock)
            {
                List<string> temp = IdentificadoresRegistrados.Where(
                    x => x.Contains(id, StringComparison.InvariantCultureIgnoreCase)).ToList();
                foreach (string item in temp)
                {
                    cache.Remove(item);
                    IdentificadoresRegistrados.Remove(item);
                }
            }
            await Task.Delay(1).ConfigureAwait(false);
        }

        public async Task Inserta(string id, string modulo, T entidad, TimeSpan expira)
        {
            string clave = Clave(id, modulo);
            lock (CacheLock)
            {
                // Si la clave ya existe
                if(IdentificadoresRegistrados.IndexOf(clave)>0)
                {
                    //Es eliminada
                    cache.Remove(clave);
                } else
                {
                    //Se añad a  la lista de claves
                    IdentificadoresRegistrados.Add(clave);
                }

                //Se inserta el objeto
                cache.Add<T>(clave, entidad, expira);

            }
            await Task.Delay(1).ConfigureAwait(false);
        }

        public async Task<T> Obtiene(string id, string modulo)
        {
            T resultado = await cache.GetAsync<T>(Clave(id, modulo)).ConfigureAwait(false);
            return resultado;
        }

    }
}
