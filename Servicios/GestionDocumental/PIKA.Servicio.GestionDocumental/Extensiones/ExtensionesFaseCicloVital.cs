using System;
using System.Collections.Generic;
using System.Text;
using PIKA.Modelo.GestorDocumental;

namespace PIKA.Servicio.GestionDocumental
{
    public static partial class Extensiones
    {
        public static FaseCicloVital Copia(this FaseCicloVital f)
        {
            if (f == null) return null;
            return new FaseCicloVital()
            {
                Id = f.Id,
                Nombre = f.Nombre,
            };
        }
    }
}
