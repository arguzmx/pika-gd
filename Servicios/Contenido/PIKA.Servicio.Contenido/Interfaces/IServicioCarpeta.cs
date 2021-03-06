﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using PIKA.Modelo.Contenido;
using RepositorioEntidades;

namespace PIKA.Servicio.Contenido.Interfaces
{
   public interface IServicioCarpeta : IServicioRepositorioAsync<Carpeta, string>,
        IServicioJerarquia<string>,
        IRepositorioJerarquia<Carpeta, string, string>
    {
        Task<List<string>> Purgar();
    }
}
