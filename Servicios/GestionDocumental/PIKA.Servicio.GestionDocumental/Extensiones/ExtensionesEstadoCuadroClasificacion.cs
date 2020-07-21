using System;
using System.Collections.Generic;
using System.Text;
using PIKA.Modelo.GestorDocumental;

namespace PIKA.Servicio.GestionDocumental
{
    public static partial class Extensiones
    {
        public static EstadoCuadroClasificacion Copia(this EstadoCuadroClasificacion d)
        {
            if (d == null) return null;
            return new EstadoCuadroClasificacion()
            {
                Id = d.Id,
                Nombre = d.Nombre,
            };
        }
    }
}
