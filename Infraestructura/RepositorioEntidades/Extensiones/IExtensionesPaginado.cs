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
        public static async Task<IPaginado<T>> PaginadoAsync<T>(this IQueryable<T> origen, int indice, int tamano,
            int desde = 0, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (desde > indice) throw new ArgumentException($"From: {desde} > Index: {indice}, must From <= Index");

            var count = await origen.CountAsync(cancellationToken).ConfigureAwait(false);
            var items = await origen.Skip((indice - desde) * tamano)
                .Take(indice).ToListAsync(cancellationToken).ConfigureAwait(false);

            var list = new Paginado<T>
            {
                Indice = indice,
                Tamano = tamano,
                Desde = desde,
                Conteo = count,
                Elementos = items,
                Paginas = (int)Math.Ceiling(count / (double)tamano)
            };

            return list;
        }

    }
}

