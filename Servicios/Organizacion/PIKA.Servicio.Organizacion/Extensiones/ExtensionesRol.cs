using System;
using System.Collections.Generic;
using System.Text;
using PIKA.Modelo.Organizacion;

namespace PIKA.Servicio.Organizacion
{
    public static partial class Extensiones
    {
        public static Rol Copia(this Rol d)
        {
            if (d == null) return null;
            return new Rol()
            {
                Id = d.Id,
                Nombre = d.Nombre,
                OrigenId = d.OrigenId,
                TipoOrigenId = d.TipoOrigenId
            };
        }
    }
}
