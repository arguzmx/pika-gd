using System;
using System.Collections.Generic;
using System.Text;
using PIKA.Infraestructura.Comun;
using PIKA.Modelo.Contenido;
using RepositorioEntidades;

namespace PIKA.Servicio.Contenido.Interfaces
{
    public interface IServicioPermiso : IServicioRepositorioAsync<Permiso, string> , IServicioAutenticado<Permiso>
    {

    }
}
