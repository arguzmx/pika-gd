﻿using PIKA.Modelo.Contacto;
using System;
using System.Collections.Generic;
using System.Text;

namespace PIKA.Servicio.Contacto
{
    public static partial class Extensiones 

    {
        public static TipoFuenteContacto Copia(this TipoFuenteContacto d)
        {
            return new TipoFuenteContacto()
            {
                Id = d.Id,
                Nombre = d.Nombre,
            };
        }


    }
}
