using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RepositorioEntidades
{


    public static class IExtensionesPaginado
    {

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="origen">consulta origen</param>
        /// <param name="indice">número de página en base cero</param>
        /// <param name="tamano">tamaño de la página</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public static async Task<IPaginado<T>> PaginadoAsync<T>(this IQueryable<T> origen, int indice, int tamano,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            
            var count = await origen.CountAsync(cancellationToken).ConfigureAwait(false);
            int desde = (indice * tamano);

            var items = await origen.Skip(desde)
                .Take(tamano).ToListAsync(cancellationToken).ConfigureAwait(false);
            
            var list = new Paginado<T>
            {
                Indice = indice,
                Tamano = tamano,
                Desde = desde,
                ConteoTotal = count,
                //ConteoFiltrado
                Elementos = items,
                Paginas = (int)Math.Ceiling(count / (double)tamano)
            };

            return list;
        }

    }
}

