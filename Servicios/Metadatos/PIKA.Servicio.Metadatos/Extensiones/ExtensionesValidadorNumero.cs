using System;
using System.Collections.Generic;
using System.Text;
using PIKA.Modelo.Metadatos;

namespace PIKA.Servicio.Metadatos
{
    public static partial class Extensiones
    {
        public static ValidadorNumero Copia(this ValidadorNumero d)
        {
            if (d == null) return null;
            return new ValidadorNumero()
            {
                Id = d.Id,
                PropiedadId = d.PropiedadId,
                valordefault = d.valordefault,
                max = d.max,
                min = d.min,
                UtilizarMax = d.UtilizarMax,
                UtilizarMin = d.UtilizarMin
            };
        }
    }
}
