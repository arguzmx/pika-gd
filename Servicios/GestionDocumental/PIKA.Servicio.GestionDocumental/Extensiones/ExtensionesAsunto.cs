using System;
using System.Collections.Generic;
using System.Text;
using PIKA.Modelo.GestorDocumental;

namespace PIKA.Servicio.GestionDocumental
{
    public static partial class Extensiones
    {
        public static Asunto Copia(this Asunto a)
        {
            if (a == null) return null;
            return new Asunto()
            {
                Id = a.Id,
                Contenido = a.Contenido
            };
        }
    }
}
