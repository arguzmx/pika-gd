using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Text;

namespace PIKA.Infraestructura.Comun
{
    public class CacheMemoria : IServicioCache
    {

        /// <summary>
        /// Esta variables utilziada por otros miembros 
        /// debe mantenenerse pública
        /// </summary>
        public MemoryCache Cache { get; set; }

        public CacheMemoria() {
            CreaCache();
        }

        public void Eliminar(string Clave)
        {
            this.Cache.Remove(Clave);
        }


        public void Escribir<T>(string Clave, T Contenido, TimeSpan Duracion)
        {
            this.Cache.Set<T>(Clave, Contenido);
        }


        public T Leer<T>(string Clave)
        {
            T EntradaCache;
            if (this.Cache.TryGetValue<T>(Clave, out EntradaCache))
            {

            }
            return EntradaCache;
        }

        public void Limpiar()
        {
            this.Cache.Dispose();
            CreaCache();
        }


        private void CreaCache()
        {
            Cache = new MemoryCache(new MemoryCacheOptions
            {
                SizeLimit = 1024
            });
        }
    }
}
