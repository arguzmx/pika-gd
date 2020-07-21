using System;
using System.Collections.Generic;
using System.Text;
using PIKA.Modelo.Metadatos;

namespace PIKA.Servicio.Metadatos
{
    public static partial class Extensiones
    {
        public static ValidadorTexto Copia(this ValidadorTexto d)
        {
            if (d == null) return null;
            return new ValidadorTexto()
            {
                Id = d.Id,
                PropiedadId = d.PropiedadId,
                longmax = d.longmax,
                longmin = d.longmin,
                regexp = d.regexp,
                valordefault = d.valordefault
            };
        }
    }
}
