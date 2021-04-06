using System;
using System.Collections.Generic;
using System.Text;
using PIKA.Modelo.Contenido;

namespace PIKA.Servicio.Contenido
{
    public static partial class Extensiones
    {
        public static Elemento Copia(this Elemento d)
        {
            if (d == null) return null;
            return new Elemento()
            {
                Id = d.Id,
                Nombre = d.Nombre,
                CreadorId = d.CreadorId,
                Eliminada = d.Eliminada,
                FechaCreacion = d.FechaCreacion,
                PuntoMontajeId = d.PuntoMontajeId,
                VolumenId = d.VolumenId,
                CarpetaId = d.CarpetaId,
                PermisoId = d.PermisoId,
                Versionado = d.Versionado,
                VersionId = d.VersionId
            };
        }
    }
}
