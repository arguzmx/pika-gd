using System;
using System.Collections.Generic;
using System.Text;
using PIKA.Infraestructura.Comun;
using RepositorioEntidades;

namespace PIKA.Servicio.Seguridad.Interfaces
{
    public interface IServicioAplicacion : IServicioRepositorioAsync<Aplicacion, string>
    {

    }
}
