using System;
using System.Collections.Generic;
using System.Text;
using PIKA.Modelo.Contenido;

namespace PIKA.Servicio.Contenido
{
    public static partial class Extensiones
    {
        public static Carpeta Copia(this Carpeta d)
        {
            if (d == null) return null;
            var o = new Carpeta()
            {
                CarpetaPadreId = d.CarpetaPadreId,
                CreadorId = d.CreadorId,
                Eliminada = d.Eliminada,
                EsRaiz = d.EsRaiz,
                FechaCreacion = d.FechaCreacion,
                Id = d.Id,
                Nombre = d.Nombre,
                PermisoId = d.PermisoId,
                PuntoMontajeId = d.PuntoMontajeId
            };


            if (d.Subcarpetas != null)
            {
                foreach(var c in d.Subcarpetas)
                {
                    o.Subcarpetas.Add(c.Copia());
                }
            }

            return o;
        }
    }
}
