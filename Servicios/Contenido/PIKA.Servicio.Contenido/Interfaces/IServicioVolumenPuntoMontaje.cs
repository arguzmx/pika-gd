using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using PIKA.Modelo.Contenido;
using RepositorioEntidades;

namespace PIKA.Servicio.Contenido.Interfaces
{
    public interface IServicioVolumenPuntoMontaje : IServicioRepositorioAsync<VolumenPuntoMontaje, string> 
    {
        Task<ICollection<string>> Eliminar(string puntoMontajeId, string[] ids);
    }
}
