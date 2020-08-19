using System;
using System.Collections.Generic;
using System.Text;
using PIKA.Modelo.GestorDocumental;

namespace PIKA.Servicio.GestionDocumental
{
    public static partial class Extensiones
    {
        public static ActivoDeclinado Copia(this ActivoDeclinado c)
        {
            if (c == null) return null;
            return new ActivoDeclinado()
            {
                ActivoId = c.ActivoId,
                TransferenciaId = c.TransferenciaId,
                Motivo = c.Motivo,
                
            };
        }

    }
}
