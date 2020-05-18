﻿using PIKA.Modelo.GestorDocumental;
using RepositorioEntidades;

namespace PIKA.Servicio.GestionDocumental.Interfaces
{
    public interface IServicioActivo : IServicioRepositorioAsync<Activo, string>
    {
    }
}