using System;
using System.Collections.Generic;
using System.Text;
using PIKA.Modelo.GestorDocumental;

namespace PIKA.Servicio.GestionDocumental
{
    public static partial class Extensiones
    {
        public static Prestamo Copia(this Prestamo p)
        {
            if (p == null) return null;
            return new Prestamo()
            {
                Id = p.Id,
                Folio = p.Folio,
                Eliminada = p.Eliminada,
                FechaCreacion = p.FechaCreacion,
                FechaDevolucion = p.FechaDevolucion,
                Comentarios = p.Comentarios,
                FechaProgramadaDevolucion = p.FechaProgramadaDevolucion,
                TieneDevolucionesParciales = p.TieneDevolucionesParciales
            };
        }
    }
}
