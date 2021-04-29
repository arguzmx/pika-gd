using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace PIKA.ServicioBusqueda.Contenido
{
    public static class Extensiones
    {

        public static void LogS(this object o)
        {
            Console.WriteLine(o.ToS());
        }

        public static string ToS(this object o)
        {
            return System.Text.Json.JsonSerializer.Serialize(o);
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
