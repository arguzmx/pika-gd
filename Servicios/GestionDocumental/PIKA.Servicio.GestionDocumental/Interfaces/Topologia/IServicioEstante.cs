
using PIKA.Modelo.GestorDocumental;
using PIKA.Modelo.GestorDocumental.Topologia;
using RepositorioEntidades;
using System;
using System.Collections.Generic;
using System.Text;

namespace PIKA.Servicio.GestionDocumental.Interfaces
{
    public interface IServicioEstante : IServicioRepositorioAsync<Estante, string>
    {
    }
}