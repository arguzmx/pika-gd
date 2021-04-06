using Microsoft.EntityFrameworkCore;
using PIKA.Modelo.Contenido;
using RepositorioEntidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace PIKA.Servicio.Contenido.Servicios.Busqueda
{
    public static class Extenders
    {
        public static async Task<IPaginado<string>> PaginadoElementosAsync(this IQueryable<Elemento> origen, int indice, int tamano,
          CancellationToken cancellationToken = default(CancellationToken))
        {
            int desde = (indice * tamano);
            List<Elemento> items = await origen.Skip(desde)
                .Take(tamano).ToListAsync<Elemento>(cancellationToken).ConfigureAwait(false);

            var list = new Paginado<string>()
            {
                Indice = indice,
                Tamano = tamano,
                Desde = desde,
                ConteoTotal = 0,
                Elementos = items.Select(x=>x.Id).ToList(),
                Paginas = 0
            };

            return list;
        }


        public static async Task<IPaginado<string>> PaginadoCarpetasAsync(this IQueryable<Carpeta> origen, int indice, int tamano,
          CancellationToken cancellationToken = default(CancellationToken))
        {
            int desde = (indice * tamano);
            List<Carpeta> items = await origen.Skip(desde)
                .Take(tamano).ToListAsync<Carpeta>(cancellationToken).ConfigureAwait(false);

            var list = new Paginado<string>()
            {
                Indice = indice,
                Tamano = tamano,
                Desde = desde,
                ConteoTotal = 0,
                Elementos = items.Select(x => x.Id).ToList(),
                Paginas = 0
            };

            return list;
        }
    }
}
