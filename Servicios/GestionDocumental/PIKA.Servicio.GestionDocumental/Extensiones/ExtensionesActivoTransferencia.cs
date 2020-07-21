using System;
using System.Collections.Generic;
using System.Text;
using PIKA.Modelo.GestorDocumental;

namespace PIKA.Servicio.GestionDocumental
{
    public static partial class Extensiones
    {
        public static ActivoTransferencia Copia(this ActivoTransferencia c)
        {
            if (c == null) return null;
            return new ActivoTransferencia()
            {
                ActivoId = c.ActivoId,
                TransferenciaId = c.TransferenciaId,
            };
        }

    }
}
