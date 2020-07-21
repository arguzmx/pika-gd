using System;
using System.Collections.Generic;
using System.Text;
using PIKA.Infraestructura.Comun;

namespace PIKA.Servicio.Seguridad
{
    public static partial class Extensiones
    {
        public static TipoAdministradorModulo Copia(this TipoAdministradorModulo d)
        {
            if (d == null) return null;
            return new TipoAdministradorModulo()
            {
                Id = d.Id,
                AplicacionId = d.AplicacionId,
                ModuloId = d.ModuloId
            };
        }
    }
}
