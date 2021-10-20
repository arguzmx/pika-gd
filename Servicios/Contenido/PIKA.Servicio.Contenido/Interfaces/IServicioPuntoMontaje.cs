﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using PIKA.Infraestructura.Comun.Seguridad;
using PIKA.Modelo.Contenido;
using RepositorioEntidades;

namespace PIKA.Servicio.Contenido.Interfaces
{
   public interface IServicioPuntoMontaje : IServicioRepositorioAsync<PuntoMontaje, string>,
        IServicioValorTextoAsync<PuntoMontaje>
    {
        UsuarioAPI usuario { get; set; }
        Task<List<string>> Purgar();
    }
}
