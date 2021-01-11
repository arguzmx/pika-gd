using LazyCache;
using Microsoft.EntityFrameworkCore;
using PIKA.Modelo.Metadatos;
using PIKA.Servicio.Metadatos.Interfaces;
using System;
using System.Threading.Tasks;

namespace PIKA.GD.API.Servicios.Caches
{
    public static class CacheMetadatos
    {
        private static string ClavePlantilla(string id)
        {
            return $"PL-{id}";
        }

        private static string ClavePlantillaGenerada(string id)
        {
            return $"PLGEN-{id}";
        }

        public async static Task<Plantilla> ObtienePlantillaPorId(string id, IAppCache cache, IServicioPlantilla servicio)
        {
            Plantilla p = await  cache.GetAsync<Plantilla>(ClavePlantilla(id)).ConfigureAwait(false);
            if (p == null)
            {
                p = await servicio.UnicoAsync(x => x.Id == id, null,
                    y => y.Include(z => z.Propiedades).ThenInclude(z => z.TipoDato)
                    .Include(z => z.Propiedades).ThenInclude(z => z.ValidadorNumero)
                    .Include(z => z.Propiedades).ThenInclude(z => z.ValidadorTexto)
                    .Include(z => z.Propiedades).ThenInclude(z => z.ValoresLista)
                ).ConfigureAwait(false);

                foreach(var i in p.Propiedades)
                {
                    if(i.TipoDatoId == TipoDato.tList)
                    {
                        i.ValoresLista = await servicio.ObtenerValores(i.Id).ConfigureAwait(false);
                    }
                }

                if (p != null) cache.Add<Plantilla>(ClavePlantilla(id), p);
            }
            return p;
        }


        public async static Task<bool> PlantillaGenerada(string id, IAppCache cache)
        {
            string p = await cache.GetAsync<string>(ClavePlantillaGenerada(id)).ConfigureAwait(false);
            return !string.IsNullOrEmpty(p);
        }

        public static void EstablecePlantillaGenerada(string id, IAppCache cache)
        {
            cache.Add<string>(ClavePlantillaGenerada(id), "generada");
        }
    }
}
