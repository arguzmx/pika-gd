﻿using System;
using System.Collections.Generic;
using System.Text;
using PIKA.Infraestructura.Comun;
using PIKA.Modelo.GestorDocumental;
using RepositorioEntidades;

namespace PIKA.Servicio.GestionDocumental.Interfaces
{
    public interface IServicioTipoDisposicionDocumental : IServicioRepositorioAsync<TipoDisposicionDocumental, string>, IServicioValorTextoAsync<TipoDisposicionDocumental>
        , IServicioAutenticado<TipoDisposicionDocumental>
    {
    }
   
}
