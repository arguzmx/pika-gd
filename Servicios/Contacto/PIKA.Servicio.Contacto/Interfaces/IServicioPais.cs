﻿using PIKA.Modelo.Contacto;
using RepositorioEntidades;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PIKA.Servicio.Contacto
{
    public interface IServicioPais : IServicioRepositorioAsync<Pais, string>, 
        IServicioValorTextoAsync<Pais>
    {

    }
}
