using PIKA.Modelo.Contacto;
using System;
using System.Collections.Generic;
using System.Resources;
using System.Text;

namespace PIKA.Servicio.Contacto
{
    public static partial class Extensiones
    {
        public static Pais Copia(this Pais d)
        {
            if (d == null) return null;

            return new Pais()
            {
                Id = d.Id,
                Nombre = d.Nombre,
                Estados = null
            };
        }

        public static Estado Copia(this Estado d)
        {
            if (d == null) return null;

            return new Estado()
            {
                Id = d.Id,
                Nombre = d.Nombre,
                PaisId = d.PaisId
            };
        }
    }
}
