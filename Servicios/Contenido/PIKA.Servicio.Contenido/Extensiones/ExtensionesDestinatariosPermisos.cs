using System;
using System.Collections.Generic;
using System.Text;
using PIKA.Modelo.Contenido;

namespace PIKA.Servicio.Contenido
{
    public static partial class Extensiones
    {
        public static DestinatarioPermiso Copia(this DestinatarioPermiso d)
        {
            if (d == null) return null;
            var  o = new DestinatarioPermiso()
            {
                PermisoId = d.PermisoId,
                DestinatarioId = d.DestinatarioId,
                EsGrupo= d.EsGrupo,
            };
            return o;
        }
    }
}
