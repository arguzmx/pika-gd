using PIKA.Infraestructura.Comun;
using PIKA.Modelo.GestorDocumental;
using RepositorioEntidades;
using System;
using System.Collections.Generic;
using System.Text;

namespace PIKA.Servicio.GestionDocumental.Interfaces
{
    public interface IServicioAmpliacion : IServicioRepositorioAsync<Ampliacion, string>, IServicioAutenticado<Ampliacion>
    {
    }
}