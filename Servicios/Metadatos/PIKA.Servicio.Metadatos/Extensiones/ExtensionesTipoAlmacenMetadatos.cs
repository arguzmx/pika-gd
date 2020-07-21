using System;
using System.Collections.Generic;
using System.Text;
using PIKA.Modelo.Metadatos;

namespace PIKA.Servicio.Metadatos
{
    public static partial class Extensiones
    {
        public static TipoAlmacenMetadatos Copia(this TipoAlmacenMetadatos d)
        {
            if (d == null) return null;
            return new TipoAlmacenMetadatos()
            {
                Id = d.Id,
                Nombre = d.Nombre
            };
        }
    }
}
