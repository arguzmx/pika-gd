using System;
using System.Collections.Generic;
using System.Text;
using PIKA.Modelo.GestorDocumental;

namespace PIKA.Servicio.GestionDocumental
{
    public static partial class Extensiones
    {
        public static ElementoClasificacion Copia(this ElementoClasificacion d)
        {
            if (d == null) return null;
            return new ElementoClasificacion()
            {
                Id = d.Id,
                Nombre = d.Nombre,
                Eliminada = d.Eliminada
            };
        }
    }
}
