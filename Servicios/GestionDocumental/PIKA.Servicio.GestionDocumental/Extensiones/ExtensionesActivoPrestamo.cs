using System;
using System.Collections.Generic;
using System.Text;
using PIKA.Modelo.GestorDocumental;

namespace PIKA.Servicio.GestionDocumental
{
    public static partial class Extensiones
    {
        public static ActivoPrestamo Copia(this ActivoPrestamo ap)
        {
            if (ap == null) return null;
            return new ActivoPrestamo()
            {
                PrestamoId = ap.PrestamoId,
                ActivoId = ap.ActivoId,
                Devuelto = ap.Devuelto,
                FechaDevolucion = ap.FechaDevolucion
            };
        }
    }
}
