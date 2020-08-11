using System;
using System.Collections.Generic;
using System.Text;
using PIKA.Modelo.GestorDocumental;

namespace PIKA.Servicio.GestionDocumental
{
    public static partial class Extensiones
    {
        public static TipoArchivo Copia(this TipoArchivo t)
        {
            if (t == null) return null;
            return new TipoArchivo()
            {
                Id = t.Id,
                Nombre = t.Nombre
            };
        }
    }
}
