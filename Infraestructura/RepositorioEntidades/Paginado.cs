using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RepositorioEntidades
{
    public class Paginado<T> : IPaginado<T>
    {
        public Paginado(IEnumerable<T> source, int Indice, int Tamano, int Desde)
        {
            var enumerable = source as T[] ?? source.ToArray();

            if (Desde > Indice)
                throw new ArgumentException($"IndiceDesde: {Desde} > pageIndice: {Indice}, must IndiceDesde <= pageIndice");

            if (source is IQueryable<T> querable)
            {
                this.Indice = Indice;
                this.Tamano = Tamano;
                this.Desde = Desde;
                this.ConteoFiltrado = querable.Count();
                Paginas = (int)Math.Ceiling(ConteoFiltrado / (double)Tamano);

                Elementos = querable.Skip((Indice - Desde) * Tamano).Take(Tamano).ToList();
            }
            else
            {
                this.Indice = Indice;
                this.Tamano = Tamano;
                this.Desde = Desde;

                ConteoFiltrado = enumerable.Count();
                Paginas = (int)Math.Ceiling(ConteoFiltrado / (double)Tamano);

                Elementos = enumerable.Skip((Indice - Desde) * Tamano).Take(Tamano).ToList();
            }
        }

        public Paginado()
        {
            Elementos = new T[0];
        }

        public int Desde { get; set; }
        public int Indice { get; set; }
        public int Tamano { get; set; }
        public int Paginas { get; set; }
        public IList<T> Elementos { get; set; }
        public bool TienePrevio => Indice - Desde > 0;
        public bool TieneSiguiente => Indice - Desde + 1 < Paginas;

        public int ConteoTotal { get; set; }

        public int ConteoFiltrado { get; set; }

        public PropiedadesExtendidas PropiedadesExtendidas { get; set; }

    }


    public class Paginado<TSource, TResult> : IPaginado<TResult>
    {
        public Paginado(IEnumerable<TSource> source, Func<IEnumerable<TSource>, IEnumerable<TResult>> converter,
            int Indice, int Tamano, int Desde)
        {
            var enumerable = source as TSource[] ?? source.ToArray();

            if (Desde > Indice) throw new ArgumentException($"Desde: {Desde} > Indice: {Indice}, must Desde <= Indice");

            if (source is IQueryable<TSource> queryable)
            {
                this.Indice = Indice;
                this.Tamano = Tamano;
                this.Desde = Desde;
                this.ConteoFiltrado = enumerable.Count();
                this.Paginas = (int)Math.Ceiling(this.ConteoFiltrado / (double)Tamano);

                var items = queryable.Skip((Indice - Desde) * Tamano).Take(Tamano).ToArray();

                Elementos = new List<TResult>(converter(items));
            }
            else
            {
                this.Indice = Indice;
                this.Tamano = Tamano;
                this.Desde = Desde;
                this.ConteoFiltrado = enumerable.Count();
                this.Paginas = (int)Math.Ceiling(this.ConteoFiltrado / (double)Tamano);

                var items = enumerable.Skip((Indice - Desde) * Tamano).Take(Tamano).ToArray();

                Elementos = new List<TResult>(converter(items));
            }
        }


        public Paginado(IPaginado<TSource> source, Func<IEnumerable<TSource>, IEnumerable<TResult>> converter)
        {
            Indice = source.Indice;
            Tamano = source.Tamano;
            Desde = source.Desde;
            ConteoFiltrado = source.ConteoFiltrado;
            ConteoTotal = source.ConteoTotal;
            Paginas = source.Paginas;

            Elementos = new List<TResult>(converter(source.Elementos));
        }

        public int Indice { get; }

        public int Tamano { get; }

        public int Paginas { get; }

        public int Desde { get; }

        public IList<TResult> Elementos { get; }

        public bool TienePrevio => Indice - Desde > 0;

        public bool TieneSiguiente => Indice - Desde + 1 < Paginas;

        public int ConteoTotal { get; set; }

        public int ConteoFiltrado { get; set; }
    }

    public static class Paginado
    {
        public static IPaginado<T> Empty<T>()
        {
            return new Paginado<T>();
        }

        public static IPaginado<TResult> Desde<TResult, TSource>(IPaginado<TSource> source,
            Func<IEnumerable<TSource>, IEnumerable<TResult>> converter)
        {
            return new Paginado<TSource, TResult>(source, converter);
        }
    }
}
