﻿using PIKA.Infraestructura.Comun;
using PIKA.Modelo.Metadatos;
using RepositorioEntidades;
using System;
using System.Collections.Generic;
using System.Text;

namespace PIKA.Servicio.Metadatos.Interfaces
{
    public interface IServicioTipoDato: IServicioRepositorioAsync<TipoDato, string>, 
        IServicioValorTextoAsync<TipoDato>, IServicioAutenticado<TipoDato>
    {

    }
}
