using System;
using System.Collections.Generic;
using System.Text;
using PIKA.Infraestructura.Comun;

namespace PIKA.Servicio.Seguridad
{
    public static partial class Extensiones
    {
        public static ModuloAplicacion Copia(this ModuloAplicacion d)
        {
            if (d == null) return null;
            return new ModuloAplicacion()
            {
                Id = d.Id,
                Nombre = d.Nombre,
                Descripcion = d.Descripcion,
                UICulture = d.UICulture,
                Icono = d.Icono,
                AplicacionId = d.AplicacionId,
                Asegurable = d.Asegurable,
                PermisosDisponibles = d.PermisosDisponibles,
                ModuloPadreId = d.ModuloPadreId
            };
        }
    }
}
