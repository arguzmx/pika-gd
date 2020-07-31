using PIKA.Modelo.Seguridad;
using System;
using System.Collections.Generic;
using System.Text;

namespace PIKA.Servicio.Seguridad
{
    public static partial class Extensiones
    {
        public static Genero Copia(this Genero d)
        {
            if (d == null) return null;
            return new Genero()
            {
                Id = d.Id,
                Nombre = d.Nombre,
            };
        }
    }
}