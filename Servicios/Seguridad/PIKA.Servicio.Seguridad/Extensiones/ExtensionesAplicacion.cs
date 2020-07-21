using System;
using System.Collections.Generic;
using System.Text;
using PIKA.Infraestructura.Comun;

namespace PIKA.Servicio.Seguridad
{
    public static partial class Extensiones
    {
        public static Aplicacion Copia(this Aplicacion d)
        {
            if (d == null) return null;
            return new Aplicacion()
            {
                Id = d.Id,
                Nombre = d.Nombre,
                Descripcion = d.Descripcion,
                UICulture = d.UICulture,
                ReleaseIndex = d.ReleaseIndex,
                Version = d.Version
            };
        }
    }
}
