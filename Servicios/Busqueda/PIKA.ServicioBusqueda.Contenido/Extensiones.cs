using RepositorioEntidades;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace PIKA.ServicioBusqueda.Contenido
{
    public static class Extensiones
    {
        public static void LogS(this string o)
        {
            Console.WriteLine(o);
        }
        public static void LogS(this object o)
        {
            Console.WriteLine(o.ToS() + "\r\n");
        }


        public static bool OrdenamientoMetadatos(this BusquedaContenido c)
        {
            return Guid.TryParse(c.ord_columna, out _);
        }


        public static string ToS(this object o)
        {
            return System.Text.Json.JsonSerializer.Serialize(o, new System.Text.Json.JsonSerializerOptions() {  WriteIndented = true});
        }

        public static IEnumerable<T> Select<T>(this IDataReader reader,
                                       Func<IDataReader, T> projection)
        {
            while (reader.Read())
            {
                yield return projection(reader);
            }
        }

        public static bool BuscarPropiedades(this BusquedaContenido b)
        {
            return (b.Elementos.Any(e => e.Tag == Constantes.PROPIEDEDES));
        }

        public static bool BuscarMetadatos(this BusquedaContenido b)
        {
            return (b.Elementos.Any(e => e.Tag == Constantes.METADATOS));
        }

        public static bool BuscarEnFolder(this BusquedaContenido b)
        {
            return (b.Elementos.Any(e => e.Tag == Constantes.ENFOLDER));
        }

        public static long Conteo(this BusquedaContenido b, string Tipo)
        {
            var elemento = b.Elementos.Where(e => e.Tag == Tipo).FirstOrDefault();
            return elemento==null? 0 : elemento.Conteo; 
        }

        public static Consulta AConsulta(this BusquedaContenido b)
        {
            Consulta el = new Consulta()
            {
                consecutivo = 0,
                indice = b.indice,
                ord_columna = b.ord_columna,
                ord_direccion = b.ord_direccion,
                recalcular_totales = b.recalcular_totales,
                tamano = b.tamano,
                Filtros = new List<FiltroConsulta>()
            };

            return el;
        }


        public static void ActualizaConteo(this BusquedaContenido b, string Tag, long Conteo)
        {
            Busqueda el = b.Elementos.FirstOrDefault(x => x.Tag == Tag);
            if (el != null)
            {
                el.Conteo = Conteo;
            }
        }


        public static void ActualizaIds(this BusquedaContenido b, string Tag, List<string> ids)
        {
            Busqueda el = b.Elementos.FirstOrDefault(x => x.Tag == Tag);
            if (el != null)
            {
                el.Ids = ids;
            }
        }

        public static Busqueda ObtenerBusqueda(this BusquedaContenido b, string Tag)
        {
            return (b.Elementos.FirstOrDefault(e => e.Tag == Tag));
        }

    }
}
