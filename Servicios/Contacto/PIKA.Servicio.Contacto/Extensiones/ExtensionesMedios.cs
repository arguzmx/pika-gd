﻿using PIKA.Modelo.Contacto;
using System;
using System.Collections.Generic;
using System.Text;

namespace PIKA.Servicio.Contacto
{
    public static partial class Extensiones

    {
        public static TipoMedio Copia(this TipoMedio d)
        {
            if (d == null) return null;

            return new TipoMedio()
            {
                Id = d.Id,
                Nombre = d.Nombre,
            };
        }


    }
}
