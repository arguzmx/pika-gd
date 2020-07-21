using System;
using System.Collections.Generic;
using System.Text;
using PIKA.Modelo.GestorDocumental;

namespace PIKA.Servicio.GestionDocumental
{
    public static partial class Extensiones
    {
        public static ComentarioPrestamo Copia(this ComentarioPrestamo cc)
        {
            if (cc == null) return null;
            return new ComentarioPrestamo()
            {
                Id = cc.Id,
                Comentario = cc.Comentario,
                Fecha = cc.Fecha,
                PrestamoId = cc.PrestamoId
            };
        }
    }
}
