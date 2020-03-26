using PIKA.Infraestructura.Comun;
using RepositorioEntidades;
using System;
using System.Collections.Generic;
using System.Text;

namespace PIKA.Servicio.Seguridad.Interfaces
{
    public interface IServicioAplicacion : IServicioRepositorioAsync<Aplicacion, string>
    {

    }
}
