using System;
using System.Collections.Generic;
using System.Text;
using PIKA.Infraestructura.Comun;

namespace PIKA.Servicio.Seguridad
{
    public static partial class Extensiones
    {
        public static TraduccionAplicacionModulo Copia(this TraduccionAplicacionModulo d)
        {
            if (d == null) return null;
            return new TraduccionAplicacionModulo()
            {
                Id = d.Id,
                Nombre = d.Nombre,
                Descripcion = d.Descripcion,
                AplicacionId = d.AplicacionId,
                ModuloId = d.ModuloId,
                UICulture = d.UICulture
            };
        }
    }
}
