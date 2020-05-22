using PIKA.Modelo.Metadatos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PIKA.GD.API.Servicios
{
    public interface IAPICache<T>
    {
        Task<T> Obtiene(string id);

        Task Inserta(string id, T entidad, TimeSpan expira);

        Task Elimina(string id);
    }
}
