using System;
using System.Collections.Generic;
using System.Text;
using PIKA.Modelo.GestorDocumental;

namespace PIKA.Servicio.GestionDocumental
{
    public static partial class Extensiones
    {
        public static TipoAmpliacion Copia(this TipoAmpliacion a)
        {
            if (a == null) return null;
            return new TipoAmpliacion()
            {
                Id = a.Id,
                Nombre = a.Nombre
            };
        }
    }
}
