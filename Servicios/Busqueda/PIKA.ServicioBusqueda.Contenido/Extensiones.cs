﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace PIKA.ServicioBusqueda.Contenido
{
    public static class Extensiones
    {

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
            return (b.Elementos.Any(e => e.Tag == Constantes.PROPIEDEDES)) ;
        }

        public static bool BuscarMetadatos(this BusquedaContenido b)
        {
            return (b.Elementos.Any(e => e.Tag == Constantes.METADATOS));
        }

        public static bool BuscarEnFolder(this BusquedaContenido b)
        {
            return (b.Elementos.Any(e => e.Tag == Constantes.ENFOLDER));
        }

        public static void ActualizaConteo(this BusquedaContenido b, string Tag, long Conteo)
        {
            Busqueda el = b.Elementos.FirstOrDefault(x => x.Tag == Tag);
            if (el != null)
            {
                el.Conteo = Conteo;
            }
        }

        public static Busqueda ObtenerBusqueda(this BusquedaContenido b, string Tag)
        {
            return (b.Elementos.FirstOrDefault(e => e.Tag == Tag));
        }

    }
}
