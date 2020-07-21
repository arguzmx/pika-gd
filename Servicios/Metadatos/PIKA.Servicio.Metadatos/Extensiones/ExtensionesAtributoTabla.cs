using System;
using System.Collections.Generic;
using System.Text;
using PIKA.Modelo.Metadatos;

namespace PIKA.Servicio.Metadatos
{
    public static partial class Extensiones
    {
        public static AtributoTabla Copia(this AtributoTabla d)
        {
            if (d == null) return null;
            return new AtributoTabla()
            {
                Id = d.Id,
                PropiedadId = d.PropiedadId,
                Alternable = d.Alternable,
                IdTablaCliente = d.IdTablaCliente,
                Incluir = d.Incluir,
                IndiceOrdebnamiento = d.IndiceOrdebnamiento,
                Visible = d.Visible

            };
        }
    }
}
