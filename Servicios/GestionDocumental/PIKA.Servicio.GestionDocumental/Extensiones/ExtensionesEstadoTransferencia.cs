using System;
using System.Collections.Generic;
using System.Text;
using PIKA.Modelo.GestorDocumental;

namespace PIKA.Servicio.GestionDocumental
{
    public static partial class Extensiones
    {
        public static EstadoTransferencia Copia(this EstadoTransferencia e)
        {
            if (e == null) return null;
            return new EstadoTransferencia()
            {
                Id = e.Id,
                Nombre = e.Nombre,
            };
        }
    }
}
