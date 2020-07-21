using System;
using System.Collections.Generic;
using System.Text;
using PIKA.Modelo.GestorDocumental;

namespace PIKA.Servicio.GestionDocumental
{
    public static partial class Extensiones
    {
        public static ComentarioTransferencia Copia(this ComentarioTransferencia c)
        {
            if (c == null) return null;
            return new ComentarioTransferencia()
            {
                Id = c.Id,
                TransferenciaId = c.TransferenciaId,
                UsuarioId = c.UsuarioId,
                Fecha = c.Fecha,
                Comentario = c.Comentario,
                Publico = c.Publico,
            };
        }
    }
}
