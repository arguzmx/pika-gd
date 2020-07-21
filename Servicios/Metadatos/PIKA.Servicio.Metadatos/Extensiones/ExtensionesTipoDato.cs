using System;
using System.Collections.Generic;
using System.Text;
using PIKA.Modelo.Metadatos;

namespace PIKA.Servicio.Metadatos
{
    public static partial class Extensiones
    {
        public static TipoDato Copia(this TipoDato d)
        {
            if (d == null) return null;
            return new TipoDato()
            {
                Id = d.Id,
                Nombre = d.Nombre
            };
        }
    }
}
