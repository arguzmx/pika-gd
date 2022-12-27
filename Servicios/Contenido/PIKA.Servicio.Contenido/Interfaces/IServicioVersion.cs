using System;
using System.Collections.Generic;
using System.Text;
using RepositorioEntidades;
using PIKA.Modelo.Contenido;
using Version = PIKA.Modelo.Contenido.Version;
using System.Threading.Tasks;
using PIKA.Infraestructura.Comun;

namespace PIKA.Servicio.Contenido.Interfaces
{
  public interface IServicioVersion : IServicioRepositorioAsync<Version, string>, IServicioAutenticado<Version>
    {
        Task<List<string>> Purgar();
    }
}
