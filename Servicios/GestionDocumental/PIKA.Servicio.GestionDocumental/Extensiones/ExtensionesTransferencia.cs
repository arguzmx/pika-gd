using System;
using System.Collections.Generic;
using System.Text;
using PIKA.Modelo.GestorDocumental;

namespace PIKA.Servicio.GestionDocumental
{
    public static partial class Extensiones
    {
        public static Transferencia Copia(this Transferencia t)
        {
            if (t == null) return null;
            return new Transferencia()
            {
                Id = t.Id,
                Nombre = t.Nombre,
                FechaCreacion = t.FechaCreacion,
                ArchivoOrigen = t.ArchivoOrigen,
                ArchivoDestino = t.ArchivoDestino,
                EstadoTransferenciaId = t.EstadoTransferenciaId,
                UsuarioId = t.UsuarioId
            };
        }
    }
}
