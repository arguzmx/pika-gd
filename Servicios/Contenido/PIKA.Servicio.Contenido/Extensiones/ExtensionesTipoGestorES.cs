using System;
using System.Collections.Generic;
using System.Text;
using PIKA.Modelo.Contenido;

namespace PIKA.Servicio.Contenido
{
    public static partial class Extensiones
    {
        public static TipoGestorES Copia(this TipoGestorES d)
        {
            if (d == null) return null;
            return new TipoGestorES()
            {
                Id = d.Id,
                Nombre = d.Nombre,
            };
        }
    }
}
