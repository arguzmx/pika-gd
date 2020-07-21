using System;
using System.Collections.Generic;
using System.Text;
using Version = PIKA.Modelo.Contenido.Version;

namespace PIKA.Servicio.Contenido
{
    public static partial class Extensiones
    {
        public static Version Copia(this Version d)
        {
            if (d == null) return null;
            return new Version()
            {
                Id = d.Id,
                Activa = d.Activa,
                ElementoId = d.ElementoId,
                FechaCreacion = d.FechaCreacion,
                FechaActualizacion = d.FechaActualizacion
            };
        }
    }
}
