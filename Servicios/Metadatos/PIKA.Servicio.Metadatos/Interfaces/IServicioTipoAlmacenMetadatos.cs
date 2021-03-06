﻿using System;
using System.Collections.Generic;
using System.Text;
using PIKA.Modelo.Metadatos;
using RepositorioEntidades;

namespace PIKA.Servicio.Metadatos.Interfaces
{
  public interface IServicioTipoAlmacenMetadatos : IServicioRepositorioAsync<TipoAlmacenMetadatos, string>, IServicioValorTextoAsync<TipoAlmacenMetadatos>
    {

    }
}
